using UnityEngine;

/// <summary>
/// A very simple script that moves a UI element upward at a consistent rate.
/// Attach this to any UI element with a RectTransform to make it scroll upward.
/// </summary>
public class SimpleScrollUpward : MonoBehaviour
{
    [Tooltip("Speed of upward movement in pixels per second")]
    [SerializeField] private float scrollSpeed = 50f;

    private RectTransform _rectTransform;
    private Vector2 _startPosition;
    private float _currentOffset = 0f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        if (_rectTransform == null)
        {
            Debug.LogError("SimpleScrollUpward requires a RectTransform component!");
            enabled = false;
            return;
        }
        
        // Store the initial position
        _startPosition = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        // Calculate the new offset based on time and speed
        _currentOffset += scrollSpeed * Time.deltaTime;
        
        // Update the position (maintaining the original X position)
        _rectTransform.anchoredPosition = new Vector2(
            _startPosition.x, 
            _startPosition.y + _currentOffset);
    }
} 