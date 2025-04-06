using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private Image characterFaceImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI totalCashText;
    [SerializeField] private TextMeshProUGUI todayCashText;
    [SerializeField] private Toggle powerAvailableToggle;
    
    private int _playerId;
    private PlayerState _lastKnownPlayerState;
    
    private void Start()
    {
        // Subscribe to game state changes
        CurrentGameState.Subscribe(OnGameStateChanged, this);
        
        // Check if game state is already initialized, and if so, update the UI
        if (CurrentGameState.ReadOnly != null && CurrentGameState.ReadOnly.IsInitialized)
        {
            UpdateFromGameState(CurrentGameState.ReadOnly);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when this object is destroyed
        CurrentGameState.Unsubscribe(this);
    }
    
    public void InitWithPlayerState(PlayerState playerState)
    {
        if (playerState == null)
            return;
            
        _playerId = playerState.Player.Index;
        _lastKnownPlayerState = playerState;
        UpdateUi(playerState);
    }
    
    private void OnGameStateChanged(GameStateChanged evt)
    {
        if (evt == null || evt.State == null)
            return;
            
        UpdateFromGameState(evt.State);
    }
    
    private void UpdateFromGameState(GameState gameState)
    {
        if (gameState == null || gameState.PlayerStates == null)
            return;
            
        foreach (var playerState in gameState.PlayerStates)
        {
            if (playerState.Player.Index == _playerId)
            {
                _lastKnownPlayerState = playerState;
                UpdateUi(playerState);
                break;
            }
        }
    }
    
    private void UpdateUi(PlayerState playerState)
    {
        if (playerState == null)
            return;
            
        // Update character face
        if (characterFaceImage != null && playerState.Player.Character != null)
        {
            characterFaceImage.sprite = playerState.Player.Character.Face;
        }
        
        // Update character name
        if (characterNameText != null && playerState.Player.Character != null)
        {
            characterNameText.text = playerState.Player.Character.Name;
        }
        
        // Update total cash
        if (totalCashText != null)
        {
            totalCashText.text = $"Total: ${playerState.BankedCash + playerState.CurrentRoundCash}";
        }
        
        // Update today's cash
        if (todayCashText != null)
        {
            todayCashText.text = $"Today: ${playerState.CurrentRoundCash}";
        }
        
        // Update power available toggle
        if (powerAvailableToggle != null)
        {
            powerAvailableToggle.isOn = !playerState.PowerUsedToday;
            powerAvailableToggle.interactable = false; // Just for display, not interactive
        }
    }
    
    // Method to manually refresh the UI
    public void RefreshUi()
    {
        if (_lastKnownPlayerState != null)
        {
            UpdateUi(_lastKnownPlayerState);
        }
    }
}
