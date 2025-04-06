using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CheatCodes : MonoBehaviour
{
    [SerializeField] private bool enableCheats = true;
    [SerializeField] private KeyCode finishDayCheatKey = KeyCode.F12;
    [SerializeField] private KeyCode finishDayWith500Key = KeyCode.F11;
    [SerializeField] private int minRandomMoney = 10;
    [SerializeField] private int maxRandomMoney = 1000;
    [SerializeField] private float cheatCooldown = 5f; // Cooldown period in seconds
    
    private float _lastCheatTime = -5f; // Initialize to allow immediate use on start
    private bool _isProcessingCheat = false;
    
    private void Update()
    {
        if (!enableCheats || _isProcessingCheat) 
            return;
            
        // Check if we're still in cooldown period
        if (Time.time - _lastCheatTime < cheatCooldown)
            return;
            
        // Check for finish day cheat with random money
        if (Input.GetKeyDown(finishDayCheatKey))
        {
            FinishCurrentDayWithRandomMoney();
            _lastCheatTime = Time.time; // Record the time this cheat was triggered
        }
        
        // Check for finish day with specific amount
        if (Input.GetKeyDown(finishDayWith500Key))
        {
            FinishCurrentDayWithSpecificAmount(500); 
            _lastCheatTime = Time.time; // Record the time this cheat was triggered
        }
    }
    
    /// <summary>
    /// Finishes the current day by giving all players a semi-random amount of banked money.
    /// This can be called from anywhere to wrap up the current day.
    /// </summary>
    public void FinishCurrentDayWithRandomMoney()
    {
        if (CurrentGameState.ReadOnly == null || !CurrentGameState.ReadOnly.IsInitialized || _isProcessingCheat)
            return;
            
        _isProcessingCheat = true;
        
        GameState gameState = CurrentGameState.ReadOnly;
        Day currentDay = gameState.CurrentDay;
        
        // Give each active player a semi-random amount of money and mark them as finished for the day
        foreach (var player in gameState.PlayerStates)
        {
            if (player.IsActiveInDay)
            {
                // Generate a semi-random amount of money
                int randomAmount = Random.Range(minRandomMoney, maxRandomMoney);
                
                // Add the amount to the player's current round cash
                player.ChangeCurrentDayCash(randomAmount);
                
                // Mark them as finished for the day
                player.BankCash();
                
                Debug.Log($"Cheat: Gave {randomAmount} to {player.Player.Character.Name}");
            }
        }
        
        // End the day
        Message.Publish(new DayFinished(currentDay, gameState.PlayerStates));
        
        Debug.Log($"Cheat: Finished day {currentDay} with random money");
        
        // Release the lock after a short delay to prevent multiple keypresses
        Invoke("ResetCheatLock", 1.0f);
    }
    
    /// <summary>
    /// Finishes the current day by giving all players a specific amount of money.
    /// Useful for debugging or demonstrations with predictable results.
    /// </summary>
    public void FinishCurrentDayWithSpecificAmount(int amount)
    {
        if (CurrentGameState.ReadOnly == null || !CurrentGameState.ReadOnly.IsInitialized || _isProcessingCheat)
            return;
            
        _isProcessingCheat = true;
        
        GameState gameState = CurrentGameState.ReadOnly;
        Day currentDay = gameState.CurrentDay;
        
        // Give each active player the specified amount of money
        foreach (var player in gameState.PlayerStates)
        {
            if (player.IsActiveInDay)
            {
                // Add the specific amount to the player's current round cash
                player.ChangeCurrentDayCash(amount);
                
                // Mark them as finished for the day
                player.BankCash();
                
                Debug.Log($"Cheat: Gave ${amount} to {player.Player.Character.Name}");
            }
        }
        
        // End the day
        Message.Publish(new DayFinished(currentDay, gameState.PlayerStates));
        
        Debug.Log($"Cheat: Finished day {currentDay} with ${amount} for each player");
        
        // Release the lock after a short delay to prevent multiple keypresses
        Invoke("ResetCheatLock", 1.0f);
    }
    
    private void ResetCheatLock()
    {
        _isProcessingCheat = false;
    }
} 