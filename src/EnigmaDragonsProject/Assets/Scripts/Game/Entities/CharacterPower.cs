
public enum CharacterPowerFrequency
{
  OncePerDay,
  OnceEver
}

public enum CharacterPowerActivationType
{ 
  Automatic,
  ReactiveChoice,
  ActivateOnDemand,
}

public interface CharacterPower 
{
  public bool IsAvailable { get; }
  public CharacterPowerActivationType ActivationType { get; }
  public void NotifyNewDayStarted();
  public void Apply(PowerContext context);
}

public class PowerContext
{
  public PlayerState UsingPlayer { get; set; }
  public GameState GameState { get; set; }
}
