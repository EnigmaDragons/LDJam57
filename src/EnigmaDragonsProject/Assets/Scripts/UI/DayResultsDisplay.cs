using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;
using System.Collections;

public class DayResultsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject darken;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private PlayerUi[] playerUis;
    [SerializeField] private TextMeshProUGUI dayTitle;
    [SerializeField] private Button continueButton;
    [SerializeField] private float panelEntranceTime = 0.5f;
    [SerializeField] private float playerPopDelay = 0.2f;

    private void Start()
    {
        resultsPanel.SetActive(false);
        darken.SetActive(false);

        foreach (var playerUI in playerUis)
        {
            playerUI.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Message.Subscribe<ShowDayResults>(OnShowDayResults, this);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
        
        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueClicked);
    }
    
    private void OnShowDayResults(ShowDayResults msg)
    {
        Debug.Log($"OnShowDayResults called for {msg.Day}");
        
        // Check if results panel is already active, which might indicate a duplicate call
        if (resultsPanel.activeSelf)
        {
            Debug.LogWarning("Results panel is already active. Ignoring duplicate call.");
            return;
        }
        
        dayTitle.text = $"{msg.Day} Results";
        
        // Hide all player UIs initially
        foreach (var playerUI in playerUis)
        {
            if (playerUI != null)
                playerUI.gameObject.SetActive(false);
        }

        // Sort player states by total cash (decreasing), then banked cash (decreasing)
        var sortedPlayers = msg.PlayerStates.OrderByDescending(p => p.BankedCash + p.CurrentRoundCash)
                                        .ThenByDescending(p => p.BankedCash)
                                        .ToArray();

        // Initialize player UIs with the sorted player states
        for (int i = 0; i < sortedPlayers.Length && i < playerUis.Length; i++)
        {
            if (playerUis[i] != null)
            {
                // Initialize each player UI with their respective state
                playerUis[i].InitWithPlayerState(sortedPlayers[i]);
                
                // Set to final display mode
                playerUis[i].SetFinalDisplayMode(true);
            }
        }
        
        // Animate the results panel entrance and player UIs
        StartCoroutine(AnimateResultsEntrance());
    }

    private IEnumerator AnimateResultsEntrance()
    {
        darken.SetActive(true);
        
        // Show the results panel but set it initially off-screen
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
            resultsPanel.transform.localPosition = new Vector3(0, -2000, 0);

            // Animate the panel sliding up
            resultsPanel.transform.DOLocalMoveY(0, panelEntranceTime).SetEase(Ease.OutBack);
            
            // Wait for panel to finish animating in
            yield return new WaitForSeconds(panelEntranceTime);
            
            // Now animate each player UI one by one
            for (int i = 0; i < playerUis.Length; i++)
            {
                if (playerUis[i] != null)
                {
                    // Make player UI visible with normal scale
                    playerUis[i].gameObject.SetActive(true);
                    playerUis[i].transform.localScale = Vector3.one; // Set scale to 1
                    
                    // Add a pop animation
                    playerUis[i].transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 2, 0.5f);
                    
                    // Wait before showing the next player UI
                    yield return new WaitForSeconds(playerPopDelay);
                }
            }
        }
    }
    
    private void OnContinueClicked()
    {
        // Hide the results panel
        if (resultsPanel != null)
            resultsPanel.SetActive(false);
            
        // Reset and hide all player UIs
        foreach (var playerUI in playerUis)
        {
            if (playerUI != null)
            {
                playerUI.SetFinalDisplayMode(false);
                playerUI.gameObject.SetActive(false);
            }
        }
    }
} 