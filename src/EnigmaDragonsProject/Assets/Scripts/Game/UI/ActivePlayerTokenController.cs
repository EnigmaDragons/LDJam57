using System.Collections.Generic;
using UnityEngine;

public class ActivePlayerTokenController : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;
    
    private List<Transform> _playerTargets = new List<Transform>();
    private int _currentPlayerIndex = -1;
    private RectTransform _rectTransform;
    
    private void Awake()
    {
        if (targetObj == null)
        {
            Debug.LogError("Target object not assigned in ActivePlayerTokenController");
            return;
        }
        
        _rectTransform = GetComponent<RectTransform>();
        
        // Find all children with the ActivePlayerTarget tag recursively
        FindTargetsRecursively(targetObj.transform);
        
        if (_playerTargets.Count == 0)
        {
            Debug.LogError("No ActivePlayerTarget objects found in the target object's hierarchy");
            return;
        }
        
        // Subscribe to game state changes
        CurrentGameState.Subscribe(HandleGameStateChanged, this);
    }
    
    private void FindTargetsRecursively(Transform parent)
    {
        // Check the current transform
        if (parent.CompareTag("ActivePlayerTarget"))
        {
            _playerTargets.Add(parent);
        }
        
        // Check all children recursively
        foreach (Transform child in parent)
        {
            FindTargetsRecursively(child);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        CurrentGameState.Unsubscribe(this);
    }
    
    private void HandleGameStateChanged(GameStateChanged gs)
    {
        if (CurrentGameState.ReadOnly == null || _playerTargets.Count == 0)
            return;
            
        int newPlayerIndex = CurrentGameState.ReadOnly.PlayerTurnIndex;
        
        // Only move if the player index has changed
        if (newPlayerIndex != _currentPlayerIndex && newPlayerIndex < _playerTargets.Count)
        {
            _currentPlayerIndex = newPlayerIndex;
            
            if (_currentPlayerIndex == -1)
            {
                // Unparent and hide the token
                transform.SetParent(null);
                gameObject.SetActive(false);
            }
            else
            {
                // Move to target, then make visible
                MoveToTarget(_playerTargets[_currentPlayerIndex]);
                gameObject.SetActive(true);
            }
        }
    }
    
    private void MoveToTarget(Transform target)
    {
        // Parent this object under the target and reset position
        transform.SetParent(target);
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }
}