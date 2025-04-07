public class NoPower : CharacterPower
{
    public bool IsAvailable => false;
    public PowerType PowerType => PowerType.AutoStartOfWeek;
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

    public void Apply(PowerContext context)
    {
        IsAvailable = false;
        CurrentGameState.UpdateState(gs => context.UsingPlayer.ChangeCashFromEffect(_amount));
    }
}
