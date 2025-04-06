
public class NotifyPlayerSelectedAction
{
    public PlayerState Player { get; }
    public ActionType Action { get; }

    public NotifyPlayerSelectedAction(PlayerState player, ActionType action)
    {
        Player = player;
        Action = action;
    }
}
