using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ShowCharacterPowerPresenter : OnMessage<ShowCharacterPowerExplanation>
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float displayDuration = 2.5f;
    [SerializeField] private float fadeDuration = 0.5f;
    
    private Coroutine _currentAnimation;
    
    private void Awake()
    {
        if (powerText == null)
            Debug.LogError("Power text not assigned in ShowCharacterPowerPresenter");
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
            
        // Start hidden
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    protected override void Execute(ShowCharacterPowerExplanation msg)
    {
        // Stop any existing animation
        if (_currentAnimation != null)
            StopCoroutine(_currentAnimation);
            
        // Start new animation
        _currentAnimation = StartCoroutine(ShowPowerMessage(msg));
    }
    
    private IEnumerator ShowPowerMessage(ShowCharacterPowerExplanation msg)
    {
        // Set the text
        powerText.text = msg.Explanation;
        
        // Fade in immediately
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, fadeDuration);
        }
        
        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, fadeDuration);
        }
        
        _currentAnimation = null;
    }
} 