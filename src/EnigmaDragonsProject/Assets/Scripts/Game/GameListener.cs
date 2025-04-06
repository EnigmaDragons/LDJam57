using UnityEngine;
using System.Collections;
using System;

public class GameListener : MonoBehaviour
{
    [SerializeField] private DayNegotiation day;
    [SerializeField] private float aiDecisionTimeSeconds = 0.8f;
    
    private void OnEnable()
    {
        Message.Subscribe<ShowDieRoll>(OnShowDieRoll, this);
        Message.Subscribe<NotifyPlayerSelectedAction>(OnPlayerActionSelected, this);
        Message.Subscribe<ReadyForPlayerSelection>(OnReadyForPlayerSelection, this);
        Message.Subscribe<DayFinished>(OnDayFinished, this);
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
        yield return new WaitForSeconds(aiDecisionTimeSeconds);
        
        if (CurrentGameState.ReadOnly.IsDayFinished) {
          Debug.Log("Day finished, skipping AI decision");
          yield break;
        }

        var aiAction = aiPlayer.Ai.SelectAction(CurrentGameState.ReadOnly, aiPlayer);
        Message.Publish(new NotifyPlayerSelectedAction(aiPlayer, aiAction));
    }

    private void OnDayFinished(DayFinished msg)
    {
        if (!CurrentGameState.ReadOnly.IsDayFinished)
          CurrentGameState.UpdateState(gs => gs.IsDayFinished = true);

        Debug.Log($"Day {msg.Day} Finished with {msg.PlayerStates.Length} players!");
        
        Message.Publish(new ShowDayResults(msg.Day, msg.PlayerStates));
        
        // Player must click a button that calls day.AdvanceToNextDay() to continue
        // This is already set up in DayNegotiation.ProcessDayEndStep() > DayEndStep.ShowResults
    }
}
