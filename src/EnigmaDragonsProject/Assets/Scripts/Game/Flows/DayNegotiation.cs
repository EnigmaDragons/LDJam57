using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class DayNegotiation : MonoBehaviour
{
    public enum Phase
    {
        Setup,
        PlayerTurns,
        DayEnd
    }
    
    private enum PlayerTurnStep
    {
        AwaitPlayerSelection,
        ProcessPlayerSelection,
        MoveToNextPlayer
    }

    private enum DayEndStep
    {
        ShowResults,
        ProcessToNextDay
    }

    private Phase _currentPhase = Phase.Setup;
    private PlayerTurnStep _currentPlayerTurnStep;
    private DayEndStep _currentDayEndStep;
    
    // Public accessor for the current phase
    public Phase CurrentPhase => _currentPhase;

    private void OnEnable()
    {
        Message.Subscribe<Finished<ShowDieRoll>>(_ => FinishSetup(), this);
        Message.Subscribe<Finished<ShowCardDrawn>>(OnCardDrawShown, this);
        Message.Subscribe<ContinueToNextDayButtonPressed>(_ => AdvanceToNextDay(), this);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    public void Start()
    {
        foreach (var player in CurrentGameState.ReadOnly.PlayerStates) 
            player.NotifyDayChanged();

        CharacterPowerProcessor.TriggerStartOfWeekPowers();
        ProcessCurrentStep();
    }
    
    public void ProcessCurrentStep()
    {
        switch (_currentPhase)
        {
            case Phase.Setup:
                ProcessSetupStep();
                break;
            case Phase.PlayerTurns:
                ProcessPlayerTurnStep();
                break;
            case Phase.DayEnd:
                ProcessDayEndStep();
                break;
            default:
                Debug.LogError("Unknown phase: " + _currentPhase);
                break;
        }
    }
    
    private void ProcessSetupStep()
    {
        CurrentGameState.UpdateState(gs =>
        {
            gs.CurrentDeck = BasicDeck.CreateStandardDeck().Shuffled();
        });
        
        // Play deck shuffled sound when we create a new deck
        Message.Publish(new PlayUiSound(SoundType.DeckShuffled));
        
        // Wait 2.5 seconds after shuffling before rolling
        StartCoroutine(DelayedDieRoll());
    }

    private System.Collections.IEnumerator DelayedDieRoll()
    {
        yield return new WaitForSeconds(2.5f);
        
        var dieRollResult = UnityEngine.Random.Range(1, 7);
        Debug.Log($"Rolling a D6 to determine first player. Result: {dieRollResult}");
        
        CurrentGameState.UpdateState(gs =>
        {
            gs.PlayerTurnIndex = (dieRollResult - 1) % gs.PlayerStates.Length;
        });
        
        Message.Publish(new ShowDieRoll(dieRollResult, 6));
        
        CharacterPowerProcessor.TriggerStartOfDayPowers();
    }

    private void FinishSetup()
    {
        _currentPhase = Phase.PlayerTurns;
        _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
        ProcessCurrentStep();
    }
        
    private void ProcessPlayerTurnStep()
    {
        // If no active players, go to day end
        if (CurrentGameState.ReadOnly.ActivePlayerCount == 0)
        {
            TransitionToDayEnd();
            return;
        }
        
        switch (_currentPlayerTurnStep)
        {
            case PlayerTurnStep.AwaitPlayerSelection:
                Message.Publish(new ReadyForPlayerSelection(CurrentGameState.ReadOnly.ActivePlayer));
                break;
            
            case PlayerTurnStep.MoveToNextPlayer:
                CurrentGameState.UpdateState(gs => gs.MoveToNextActivePlayer());
                _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
                ProcessCurrentStep();
                break;
                
            default:
                Debug.LogError("Unknown player turn step: " + _currentPlayerTurnStep);
                break;
        }
    }
    
    private void ProcessDayEndStep()
    {
        switch (_currentDayEndStep)
        {
            case DayEndStep.ShowResults:
                break;
                
            case DayEndStep.ProcessToNextDay:
                CurrentGameState.UpdateState(gs => gs.AdvanceToNextDay());
                
                // Reset phase to Setup to begin the new day
                _currentPhase = Phase.Setup;
                Debug.Log($"Day transition complete. Starting new day: {CurrentGameState.ReadOnly.CurrentDay}");
                
                // Play deck shuffled sound when we start a new day
                Message.Publish(new PlayUiSound(SoundType.DeckShuffled));
                
                // Publish day transition completed message
                Message.Publish(new DayTransitionCompleted(CurrentGameState.ReadOnly.CurrentDay));
                
                // Restart the flow for the new day
                ProcessCurrentStep();
                break;
                
            default:
                Debug.LogError("Unknown day end step: " + _currentDayEndStep);
                break;
        }
    }
    
    public void DrawCard()
    {
        if (_currentPhase != Phase.PlayerTurns || _currentPlayerTurnStep != PlayerTurnStep.AwaitPlayerSelection)
            return;
            
        _currentPlayerTurnStep = PlayerTurnStep.ProcessPlayerSelection;
        
        // Check if this is the player's first card in this round
        var currentPlayer = CurrentGameState.ReadOnly.ActivePlayer;
        bool isFirstCard = currentPlayer.CurrentRoundCash == 0;
        
        // Draw a card from the deck
        var card = CurrentGameState.ReadOnly.CurrentDeck.DrawOne();
        
        // CHEAT: If it's the player's first card and it's a Snap card, put it back and draw a non-snap card
        if (isFirstCard && card is SnapCard)
        {
            Debug.Log("CHEAT ACTIVATED: Player would have drawn a Snap as their first card. Drawing a different card instead.");
            
            // Put the Snap card back (at a random position)
            CurrentGameState.UpdateState(gs => {
                gs.CurrentDeck.ShuffleCardIn(card);
            });
            
            // Keep drawing until we get a non-Snap card
            bool foundNonSnapCard = false;
            while (!foundNonSnapCard)
            {
                card = CurrentGameState.ReadOnly.CurrentDeck.DrawOne();
                foundNonSnapCard = !(card is SnapCard);
                
                if (!foundNonSnapCard)
                {
                    // Put the Snap card back again
                    CurrentGameState.UpdateState(gs => {
                        gs.CurrentDeck.ShuffleCardIn(card);
                    });
                }
            }
        }
        
        CurrentGameState.UpdateState(gs => gs);
        Message.Publish(new ShowCardDrawn(card));
    }
    
    private void OnCardDrawShown(Finished<ShowCardDrawn> msg)
    {
        var currentPlayer = CurrentGameState.ReadOnly.ActivePlayer;
        if (currentPlayer == null)
            return;

        var card = msg.Message.Card;
        // NOTE: It's not clear that this logic belongs here... but for now we have to implement fast
        var character = currentPlayer.Player.Character;
        var power = character.Power;
        var skipApply = false;
        if (card.Type == CardType.Snap && power.PowerType == PowerType.DiscardSnapAfterDrawn && power.IsAvailable)
        {
            power.Apply(new PowerContext(CurrentGameState.ReadOnly, currentPlayer));
            skipApply = true;
        }

        if (!skipApply)
        {
            // Apply card effects to the player
            card.Apply(CurrentGameState.ReadOnly, currentPlayer);

            // Update Boss Mood
            var cardBossMoodMod = card.BossMoodMod;
            if (cardBossMoodMod > 0 && power.PowerType == PowerType.NoMoodEscalation)
            {
                Debug.Log($"Card has a mood mod of {cardBossMoodMod}, but {character.PowerName} is active. Setting mood mod to 0.");
                cardBossMoodMod = 0;
            }
            
            if (cardBossMoodMod != 0) {
                CurrentGameState.UpdateState(gs =>
                {
                    // If the card affects boss mood, update it
                    if (cardBossMoodMod != 0)
                    {
                        // Get the number of snaps to add based on mood change
                        int snapsToAdd = gs.BossState.UpdateMoodAndGetSnapsChanges(cardBossMoodMod);

                        // If snaps should be added, create and shuffle them into the deck
                        if (snapsToAdd > 0)
                        {
                            Debug.Log($"Boss mood increased! Adding {snapsToAdd} snap(s) to the deck");

                            // Create snap cards and add them to the deck
                            for (int i = 0; i < snapsToAdd; i++)
                            {
                                gs.CurrentDeck.ShuffleCardIn(new SnapCard());
                            }

                            // Notify of new snaps
                            Message.Publish(new SnapsAddedToDeck(snapsToAdd));

                            // Play deck shuffled sound when we add snaps to the deck
                            Message.Publish(new PlayUiSound(SoundType.DeckShuffledShort));
                        }
                    }
                });
            }
        }

        ContinuePlayerTurnFlow();
    }
    
    public void AcceptOffer()
    {
        if (_currentPhase != Phase.PlayerTurns || _currentPlayerTurnStep != PlayerTurnStep.AwaitPlayerSelection)
            return;
            
        _currentPlayerTurnStep = PlayerTurnStep.ProcessPlayerSelection;
        CurrentGameState.ReadOnly.ActivePlayer.BankCash();
        
        // Play happy card sound when accepting an offer
        Message.Publish(new PlayUiSound(SoundType.AcceptOffer));
        
        // ATTN: Need to implement UI notification for player accepting offer

        ContinuePlayerTurnFlow();
    }
    
    private void ContinuePlayerTurnFlow()
    {
        if (CurrentGameState.ReadOnly.ActivePlayerCount > 0)
        {
            _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
            CurrentGameState.UpdateState(gs => gs.MoveToNextActivePlayer());
        }
        else
        {
            TransitionToDayEnd();
        }
        
        ProcessCurrentStep();
    }
    
    private void TransitionToDayEnd()
    {
        CurrentGameState.UpdateState(gs => gs.PlayerTurnIndex = -1);
        _currentPhase = Phase.DayEnd;
        _currentDayEndStep = DayEndStep.ShowResults;
        
        // Publish day finished message
        GameState state = CurrentGameState.ReadOnly;
        Message.Publish(new DayFinished(state.CurrentDay, state.PlayerStates));
        Debug.Log($"Day {state.CurrentDay} has finished with {state.PlayerStates.Length} players");
    }
    
    public void AdvanceToNextDay()
    {           
        _currentDayEndStep = DayEndStep.ProcessToNextDay;
        ProcessCurrentStep();
    }
}