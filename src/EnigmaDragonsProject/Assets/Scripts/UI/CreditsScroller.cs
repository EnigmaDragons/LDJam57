using UnityEngine;
using TMPro;
using DG.Tweening;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float endPauseDelay = 3f;
    [SerializeField] private float initialOffscreenPosition = 1200f;
    [SerializeField] private float finalOffscreenPosition = -3000f;
    
    [Header("Auto Start")]
    [SerializeField] private bool autoStartScrolling = false;
    [SerializeField] private bool loopCredits = false;
    [SerializeField] private bool useDefaultCredits = true;
    
    [TextArea(20, 30)]
    [SerializeField] private string customCreditsContent;
    
    private RectTransform _textRectTransform;
    private bool _isScrolling = false;
    private Tween _scrollTween;
    
    private void Awake()
    {
        _textRectTransform = creditsText.GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        // Set credits text
        if (useDefaultCredits)
        {
            creditsText.text = CreditsContent.GetCreditsText();
        }
        else
        {
            creditsText.text = customCreditsContent;
        }
        
        // Initial position
        _textRectTransform.anchoredPosition = new Vector2(
            _textRectTransform.anchoredPosition.x, 
            initialOffscreenPosition);
        
        if (autoStartScrolling)
        {
            StartScrolling();
        }
    }
    
    private void OnDestroy()
    {
        if (_scrollTween != null)
        {
            _scrollTween.Kill();
        }
    }
    
    public void StartScrolling()
    {
        if (_isScrolling) return;
        
        _isScrolling = true;
        
        // Calculate the duration based on the text height and scroll speed
        float contentHeight = creditsText.preferredHeight;
        float totalScrollDistance = initialOffscreenPosition - finalOffscreenPosition;
        float scrollDuration = totalScrollDistance / scrollSpeed;
        
        // Start from the initial position
        _textRectTransform.anchoredPosition = new Vector2(
            _textRectTransform.anchoredPosition.x, 
            initialOffscreenPosition);
        
        // Create the scroll sequence
        Sequence scrollSequence = DOTween.Sequence();
        
        // Start delay
        scrollSequence.AppendInterval(startDelay);
        
        // Scroll animation
        _scrollTween = _textRectTransform.DOAnchorPosY(finalOffscreenPosition, scrollDuration)
            .SetEase(Ease.Linear);
        scrollSequence.Append(_scrollTween);
        
        // End delay
        scrollSequence.AppendInterval(endPauseDelay);
        
        // Handle looping
        if (loopCredits)
        {
            scrollSequence.OnComplete(() => {
                _isScrolling = false;
                StartScrolling();
            });
        }
        else
        {
            scrollSequence.OnComplete(() => {
                _isScrolling = false;
            });
        }
        
        scrollSequence.Play();
    }
    
    public void StopScrolling()
    {
        if (_scrollTween != null)
        {
            _scrollTween.Kill();
            _isScrolling = false;
        }
    }
} 