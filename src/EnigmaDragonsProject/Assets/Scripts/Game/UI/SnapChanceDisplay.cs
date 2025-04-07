using UnityEngine;
using TMPro;

public class SnapChanceDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI snapChanceText;
    
    [Header("Display Settings")]
    [SerializeField] private string format = "SNAP CHANCE: {0}%";
    [SerializeField] private Color lowRiskColor = new Color(0.2f, 0.8f, 0.2f);    // Green
    [SerializeField] private Color mediumRiskColor = new Color(0.9f, 0.9f, 0.2f); // Yellow
    [SerializeField] private Color highRiskColor = new Color(0.9f, 0.3f, 0.2f);   // Red
    
    [Header("Risk Thresholds")]
    [SerializeField] private float mediumRiskThreshold = 15f;  // 15% and above is medium risk
    [SerializeField] private float highRiskThreshold = 30f;    // 30% and above is high risk
    
    private GameState _lastKnownState;
    
    private void Start()
    {
        // Subscribe to game state changes
        CurrentGameState.Subscribe(OnGameStateChanged, this);
        
        // Initialize with current state if available
        if (CurrentGameState.ReadOnly != null)
        {
            UpdateSnapChanceDisplay(CurrentGameState.ReadOnly);
        }
        else
        {
            SetDisplayVisible(false);
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
        
        _lastKnownState = evt.State;
        UpdateSnapChanceDisplay(evt.State);
    }
    
    public void UpdateSnapChanceDisplay(GameState gameState)
    {
        if (gameState == null || snapChanceText == null)
        {
            SetDisplayVisible(false);
            return;
        }
        
        SetDisplayVisible(true);
        
        // Get current deck
        Deck currentDeck = gameState.CurrentDeck;
        if (currentDeck == null || currentDeck.Count == 0)
        {
            snapChanceText.text = string.Format(format, "0");
            snapChanceText.color = lowRiskColor;
            return;
        }
        
        // Calculate snap chance percentage
        int totalCards = currentDeck.Count;
        int snapCards = currentDeck.SnapCount;
        float snapChance = (float)snapCards / totalCards * 100f;
        
        // Format text
        snapChanceText.text = string.Format(format, snapChance.ToString("F1"));
        
        // Set color based on risk level
        if (snapChance >= highRiskThreshold)
        {
            snapChanceText.color = highRiskColor;
        }
        else if (snapChance >= mediumRiskThreshold)
        {
            snapChanceText.color = mediumRiskColor;
        }
        else
        {
            snapChanceText.color = lowRiskColor;
        }
        
        // Log for debugging
        Debug.Log($"Snap Chance Updated: {snapChance:F1}% ({snapCards} snaps in {totalCards} cards)");
    }
    
    private void SetDisplayVisible(bool isVisible)
    {
        if (snapChanceText != null)
            snapChanceText.gameObject.SetActive(isVisible);
    }
    
    public void ForceRefresh()
    {
        if (_lastKnownState != null)
        {
            UpdateSnapChanceDisplay(_lastKnownState);
        }
    }
} 