using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ShowCardDrawOutcomePresenter : OnMessage<ShowCardDrawn>
{
    [Header("Card Animation Settings")]
    [SerializeField] private RectTransform cardContainer;
    [SerializeField] private float slideInDuration = 1.0f;
    [SerializeField] private float flipDuration = 1.5f;
    [SerializeField] private float showDuration = 3.0f;
    [SerializeField] private float fadeOutDuration = 0.8f;
    
    [Header("Card Visual Elements")]
    [SerializeField] private Image cardFront;
    [SerializeField] private Image cardBack;
    [SerializeField] private TextMeshProUGUI cardDescription;

    [Header("Card Starting Position")]
    [SerializeField] private Vector2 startPosition = new Vector2(1500, 0);
    [SerializeField] private Vector2 displayPosition = new Vector2(0, 0);
    
    private Vector3 _initialScale;
    private bool _isAnimating = false;
    private ShowCardDrawn _msg;
    
    private void Awake()
    {
        if (cardContainer == null)
        {
            Debug.LogError("Card container not assigned in ShowCardDrawOutcomePresenter");
            return;
        }
        
        // Store initial scale for reset
        _initialScale = cardContainer.localScale;
        
        // Initialize card in start position (off-screen)
        cardContainer.anchoredPosition = startPosition;
        cardContainer.gameObject.SetActive(false);
        
        // Make sure back is visible at start
        if (cardFront != null) cardFront.gameObject.SetActive(false);
        if (cardBack != null) cardBack.gameObject.SetActive(true);
    }

    protected override void Execute(ShowCardDrawn msg)
    {
        // Don't interrupt existing animations
        if (_isAnimating) return;
        
        // Get card info from the message
        var drawnCard = msg.Card;
        if (drawnCard == null)
        {
            Debug.LogError("Card is null in NotifyPlayerSelectedAction message");
            return;
        }
        
        // Start the card animation sequence
        _msg = msg;
        StartCoroutine(AnimateCardSequence(drawnCard));
    }
    
    private IEnumerator AnimateCardSequence(Card card)
    {
        _isAnimating = true;
        
        // Set up the card visuals (front side)
        if (cardDescription != null) cardDescription.text = card.Description;
        
        // Make sure card is visible but showing the back
        cardContainer.gameObject.SetActive(true);
        if (cardFront != null) cardFront.gameObject.SetActive(false);
        if (cardBack != null) cardBack.gameObject.SetActive(true);
        
        // Reset position and rotation
        cardContainer.anchoredPosition = startPosition;
        cardContainer.localRotation = Quaternion.identity;
        cardContainer.localScale = _initialScale;
        
        // Make card container fully opaque
        CanvasGroup canvasGroup = cardContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = cardContainer.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        
        // Slide in animation
        Tween slideInTween = cardContainer.DOAnchorPos(displayPosition, slideInDuration)
            .SetEase(Ease.OutQuint);
        
        yield return slideInTween.WaitForCompletion();
        
        // Small delay before flip
        yield return new WaitForSeconds(0.3f);
        
        // Flip animation
        Sequence flipSequence = DOTween.Sequence();
        
        // First half of the flip (scale X to 0)
        flipSequence.Append(cardContainer.DOScaleX(0, flipDuration / 2)
            .SetEase(Ease.InSine));
        
        // Switch the card face at the middle of the flip
        flipSequence.AppendCallback(() =>
        {
            if (cardFront != null) cardFront.gameObject.SetActive(true);
            if (cardBack != null) cardBack.gameObject.SetActive(false);
        });
        
        // Second half of the flip (scale X back to normal)
        flipSequence.Append(cardContainer.DOScaleX(_initialScale.x, flipDuration / 2)
            .SetEase(Ease.OutSine));
        
        yield return flipSequence.WaitForCompletion();
        
        // Wait while the card is displayed
        yield return new WaitForSeconds(showDuration);
        
        // Fade out animation
        Tween fadeOutTween = canvasGroup.DOFade(0, fadeOutDuration)
            .SetEase(Ease.InQuad);
        
        yield return fadeOutTween.WaitForCompletion();
        
        // Hide card container
        cardContainer.gameObject.SetActive(false);
        
        // Notify that card animation is complete, move to next turn
        Message.Publish(new Finished<ShowCardDrawn>{ Message = _msg });

        _msg = null;
        _isAnimating = false;
    }
}

