using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossMoodBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image moodFillBar;
    [SerializeField] private TextMeshProUGUI moodLabelText;
    [SerializeField] private GameObject moodChangeFeedback;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Mood Color Settings")]
    [SerializeField] private Color calmColor = new Color(0.3f, 0.8f, 0.3f); // Green
    [SerializeField] private Color annoyedColor = new Color(0.9f, 0.9f, 0.2f); // Yellow
    [SerializeField] private Color angryColor = new Color(0.9f, 0.3f, 0.2f); // Red
    
    [Header("Mood Thresholds")]
    [SerializeField] private int annoyedThreshold = 20;
    [SerializeField] private int angryThreshold = 40;
    
    [Header("Feedback Settings")]
    [SerializeField] private float feedbackDisplayTime = 3f;
    
    // The maximum mood value to consider for the fill bar (this is somewhat arbitrary)
    // We can consider 60 as the max since all bosses have keys up to 60 in their mood tables
    [SerializeField] private int maxMoodValue = 60;
    
    private GameState _lastKnownState;
    private int _previousMoodTier = 0;
    
    private void Start()
    {
        // Subscribe to game state changes
        CurrentGameState.Subscribe(OnGameStateChanged, this);
        
        // Subscribe to snaps added messages
        Message.Subscribe<SnapsAddedToDeck>(OnSnapsAdded, this);
        
        // Initialize with current state if available
        if (CurrentGameState.ReadOnly != null)
        {
            UpdateMoodBar(CurrentGameState.ReadOnly);
        }
        else
        {
            // Hide the UI if no state is available
            SetMoodBarVisible(false);
        }
        
        // Hide feedback initially
        if (moodChangeFeedback != null)
            moodChangeFeedback.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when this object is destroyed
        CurrentGameState.Unsubscribe(this);
        Message.Unsubscribe(this);
    }
    
    private void OnGameStateChanged(GameStateChanged evt)
    {
        if (evt == null || evt.State == null)
            return;
        
        _lastKnownState = evt.State;
        UpdateMoodBar(evt.State);
    }
    
    private void OnSnapsAdded(SnapsAddedToDeck msg)
    {
        if (msg == null || msg.SnapCount <= 0)
            return;
            
        // Show feedback when snaps are added
        ShowMoodChangeFeedback($"{msg.SnapCount} SNAP{(msg.SnapCount > 1 ? "S" : "")} ADDED TO DECK!");
    }
    
    public void UpdateMoodBar(GameState gameState)
    {
        if (gameState == null || gameState.BossState == null)
        {
            SetMoodBarVisible(false);
            return;
        }
        
        SetMoodBarVisible(true);
        
        BossState bossState = gameState.BossState;
        int currentMood = bossState.CurrentMood;
        int currentMoodTier = bossState.CurrentMoodTier;
        
        // Check if the mood tier increased
        if (currentMoodTier > _previousMoodTier && _previousMoodTier > 0)
        {
            // Boss is getting angrier!
            ShowMoodChangeFeedback("BOSS MOOD ESCALATED!");
        }
        
        _previousMoodTier = currentMoodTier;
        
        // Calculate the fill amount (0 to 1)
        float fillAmount = Mathf.Clamp01((float)currentMood / maxMoodValue);
        moodFillBar.fillAmount = fillAmount;
        
        // Set the color based on mood level
        if (currentMood >= angryThreshold)
        {
            moodFillBar.color = angryColor;
            moodLabelText.text = "FURIOUS";
        }
        else if (currentMood >= annoyedThreshold)
        {
            moodFillBar.color = annoyedColor;
            moodLabelText.text = "ANNOYED";
        }
        else
        {
            moodFillBar.color = calmColor;
            moodLabelText.text = "HAPPY";
        }
        
        // Log for debugging
        Debug.Log($"Boss Mood Updated: {bossState.Boss.Name}, Mood: {currentMood}, MoodTier: {currentMoodTier}");
    }
    
    private void ShowMoodChangeFeedback(string message)
    {
        if (moodChangeFeedback == null || feedbackText == null)
            return;
            
        // Set the feedback text
        feedbackText.text = message;
        
        // Show the feedback
        moodChangeFeedback.SetActive(true);
        
        // Auto-hide after delay
        StartCoroutine(HideFeedbackAfterDelay());
    }
    
    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDisplayTime);
        
        if (moodChangeFeedback != null)
            moodChangeFeedback.SetActive(false);
    }
    
    private void SetMoodBarVisible(bool isVisible)
    {
        if (moodFillBar != null)
            moodFillBar.gameObject.SetActive(isVisible);
            
        if (moodLabelText != null)
            moodLabelText.gameObject.SetActive(isVisible);
    }
    
    public void ForceRefresh()
    {
        if (_lastKnownState != null)
        {
            UpdateMoodBar(_lastKnownState);
        }
    }
} 