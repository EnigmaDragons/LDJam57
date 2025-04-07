using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using DG.Tweening;

public class FinalResultsScreen : MonoBehaviour
{
    [SerializeField] private bool activateOnStart = false;
    
    [Header("Main Components")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject confettiEffect;
    [SerializeField] private GameObject creditsPanel;
    
    [Header("Winner Display")]
    [SerializeField] private Image winnerFaceImage;
    [SerializeField] private TextMeshProUGUI winnerNameText;
    [SerializeField] private TextMeshProUGUI playerResultText;
    
    [Header("Player Rankings")]
    [SerializeField] private PlayerUi[] playerRankingUis;
    
    [Header("Buttons")]
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button playAgainButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float panelAnimationTime = 0.6f;
    [SerializeField] private float elementAnimationDelay = 0.3f;
    [SerializeField] private float rankingDelay = 0.2f;
    [SerializeField] private float creditsDelay = 0.2f;
    
    private static GameState CreateSampleRandomGameState() {  
      var gs = new GameState();
      gs.PlayerStates = new PlayerState[6];
      for (int i = 0; i < 6; i++)
      {
          gs.PlayerStates[i] = new PlayerState(new Player(i, Characters.TabbyTom, i == 0 ? PlayerType.Human : PlayerType.AI), Random.Range(5, 201), new NoAi());
      }
      CurrentGameState.Init(gs);
      return gs;
    }

    private void Start()
    {
        // Hide everything at start
        mainPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        HideAllPlayerRankings();
        
        // Hide buttons initially
        if (tryAgainButton != null) tryAgainButton.gameObject.SetActive(false);
        if (playAgainButton != null) playAgainButton.gameObject.SetActive(false);

        if (activateOnStart)
        {
            var fromState = CurrentGameState.ReadOnly;
            var gameState = fromState != null && fromState.IsGameOver 
              ? fromState 
              : CreateSampleRandomGameState();
            StartCoroutine(ShowFinalResults(gameState));
        }
    }
    
    private void OnEnable()
    {
        Message.Subscribe<GameOver>(OnGameOver, this);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    
    private void HideAllPlayerRankings()
    {
        foreach (var playerUi in playerRankingUis)
        {
            if (playerUi != null)
                playerUi.gameObject.SetActive(false);
        }
    }
    
    private void OnGameOver(GameOver msg)
    {
        // Set up final results and then show the screen
        StartCoroutine(ShowFinalResults(msg.GameState));
    }
    
    private IEnumerator ShowFinalResults(GameState gameState)
    {
        // Sort player states by total cash (decreasing), then banked cash (decreasing)
        var sortedPlayers = gameState.PlayerStates
            .OrderByDescending(p => p.BankedCash)
            .ToArray();
        
        // Get the winner (first in the sorted list)
        PlayerState winner = sortedPlayers[0];
        
        // Find the human player and their rank
        int humanPlayerRank = -1;
        PlayerState humanPlayer = null;
        
        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            if (sortedPlayers[i].Player.PlayerType == PlayerType.Human)
            {
                humanPlayer = sortedPlayers[i];
                humanPlayerRank = i + 1; // +1 because ranks are 1-based (1st, 2nd, etc.)
                break;
            }
        }
        
        // Show the main panel with animation
        mainPanel.SetActive(true);
        mainPanel.transform.localScale = Vector3.zero;
        winnerFaceImage.color = new Color(0, 0, 0, 0);
        winnerNameText.text = "";
        playerResultText.text = "";
        yield return mainPanel.transform.DOScale(Vector3.one, panelAnimationTime).SetEase(Ease.OutBack).WaitForCompletion();
        
        // Play confetti if we have it
        if (confettiEffect != null)
        {
            confettiEffect.SetActive(true);
        }
        else
        {
            Debug.Log("No confetti effect assigned to FinalResultsScreen");
        }
        
        // Show winner face
        if (winnerFaceImage != null && winner.Player.Character != null)
        {
            winnerFaceImage.color = Color.white;
            winnerFaceImage.sprite = winner.Player.Character.Face;
            winnerFaceImage.transform.localScale = Vector3.zero;
            yield return winnerFaceImage.transform.DOScale(Vector3.one, panelAnimationTime).SetEase(Ease.OutBack).WaitForCompletion();
        }
        
        // Show winner name text
        if (winnerNameText != null && winner.Player.Character != null)
        {
            winnerNameText.text = $"{winner.Player.Character.Name} Won!";
            winnerNameText.alpha = 0;
            yield return winnerNameText.DOFade(1, panelAnimationTime).WaitForCompletion();
        }
        
        // Show player result if human player didn't win
        if (humanPlayer != winner && playerResultText != null)
        {
            playerResultText.text = $"You got {GetOrdinalString(humanPlayerRank)} place";
            playerResultText.alpha = 0;
            yield return playerResultText.DOFade(1, panelAnimationTime).WaitForCompletion();
        }
        
        // Initialize player UIs with the sorted player states
        for (int i = 0; i < sortedPlayers.Length && i < playerRankingUis.Length; i++)
        {
            if (playerRankingUis[i] != null)
            {
                // Initialize each player UI with their respective state
                playerRankingUis[i].InitWithPlayerState(sortedPlayers[i]);
                playerRankingUis[i].SetFinalDisplayMode(true);
                
                // Animation
                playerRankingUis[i].gameObject.SetActive(true);
                playerRankingUis[i].transform.localScale = Vector3.zero;
                yield return playerRankingUis[i].transform.DOScale(Vector3.one, panelAnimationTime).SetEase(Ease.OutBack).WaitForCompletion();
                
                yield return new WaitForSeconds(rankingDelay);
            }
        }
        
        // Show the appropriate button based on whether the human player won
        if (humanPlayer == winner)
        {
            if (playAgainButton != null)
            {
                playAgainButton.gameObject.SetActive(true);
                playAgainButton.transform.localScale = Vector3.zero;
                yield return playAgainButton.transform.DOScale(Vector3.one, panelAnimationTime).SetEase(Ease.OutBack).WaitForCompletion();
            }
        }
        else
        {
            if (tryAgainButton != null)
            {
                tryAgainButton.gameObject.SetActive(true);
                tryAgainButton.transform.localScale = Vector3.zero;
                yield return tryAgainButton.transform.DOScale(Vector3.one, panelAnimationTime).SetEase(Ease.OutBack).WaitForCompletion();
            }
        }
        
        // Show credits last
        yield return new WaitForSeconds(creditsDelay);
        
        if (creditsPanel != null)
        {
            // Make sure credits panel is active but with the scroller not started yet
            creditsPanel.SetActive(true);
            
            // Animate credits from right side
            RectTransform creditsRect = creditsPanel.GetComponent<RectTransform>();
            if (creditsRect != null)
            {
                Vector2 originalPosition = creditsRect.anchoredPosition;
                creditsRect.anchoredPosition = new Vector2(Screen.width, originalPosition.y);
                yield return creditsRect.DOAnchorPosX(originalPosition.x, panelAnimationTime).SetEase(Ease.OutQuad).WaitForCompletion();
            }
        }
    }
    
    private string GetOrdinalString(int number)
    {
        switch (number)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            case 4: return "4th";
            case 5: return "5th";
            case 6: return "6th";
            default: return number + "th";
        }
    }
} 