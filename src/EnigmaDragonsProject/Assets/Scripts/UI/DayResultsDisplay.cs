using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayResultsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TextMeshProUGUI dayTitle;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private Button continueButton;
    [SerializeField] private DayNegotiation dayNegotiation;
    
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
        // Show the results panel
        if (resultsPanel != null)
            resultsPanel.SetActive(true);
            
        // Set day title
        if (dayTitle != null)
            dayTitle.text = $"Day {msg.Day} Results";
            
        // Display player results
        if (resultsText != null)
        {
            string results = "";
            
            foreach (var player in msg.PlayerStates)
            {
                string playerType = player.Player.PlayerType == PlayerType.Human ? "Player" : "AI";
                results += $"{player.Player.Character.Name} ({playerType}): {player.BankedCash} coins\n";
            }
            
            resultsText.text = results;
        }
    }
    
    private void OnContinueClicked()
    {
        // Hide the results panel
        if (resultsPanel != null)
            resultsPanel.SetActive(false);
            
        // Advance to the next day
        if (dayNegotiation != null)
            dayNegotiation.AdvanceToNextDay();
    }
} 