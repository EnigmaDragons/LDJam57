
public class GetPlayerActionRequested
{
    public PlayerState Player { get; }

    public GetPlayerActionRequested(PlayerState player)
    {
        Player = player;
    }
}
