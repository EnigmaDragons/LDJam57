using UnityEngine;

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

    private void OnEnable()
    {
        Message.Subscribe<Finished<ShowDieRoll>>(_ => FinishSetup(), this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    public void Start()
    {
        foreach (var player in CurrentGameState.ReadOnly.PlayerStates) 
            player.NotifyDayChanged();

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
        var dieRollResult = UnityEngine.Random.Range(1, 7);
        Debug.Log($"Rolling a D6 to determine first player. Result: {dieRollResult}");
        CurrentGameState.UpdateState(gs =>
        {
            gs.CurrentDeck = BasicDeck.CreateStandardDeck().Shuffled();
            gs.PlayerTurnIndex = (dieRollResult - 1) % gs.PlayerStates.Length;
        });
        
        Message.Publish(new ShowDieRoll(dieRollResult, 6));
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
            _currentPhase = Phase.DayEnd;
            _currentDayEndStep = DayEndStep.ShowResults;
            ProcessCurrentStep();
            return;
        }
        
        switch (_currentPlayerTurnStep)
        {
            case PlayerTurnStep.AwaitPlayerSelection:
                Message.Publish(new ReadyForPlayerSelection(CurrentGameState.ReadOnly.ActivePlayer));
                break;
                
            case PlayerTurnStep.ProcessPlayerSelection:
                // This step is handled by external inputs calling DrawCard() or AcceptOffer()
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
                // ATTN: Need to implement UI to show day results and final standings
                // UI should call AdvanceToNextDay() when player is ready to continue
                break;
                
            case DayEndStep.ProcessToNextDay:
                CurrentGameState.UpdateState(gs => gs.AdvanceToNextDay());
                // ATTN: Need to implement day completion event/callback to notify game flow
                break;
                
            default:
                Debug.LogError("Unknown day end step: " + _currentDayEndStep);
                break;
        }
    }
    
    // Called by UI when player chooses to draw a card
    public void DrawCard()
    {
        if (_currentPhase != Phase.PlayerTurns || _currentPlayerTurnStep != PlayerTurnStep.AwaitPlayerSelection)
            return;
            
        _currentPlayerTurnStep = PlayerTurnStep.ProcessPlayerSelection;
        
        // ATTN: Need to implement proper card drawing from deck and card effects
        var currentPlayer = CurrentGameState.ReadOnly.ActivePlayer;
        ProcessDrawnCard(currentPlayer);
        
        if (currentPlayer.IsActiveInDay)
        {
            _currentPlayerTurnStep = PlayerTurnStep.MoveToNextPlayer;
        }
        else if (CurrentGameState.ReadOnly.ActivePlayerCount > 0)
        {
            _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
            CurrentGameState.UpdateState(gs => gs.MoveToNextActivePlayer());
        }
        else
        {
            // No players left, end the day
            _currentPhase = Phase.DayEnd;
            _currentDayEndStep = DayEndStep.ShowResults;
        }
        
        ProcessCurrentStep();
    }
    
    private void ProcessDrawnCard(PlayerState currentPlayer)
    {
        var card = CurrentGameState.ReadOnly.CurrentDeck.DrawOne();
        CurrentGameState.UpdateState(gs => gs);
        card.Apply(CurrentGameState.ReadOnly, currentPlayer);
    }
    
    public void AcceptOffer()
    {
        if (_currentPhase != Phase.PlayerTurns || _currentPlayerTurnStep != PlayerTurnStep.AwaitPlayerSelection)
            return;
            
        _currentPlayerTurnStep = PlayerTurnStep.ProcessPlayerSelection;
        CurrentGameState.ReadOnly.ActivePlayer.BankCash();
        
        // ATTN: Need to implement UI notification for player accepting offer

        if (CurrentGameState.ReadOnly.ActivePlayerCount > 0)
        {
            CurrentGameState.UpdateState(gs => gs.MoveToNextActivePlayer());
            _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
        }
        else
        {
            _currentPhase = Phase.DayEnd;
            _currentDayEndStep = DayEndStep.ShowResults;
        }
        
        ProcessCurrentStep();
    }
    
    public void AdvanceToNextDay()
    {
        if (_currentPhase != Phase.DayEnd || _currentDayEndStep != DayEndStep.ShowResults)
            return;
            
        _currentDayEndStep = DayEndStep.ProcessToNextDay;
        ProcessCurrentStep();
    }
}