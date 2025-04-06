using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActivePlayerTokenController : MonoBehaviour
{
    [SerializeField] private GameObject targetObj;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutQuint;
    
    private List<Transform> _playerTargets = new List<Transform>();
    private int _currentPlayerIndex = -1;
    
    private void Awake()
    {
        if (targetObj == null)
        {
            Debug.LogError("Target object not assigned in ActivePlayerTokenController");
            return;
        }
        
        // Find all children with the ActivePlayerTarget tag
        foreach (Transform child in targetObj.transform)
        {
            if (child.CompareTag("ActivePlayerTarget"))
            {
                _playerTargets.Add(child);
            }
        }
        
        if (_playerTargets.Count == 0)
        {
            Debug.LogWarning("No ActivePlayerTarget objects found in the target object's children");
        }
        
        // Subscribe to game state changes
        CurrentGameState.Subscribe(HandleGameStateChanged, this);
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
        
        // Only animate if the player index has changed
        if (newPlayerIndex != _currentPlayerIndex && newPlayerIndex < _playerTargets.Count)
        {
            _currentPlayerIndex = newPlayerIndex;
            AnimateToTarget(_playerTargets[_currentPlayerIndex]);
        }
    }
    
    private void AnimateToTarget(Transform target)
    {
        // Animate this object to the target position
        transform.DOMove(target.position, animationDuration)
            .SetEase(animationEase);
    }
}