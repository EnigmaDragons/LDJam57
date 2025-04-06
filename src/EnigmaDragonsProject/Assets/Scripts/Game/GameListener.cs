using UnityEngine;

public class GameListener : MonoBehaviour
{
    [SerializeField] private DayNegotiation day;
    
    private void OnEnable()
    {
        Message.Subscribe<ShowDieRoll>(OnShowDieRoll, this);
        Message.Subscribe<NotifyPlayerSelectedAction>(OnPlayerActionSelected, this);
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
}
