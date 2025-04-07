using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BossMoodBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image moodFillBar;
    [SerializeField] private TextMeshProUGUI moodLabelText;
    [SerializeField] private GameObject moodChangeFeedback;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Mood Tiers")]
    [SerializeField] private List<MoodTierConfig> moodTiers = new List<MoodTierConfig>
    {
        new MoodTierConfig(0, "DELIGHTED", new Color(0.0f, 0.7f, 0.0f)),       // Dark Green
        new MoodTierConfig(5, "CONTENT", new Color(0.2f, 0.8f, 0.2f)),          // Green
        new MoodTierConfig(10, "SATISFIED", new Color(0.4f, 0.8f, 0.4f)),       // Light Green
        new MoodTierConfig(15, "ATTENTIVE", new Color(0.6f, 0.8f, 0.4f)),       // Yellow-Green
        new MoodTierConfig(20, "CURIOUS", new Color(0.8f, 0.8f, 0.2f)),         // Yellow
        new MoodTierConfig(25, "NEUTRAL", new Color(0.9f, 0.8f, 0.2f)),         // Dark Yellow
        new MoodTierConfig(30, "CONCERNED", new Color(0.9f, 0.7f, 0.1f)),       // Orange-Yellow
        new MoodTierConfig(35, "SKEPTICAL", new Color(0.9f, 0.6f, 0.1f)),       // Orange
        new MoodTierConfig(40, "WARY", new Color(0.9f, 0.5f, 0.1f)),            // Dark Orange
        new MoodTierConfig(45, "IRRITATED", new Color(0.9f, 0.3f, 0.1f)),       // Red-Orange
        new MoodTierConfig(50, "AGITATED", new Color(0.9f, 0.2f, 0.1f)),        // Light Red
        new MoodTierConfig(55, "BUDGET-CONSCIOUS", new Color(0.9f, 0.1f, 0.1f)) // Deep Red
    };
    
    [Header("Gradient Animation")]
    [SerializeField] private float gradientAnimSpeed = 0.5f;
    [SerializeField] private float gradientTiling = 2f;
    [SerializeField] private float gradientContrast = 0.2f; // How strong the gradient effect is (0-1)
    
    [Header("Feedback Settings")]
    [SerializeField] private float feedbackDisplayTime = 3f;
    
    // The maximum mood value to consider for the fill bar (this should be aligned with highest tier)
    [SerializeField] private int maxMoodValue = 60;
    
    private GameState _lastKnownState;
    private int _previousMoodTier = 0;
    private Material _gradientMaterial;
    private float _gradientOffset = 0f;
    private static readonly int MainTexOffset = Shader.PropertyToID("_MainTex_ST");
    
    private void Start()
    {
        // Setup the animated gradient material
        SetupGradientMaterial();
        
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
            
        // Sort tiers by threshold to ensure they're in order
        moodTiers.Sort((a, b) => a.Threshold.CompareTo(b.Threshold));
    }

    private void Update()
    {
        // Animate the gradient
        AnimateGradient();
    }
    
    private void OnDestroy()
    {
        // Clean up material
        if (_gradientMaterial != null && Application.isPlaying)
            Destroy(_gradientMaterial);
            
        // Unsubscribe when this object is destroyed
        CurrentGameState.Unsubscribe(this);
        Message.Unsubscribe(this);
    }
    
    private void SetupGradientMaterial()
    {
        if (moodFillBar != null)
        {
            // Create a new material based on the current one
            _gradientMaterial = new Material(moodFillBar.material);
            
            // Set the tiling to create a gradient effect
            _gradientMaterial.SetVector(MainTexOffset, new Vector4(gradientTiling, 1, 0, 0));
            
            // Apply it to the image
            moodFillBar.material = _gradientMaterial;
        }
    }
    
    private void AnimateGradient()
    {
        if (_gradientMaterial == null)
            return;
            
        // Update the offset for animation
        _gradientOffset += Time.deltaTime * gradientAnimSpeed;
        if (_gradientOffset > 1f)
            _gradientOffset -= 1f;
            
        // Update the material
        Vector4 currentOffset = _gradientMaterial.GetVector(MainTexOffset);
        _gradientMaterial.SetVector(MainTexOffset, new Vector4(currentOffset.x, currentOffset.y, _gradientOffset, 0));
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
    
    // Find the mood tier for a given mood value
    private MoodTierConfig GetMoodTierForValue(int moodValue)
    {
        MoodTierConfig result = moodTiers[0]; // Default to lowest tier
        
        foreach (var tier in moodTiers)
        {
            if (moodValue >= tier.Threshold && tier.Threshold >= result.Threshold)
            {
                result = tier;
            }
        }
        
        return result;
    }
    
    // Find the next higher tier (for interpolation)
    private MoodTierConfig GetNextMoodTier(MoodTierConfig currentTier)
    {
        int currentIndex = moodTiers.IndexOf(currentTier);
        
        // If at last tier, return the same tier
        if (currentIndex >= moodTiers.Count - 1)
            return currentTier;
            
        return moodTiers[currentIndex + 1];
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
        
        // Get the appropriate tier config for the current mood
        MoodTierConfig currentTierConfig = GetMoodTierForValue(currentMood);
        MoodTierConfig nextTierConfig = GetNextMoodTier(currentTierConfig);
        
        // Interpolate color between current tier and next tier
        Color barColor;
        if (currentTierConfig != nextTierConfig)
        {
            float t = Mathf.InverseLerp(currentTierConfig.Threshold, nextTierConfig.Threshold, currentMood);
            barColor = Color.Lerp(currentTierConfig.Color, nextTierConfig.Color, t);
        }
        else
        {
            barColor = currentTierConfig.Color;
        }
        
        // Apply the base color to the material
        moodFillBar.color = barColor;
        
        // Apply mood label
        moodLabelText.text = currentTierConfig.Label;
        
        // Log for debugging
        Debug.Log($"Boss Mood Updated: {bossState.Boss.Name}, Mood: {currentMood}, MoodTier: {currentMoodTier}, Label: {currentTierConfig.Label}");
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

[System.Serializable]
public class MoodTierConfig
{
    public int Threshold;
    public string Label;
    public Color Color;
    
    public MoodTierConfig(int threshold, string label, Color color)
    {
        Threshold = threshold;
        Label = label;
        Color = color;
    }
} 