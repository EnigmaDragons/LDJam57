using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChoicePresenter : OnMessage<GetPlayerActionRequested>
{
    [SerializeField] private GameObject uiParent;
    [SerializeField] private Button bankButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private TextMeshProUGUI dayCashLabel;

    private PlayerState _currentPlayer;
    
    private void Awake()
    {
        uiParent.SetActive(false);
        bankButton.onClick.AddListener(OnBankButtonPressed);
        drawButton.onClick.AddListener(OnDrawCardButtonPressed);
    }
    
    protected override void Execute(GetPlayerActionRequested msg)
    {
        if (msg.Player.Player.PlayerType != PlayerType.Human)
            return;

        _currentPlayer = msg.Player;
        dayCashLabel.text = $"Current Cash Value: ${_currentPlayer.CurrentRoundCash}";
        uiParent.SetActive(true);
    }

    private void OnBankButtonPressed()
    {
        Message.Publish(new NotifyPlayerSelectedAction(_currentPlayer, ActionType.BankCash));
        _currentPlayer = null;
        uiParent.SetActive(false);
    }

    private void OnDrawCardButtonPressed()
    {
        Message.Publish(new NotifyPlayerSelectedAction(_currentPlayer, ActionType.DrawCard));
        _currentPlayer = null;
        uiParent.SetActive(false);
    }
}
