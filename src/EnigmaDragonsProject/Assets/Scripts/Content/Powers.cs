public class NoPower : CharacterPower
{
    public bool IsAvailable => false;
    public PowerType PowerType => PowerType.AutoStartOfWeek;
    public void NotifyNewGameStarted() { }
    public void NotifyNewDayStarted() {}
    public void Apply(PowerContext context) {}
}

public class GainMoneyAtStartOfDayPower : CharacterPower
{
    private readonly int _amount;
    
    public GainMoneyAtStartOfDayPower(int amount)
    {
        _amount = amount;
    }
    
    public bool IsAvailable { get; private set; } = true;
    public PowerType PowerType => PowerType.AutoStartOfDay;
    
    public void NotifyNewDayStarted()
    {
        IsAvailable = true;
    }
    
    public void NotifyNewGameStarted()
    {
        IsAvailable = true;
    }
    
    public void Apply(PowerContext context)
    {
        IsAvailable = false;
        Message.Publish(new ShowCharacterPowerExplanation($"{context.UsingPlayer.Player.Character.Name} used their power to gain ${_amount}", context.UsingPlayer));
        CurrentGameState.UpdateState(gs => context.UsingPlayer.ChangeCashFromEffect(_amount));
    }
}

public class IgnoreOneSnapCardEver : CharacterPower
{
    public bool IsAvailable { get; private set; }
    public PowerType PowerType => PowerType.DiscardSnapAfterDrawn;
    
    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() => IsAvailable = true;
    public void Apply(PowerContext context)
    {
        IsAvailable = false;
        Message.Publish(new ShowCharacterPowerExplanation($"{context.UsingPlayer.Player.Character.Name} used their power to ignore the snap.", context.UsingPlayer));
    }
}
