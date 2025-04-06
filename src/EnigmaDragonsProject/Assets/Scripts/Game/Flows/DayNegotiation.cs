using System;
using System.Collections.Generic;
using UnityEngine;

public class DayNegotiation
{
    public enum Phase
    {
        Setup,
        PlayerTurns,
        DayEnd
    }

    private enum SetupStep
    {
        InitDeck,
        DetermineFirstPlayer
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

    private Phase _currentPhase;
    private SetupStep _currentSetupStep;
    private PlayerTurnStep _currentPlayerTurnStep;
    private DayEndStep _currentDayEndStep;
    
    private readonly GameState _gameState;
    
    public DayNegotiation(GameState gameState)
    {
        _gameState = gameState;
        _currentPhase = Phase.Setup;
        _currentSetupStep = SetupStep.InitDeck;
    }
    
    public void Start()
    {
        // Reset day-specific player states
        foreach (var player in _gameState.PlayerStates)
        {
            player.NotifyDayChanged();
            // Set players as active for the new day
            player.ResetActiveStatus();
        }
        
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
        switch (_currentSetupStep)
        {
            case SetupStep.InitDeck:
                _gameState.CurrentDeck = BasicDeck.CreateStandardDeck();
                _gameState.CurrentDeck.Shuffle();
                _currentSetupStep = SetupStep.DetermineFirstPlayer;
                ProcessCurrentStep();
                break;
                
            case SetupStep.DetermineFirstPlayer:
                // Store the turn index in game state
                _gameState.PlayerTurnIndex = UnityEngine.Random.Range(0, GetActivePlayersList().Count);
                
                _currentPhase = Phase.PlayerTurns;
                _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
                ProcessCurrentStep();
                break;
                
            default:
                Debug.LogError("Unknown setup step: " + _currentSetupStep);
                break;
        }
    }
        
    private void ProcessPlayerTurnStep()
    {
        // If no active players, go to day end
        if (GetActivePlayersList().Count == 0)
        {
            _currentPhase = Phase.DayEnd;
            _currentDayEndStep = DayEndStep.ShowResults;
            ProcessCurrentStep();
            return;
        }
        
        switch (_currentPlayerTurnStep)
        {
            case PlayerTurnStep.AwaitPlayerSelection:
                // ATTN: Need to implement UI hooks to notify when it's a player's turn
                // The UI will call DrawCard or AcceptOffer based on player input
                break;
                
            case PlayerTurnStep.ProcessPlayerSelection:
                // This step is handled by external inputs calling DrawCard() or AcceptOffer()
                break;
                
            case PlayerTurnStep.MoveToNextPlayer:
                // Move to the next player in the turn order who is still active
                List<PlayerState> activePlayers = GetActivePlayersList();
                if (activePlayers.Count > 0)
                {
                    _gameState.PlayerTurnIndex = (_gameState.PlayerTurnIndex + 1) % activePlayers.Count;
                }
                
                // Reset to await player selection
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
                // Move to the next day in the game
                _gameState.AdvanceToNextDay();
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
        
        // Get the current player
        PlayerState currentPlayer = GetActivePlayer();
        if (currentPlayer == null)
            return;
            
        // ATTN: Need to implement proper card drawing from deck and card effects
        ProcessDrawnCard(currentPlayer);
        
        // Check if we need to move to the next player
        List<PlayerState> activePlayers = GetActivePlayersList();
        if (activePlayers.Contains(currentPlayer))
        {
            _currentPlayerTurnStep = PlayerTurnStep.MoveToNextPlayer;
        }
        else if (activePlayers.Count > 0)
        {
            // If current player was removed but others remain
            _gameState.PlayerTurnIndex = _gameState.PlayerTurnIndex % activePlayers.Count;
            _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
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
        var card = _gameState.CurrentDeck.DrawOne();
        card.Apply(_gameState, currentPlayer);
    }
    
    // Called by UI when player chooses to accept current offer
    public void AcceptOffer()
    {
        if (_currentPhase != Phase.PlayerTurns || _currentPlayerTurnStep != PlayerTurnStep.AwaitPlayerSelection)
            return;
            
        _currentPlayerTurnStep = PlayerTurnStep.ProcessPlayerSelection;
        
        // Get the current player
        PlayerState currentPlayer = GetActivePlayer();
        if (currentPlayer == null)
            return;
            
        // Player banks their current money and is removed from the turn order
        currentPlayer.BankCash();
        
        // ATTN: Need to implement UI notification for player accepting offer
        
        // Check if we need to end the day or move to next player
        List<PlayerState> activePlayers = GetActivePlayersList();
        if (activePlayers.Count > 0)
        {
            _gameState.PlayerTurnIndex = _gameState.PlayerTurnIndex % activePlayers.Count;
            _currentPlayerTurnStep = PlayerTurnStep.AwaitPlayerSelection;
        }
        else
        {
            // No players left, end the day
            _currentPhase = Phase.DayEnd;
            _currentDayEndStep = DayEndStep.ShowResults;
        }
        
        ProcessCurrentStep();
    }
    
    // Called by UI when ready to advance to next day
    public void AdvanceToNextDay()
    {
        if (_currentPhase != Phase.DayEnd || _currentDayEndStep != DayEndStep.ShowResults)
            return;
            
        _currentDayEndStep = DayEndStep.ProcessToNextDay;
        ProcessCurrentStep();
    }
    
    // Helper methods to access game state
    private List<PlayerState> GetActivePlayersList()
    {
        List<PlayerState> activePlayers = new List<PlayerState>();
        foreach (var player in _gameState.PlayerStates)
        {
            if (player.IsActiveInDay)
            {
                activePlayers.Add(player);
            }
        }
        return activePlayers;
    }
    
    private PlayerState GetActivePlayer()
    {
        List<PlayerState> activePlayers = GetActivePlayersList();
        if (activePlayers.Count == 0)
            return null;
            
        if (_gameState.PlayerTurnIndex >= activePlayers.Count)
            _gameState.PlayerTurnIndex = 0;
            
        return activePlayers[_gameState.PlayerTurnIndex];
    }
    
    // Public getters for UI to check game state
    public PlayerState GetCurrentPlayer() => GetActivePlayer();
    public Phase GetCurrentPhase() => _currentPhase;
    public bool IsAwaitingPlayerInput() => 
        _currentPhase == Phase.PlayerTurns && _currentPlayerTurnStep == PlayerTurnStep.AwaitPlayerSelection;
    public bool IsDayComplete() => 
        _currentPhase == Phase.DayEnd && _currentDayEndStep == DayEndStep.ProcessToNextDay;
    
    // ATTN: Need to implement the following additional features:
    // - Cat special abilities/powers (once per day)
    // - Turn counter to track 6-card draw limit
    // - Visual feedback for player turns and state changes
    // - AI decision making for non-player cats
}