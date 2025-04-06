using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private Image characterFaceImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI totalCashText;
    [SerializeField] private TextMeshProUGUI totalFinalCashText;
    [SerializeField] private TextMeshProUGUI todayCashText;
    [SerializeField] private TextMeshProUGUI placeLabelText;
    [SerializeField] private Toggle powerAvailableToggle;
    [SerializeField] private CanvasGroup activeGroup;
    [SerializeField] private Image statusColorBg;
    [SerializeField] private GameObject finalCashPanel;
    [SerializeField] private GameObject dayCashPanel;
    [SerializeField] private bool isFinalCashDisplay = false;
    
    private int _playerId;
    private PlayerState _lastKnownPlayerState;
    private Vector3 _originalScale;
    
    private void Start()
    {
        // Store original scale for animations
        _originalScale = transform.localScale;
        
        // Subscribe to game state changes
        CurrentGameState.Subscribe(OnGameStateChanged, this);
        
        // Check if game state is already initialized, and if so, update the UI
        if (CurrentGameState.ReadOnly != null && CurrentGameState.ReadOnly.IsInitialized)
        {
            UpdateFromGameState(CurrentGameState.ReadOnly);
        }
        
        dayCashPanel.SetActive(!isFinalCashDisplay);
        finalCashPanel.SetActive(isFinalCashDisplay);
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

        bool isCurrentTurnPlayer = playerState.IsActiveInDay && 
            CurrentGameState.ReadOnly != null && 
            CurrentGameState.ReadOnly.PlayerTurnIndex == _playerId;

        if (isCurrentTurnPlayer)
        {
            // Make the panel 10% larger when it's this player's turn
            transform.DOScale(_originalScale * 1.1f, 0.5f).SetEase(Ease.OutQuad);
            
            activeGroup.DOFade(1f, 0.5f);
        }
        else if (playerState.IsActiveInDay) 
        {
            // Return to original size when not active
            transform.DOScale(_originalScale, 0.5f).SetEase(Ease.OutQuad);
            activeGroup.DOFade(1f, 0.5f);
        }
        else
        {
            // Return to original size when not active
            transform.DOScale(_originalScale, 0.5f).SetEase(Ease.OutQuad);
            
            // Fade out the player UI when they're not active
            if (activeGroup != null)
            {
                // Use DoTween to animate the alpha from 1 to 0.4
                activeGroup.DOFade(0.4f, 0.5f);
                
                // Change status background color based on whether they banked cash
                if (statusColorBg != null && !isFinalCashDisplay)
                {
                    Color targetColor = playerState.CurrentRoundCash > 0 
                        ? Color.green 
                        : Color.red;
                    
                    // Ensure full opacity
                    targetColor.a = 1f;
                    
                    statusColorBg.DOColor(targetColor, 0.5f);
                }
            }
            else
            {
                Debug.LogWarning("CanvasGroup component not found on PlayerUI. Cannot fade out inactive player.");
            }
        }
            
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
        
        totalCashText.text = $"Total: ${playerState.BankedCash + playerState.CurrentRoundCash}";
        totalFinalCashText.text = $"${playerState.BankedCash + playerState.CurrentRoundCash}";
        
        // Update today's cash
        if (todayCashText != null)
        {
            todayCashText.text = $"Today: ${playerState.CurrentRoundCash}";
        }
        
        // Update player place
        if (placeLabelText != null && CurrentGameState.ReadOnly != null)
        {
            int place = GetPlayerPlace(playerState);
            placeLabelText.text = GetOrdinalPlaceString(place);
        }
        
        // Update power available toggle
        if (powerAvailableToggle != null)
        {
            powerAvailableToggle.isOn = !playerState.PowerUsedToday;
            powerAvailableToggle.interactable = false; // Just for display, not interactive
        }
    }
    
    private int GetPlayerPlace(PlayerState playerState)
    {
        if (CurrentGameState.ReadOnly == null || CurrentGameState.ReadOnly.PlayerStates == null)
            return 1;
            
        int place = 1;
        int currentPlayerTotalCash = playerState.BankedCash + playerState.CurrentRoundCash;
        int currentPlayerBankedCash = playerState.BankedCash;
        
        foreach (var otherPlayer in CurrentGameState.ReadOnly.PlayerStates)
        {
            // Skip comparing against self
            if (otherPlayer.Player.Index == playerState.Player.Index)
                continue;
                
            int otherTotalCash = otherPlayer.BankedCash + otherPlayer.CurrentRoundCash;
            
            // If other player has more cash, increase place
            if (otherTotalCash > currentPlayerTotalCash)
            {
                place++;
            }
            // If tied on total cash, break tie using banked cash
            else if (otherTotalCash == currentPlayerTotalCash && 
                     otherPlayer.BankedCash > currentPlayerBankedCash)
            {
                place++;
            }
        }
        
        return place;
    }
    
    private string GetOrdinalPlaceString(int place)
    {
        switch (place)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            case 4: return "4th";
            case 5: return "5th";
            case 6: return "6th";
            default: return place.ToString() + "th";
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
    
    // Method to set the display mode to final results
    public void SetFinalDisplayMode(bool isFinal)
    {
        isFinalCashDisplay = isFinal;
        dayCashPanel.SetActive(!isFinalCashDisplay);
        finalCashPanel.SetActive(isFinalCashDisplay);
        
        // Reset scale and fade for results display
        transform.localScale = _originalScale;
        if (activeGroup != null)
        {
            activeGroup.alpha = 1f;
        }
        
        // Set the status background to white when in final display mode
        if (statusColorBg != null)
        {
            if (isFinal)
            {
                statusColorBg.DOColor(Color.white, 0.5f);
            }
        }
    }
}
