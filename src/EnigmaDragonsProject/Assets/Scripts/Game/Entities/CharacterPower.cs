public enum PowerType
{
  AutoStartOfDay = 0,
  AutoStartOfWeek = 1,
  DiscardSnapAfterDrawn = 2,
  NoMoodEscalation = 3,
  SeeSnapOdds = 4,
  AfterBankPower = 5,
  AfterDrawCardSelectedBeforeDraw = 6,
  OfferInverter = 7,
  DrawTwoCards = 8,
}

public interface CharacterPower 
{
  public bool IsImplemented { get; }
  public bool IsAvailable { get; }
  public PowerType PowerType { get; }
  public void NotifyNewGameStarted();
  public void NotifyNewDayStarted();
  public void Apply(PowerContext context);
}

public class PowerContext
{
  public PlayerState UsingPlayer { get; }
  public GameState GameState { get; }

  public PowerContext(GameState gs, PlayerState usingPlayer)
  {
    GameState = gs;
    UsingPlayer = usingPlayer;
  }
}
