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
    [SerializeField] private Color waryColor = new Color(0.9f, 0.9f, 0.2f); // Yellow
    [SerializeField] private Color budgetConsciousColor = new Color(0.9f, 0.3f, 0.2f); // Red
    
    [Header("Mood Thresholds")]
    [SerializeField] private int waryThreshold = 20;
    [SerializeField] private int budgetConsciousThreshold = 40;
    
    [Header("Gradient Animation")]
    [SerializeField] private float gradientAnimSpeed = 0.5f;
    [SerializeField] private float gradientTiling = 2f;
    [SerializeField] private float gradientContrast = 0.2f; // How strong the gradient effect is (0-1)
    
    [Header("Feedback Settings")]
    [SerializeField] private float feedbackDisplayTime = 3f;
    
    // The maximum mood value to consider for the fill bar (this is somewhat arbitrary)
    // We can consider 60 as the max since all bosses have keys up to 60 in their mood tables
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
        
        // Smoothly interpolate between colors based on mood level
        float t1 = Mathf.InverseLerp(0, waryThreshold, currentMood);
        float t2 = Mathf.InverseLerp(waryThreshold, budgetConsciousThreshold, currentMood);
        
        Color barColor;
        string moodLabel;
        
        if (currentMood >= budgetConsciousThreshold)
        {
            // Interpolate between wary and budget-conscious
            float t = Mathf.InverseLerp(budgetConsciousThreshold, maxMoodValue, currentMood);
            barColor = Color.Lerp(waryColor, budgetConsciousColor, t);
            moodLabel = "BUDGET-CONSCIOUS";
        }
        else if (currentMood >= waryThreshold)
        {
            // Interpolate between calm and wary
            float t = Mathf.InverseLerp(waryThreshold, budgetConsciousThreshold, currentMood);
            barColor = Color.Lerp(calmColor, waryColor, t);
            moodLabel = "WARY";
        }
        else
        {
            // Interpolate from happy to almost-wary based on how close to wary threshold
            float t = Mathf.InverseLerp(0, waryThreshold, currentMood);
            barColor = Color.Lerp(calmColor, Color.Lerp(calmColor, waryColor, 0.5f), t);
            moodLabel = "HAPPY";
        }
        
        // Apply the base color to the material
        moodFillBar.color = barColor;
        
        // Apply mood label
        moodLabelText.text = moodLabel;
        
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