using UnityEngine;
using System.Collections;
using TMPro;

public class InitDayEnvironment : MonoBehaviour
{
    [SerializeField] private GameObject[] bossEnvironments;
    [SerializeField] private GameMusicPlayer musicPlayer;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private PlayerUi[] playerUis;
    
    private GameState _lastKnownState;
    private bool _isInitialized = false;
    
    private void Start()
    {
        if (musicPlayer == null || bossNameText == null || locationText == null)
        {
            Debug.LogError("One or more required components not assigned to InitDayEnvironment");
            return;
        }
            
        // Subscribe to game state changes
        CurrentGameState.Subscribe(OnGameStateChanged, this);
        
        // Check if game state is already initialized, and if so, update the environment
        if (CurrentGameState.ReadOnly != null)
        {
            UpdateEnvironment(CurrentGameState.ReadOnly);
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
            return;
            
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
    }
    
    private void UpdateEnvironment(GameState gameState)
    {
        if (gameState == null)
            return;
        
        // Get the boss for the current day
        Boss currentBoss = gameState.BossState.Boss;
        if (currentBoss == null)
        {
            // Try to get the boss from the DayBossMap
            if (Bosses.DayBossMap.TryGetValue(gameState.CurrentDay, out Boss dayBoss))
            {
                currentBoss = dayBoss;
                
                // Update the state with the correct boss
                UpdateBossInGameState(dayBoss);
            }
            else
            {
                return;
            }
        }
        
        bossEnvironments.ForEachIndex((g, i) => g.SetActive(i == currentBoss.Id));
        
        // Update boss name and location text
        bossNameText.text = currentBoss.Name;
        locationText.text = currentBoss.EnvironmentName;
        
        // Update day text
        if (dayText != null)
        {
            dayText.text = gameState.CurrentDay.ToString();
        }
        
        // Update background music
        AudioClip bossMusic = currentBoss.Music;
        if (bossMusic != null)
        {
            StartCoroutine(PlayMusicWithDelay(bossMusic));
        }

        // Update player UIs
        for (int i = 0; i < playerUis.Length && i < gameState.PlayerStates.Length; i++)
        {
            playerUis[i].InitWithPlayerState(gameState.PlayerStates[i]);
        }
    }
    
    private void UpdateBossInGameState(Boss boss)
    {
        CurrentGameState.UpdateState(state => 
        {
            state.BossState = new BossState(boss, 0);
            return state;
        });
    }
    
    private IEnumerator PlayMusicWithDelay(AudioClip music)
    {
        // Wait for a few frames before playing music
        yield return new WaitForEndOfFrame();
        
        musicPlayer.PlaySelectedMusicLooping(music);
    }
    
    // Method to manually refresh the environment
    public void RefreshEnvironment()
    {
        UpdateEnvironment();
    }
}