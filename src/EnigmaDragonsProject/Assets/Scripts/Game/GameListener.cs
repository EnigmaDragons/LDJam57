using UnityEngine;

public class GameListener : MonoBehaviour
{
  private void OnEnable()
  {
    Message.Subscribe<ShowDieRoll>(OnShowDieRoll, this);
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
}
