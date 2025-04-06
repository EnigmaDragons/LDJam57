using UnityEngine;
using System.Collections;

public class GameListener : MonoBehaviour
{
    [SerializeField] private DayNegotiation day;
    
    private void OnEnable()
    {
        Message.Subscribe<ShowDieRoll>(OnShowDieRoll, this);
        Message.Subscribe<NotifyPlayerSelectedAction>(OnPlayerActionSelected, this);
        Message.Subscribe<ReadyForPlayerSelection>(OnReadyForPlayerSelection, this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    
    private void OnShowDieRoll(ShowDieRoll msg)
    {
        Debug.Log($"GameListener received ShowDieRoll message: {msg.Result}");
        Message.Publish(new Finished<ShowDieRoll> { Message = msg });
    }

    private void OnPlayerActionSelected(NotifyPlayerSelectedAction msg)
    {
        if (msg.Action == ActionType.DrawCard)
          day.DrawCard();
        else 
          day.AcceptOffer();
    }

    private void OnReadyForPlayerSelection(ReadyForPlayerSelection msg)
    {
        if (msg.Player.Player.PlayerType == PlayerType.AI)
        {
            StartCoroutine(MakeAIDecisionAfterDelay(msg.Player));
        }
    }

    private IEnumerator MakeAIDecisionAfterDelay(PlayerState aiPlayer)
    {
        yield return new WaitForSeconds(1.5f);
        
        var aiAction = aiPlayer.Ai.SelectAction(CurrentGameState.ReadOnly, aiPlayer);
        Message.Publish(new NotifyPlayerSelectedAction(aiPlayer, aiAction));
    }
}
