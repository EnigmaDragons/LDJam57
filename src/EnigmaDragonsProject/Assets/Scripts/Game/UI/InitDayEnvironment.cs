using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class InitDayEnvironment : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image bossCharacterImage;
    [SerializeField] private GameMusicPlayer musicPlayer;
    
    private GameState _lastKnownState;
    private bool _isInitialized = false;
    
    private void Start()
    {
        if (backgroundImage == null)
        {
            Debug.LogError("Background Image is not assigned to InitDayEnvironment");
            LogGameStateJson();
            return;
        }
        if (bossCharacterImage == null)
        {
            Debug.LogError("Boss Character Image is not assigned to InitDayEnvironment");
            LogGameStateJson();
            return;
        }
        if (musicPlayer == null)
        {
            Debug.LogError("Music Player is not assigned to InitDayEnvironment");
            LogGameStateJson();
            return;
        }
            
        // Subscribe to game state changes
        CurrentGameState.Subscribe(OnGameStateChanged, this);
        
        // Check if game state is already initialized, and if so, update the environment
        if (CurrentGameState.ReadOnly != null)
        {
            UpdateEnvironment(CurrentGameState.ReadOnly);
        }
        else
        {
            Debug.LogWarning("Game state is not initialized yet. Environment will be updated when the game state changes.");
            LogGameStateJson();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when this object is destroyed
        CurrentGameState.Unsubscribe(this);
    }
    
    private void OnGameStateChanged(GameStateChanged evt)
    {
        if (evt == null || evt.State == null)
        {
            Debug.LogError("Game state change event or state is null");
            LogGameStateJson();
            return;
        }
            
        // Store the latest state
        _lastKnownState = evt.State;
        _isInitialized = true;
        
        // Update environment when game state changes (like advancing to a new day)
        UpdateEnvironment(evt.State);
    }
    
    private void UpdateEnvironment()
    {
        if (_isInitialized && _lastKnownState != null)
        {
            UpdateEnvironment(_lastKnownState);
        }
        else
        {
            Debug.LogWarning("Attempting to update environment before state is initialized");
            LogGameStateJson();
        }
    }
    
    private void UpdateEnvironment(GameState gameState)
    {
        if (gameState == null)
        {
            Debug.LogError("Game state is null in UpdateEnvironment");
            LogGameStateJson();
            return;
        }
        
        // Get the boss for the current day
        Boss currentBoss = gameState.BossState.Boss;
        if (currentBoss == null)
        {
            Debug.LogError($"Current boss is null for day {gameState.CurrentDay}");
            LogGameStateJson();
            
            // Try to get the boss from the DayBossMap
            if (Bosses.DayBossMap.TryGetValue(gameState.CurrentDay, out Boss dayBoss))
            {
                currentBoss = dayBoss;
                Debug.Log($"Retrieved boss {dayBoss.Name} from DayBossMap");
                
                // Update the state with the correct boss
                UpdateBossInGameState(dayBoss);
            }
            else
            {
                Debug.LogError($"Could not find a boss for day {gameState.CurrentDay} in DayBossMap");
                return;
            }
        }
        
        // Update background image
        Sprite environmentSprite = currentBoss.Environment;
        if (environmentSprite == null)
        {
            Debug.LogError($"Environment sprite for {currentBoss.Name} is null. Path: Sprites/Environments/Env-{currentBoss.Name.Replace(" ", "")}");
            LogGameStateJson();
            return;
        }
        
        backgroundImage.sprite = environmentSprite;
        Debug.Log($"Setting environment background to {currentBoss.EnvironmentName}");
        
        // Update boss character image
        Sprite profileSprite = currentBoss.Profile;
        if (profileSprite == null)
        {
            Debug.LogError($"Profile sprite for {currentBoss.Name} is null. Path: Sprites/BossCharacters/Profile-{currentBoss.Name.Replace(" ", "")}");
            LogGameStateJson();
            return;
        }
        
        bossCharacterImage.sprite = profileSprite;
        Debug.Log($"Setting boss character to {currentBoss.Name}");
        
        // Update background music
        AudioClip bossMusic = currentBoss.Music;
        if (bossMusic == null)
        {
            string expectedPath = $"Music/V3 - {gameState.CurrentDay} - {currentBoss.Name.ToUpper()}'S {currentBoss.EnvironmentName.ToUpper()}";
            Debug.LogError($"Music for {currentBoss.Name} is null. Tried to load from: {expectedPath}");
            LogGameStateJson();
            return;
        }
        
        StartCoroutine(PlayMusicWithDelay(bossMusic, currentBoss.Name));
    }
    
    private void UpdateBossInGameState(Boss boss)
    {
        CurrentGameState.UpdateState(state => 
        {
            state.BossState = new BossState(boss, 0);
            return state;
        });
    }
    
    private IEnumerator PlayMusicWithDelay(AudioClip music, string bossName)
    {
        // Wait for a few frames before playing music
        yield return new WaitForEndOfFrame();
        
        musicPlayer.PlaySelectedMusicLooping(music);
        Debug.Log($"Playing boss music for {bossName}");
    }
    
    // Method to manually refresh the environment
    public void RefreshEnvironment()
    {
        UpdateEnvironment();
    }
    
    private void LogGameStateJson()
    {
        if (CurrentGameState.ReadOnly != null)
        {
            string json = JsonUtility.ToJson(CurrentGameState.ReadOnly, true);
            Debug.LogError($"Current Game State: {json}");
        }
        else
        {
            Debug.LogError("CurrentGameState.ReadOnly is null");
        }
    }
}