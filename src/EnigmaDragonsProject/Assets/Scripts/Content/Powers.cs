using UnityEngine;

public class NoPower : CharacterPower
{
    public bool IsImplemented => false;
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
    public bool IsImplemented => true;
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
        CurrentGameState.UpdateState(gs =>
        {
            context.UsingPlayer.ChangeCashFromEffect(_amount);
            context.UsingPlayer.RecordPowerUsed();
        });
    }
}

public class IgnoreOneSnapCardEver : CharacterPower
{
    public bool IsImplemented => true;
    public bool IsAvailable { get; private set; }
    public PowerType PowerType => PowerType.DiscardSnapAfterDrawn;
    
    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() => IsAvailable = true;
    public void Apply(PowerContext context)
    {
        IsAvailable = false;
        Message.Publish(new ShowCharacterPowerExplanation($"{context.UsingPlayer.Player.Character.Name} used their power to ignore the snap.", context.UsingPlayer));
        CurrentGameState.UpdateState(gs =>
        {
            context.UsingPlayer.RecordPowerUsed();
        });
    }
}

public class BankInterestPower : CharacterPower
{
    public bool IsImplemented => true;
    public bool IsAvailable { get; private set; }
    public PowerType PowerType => PowerType.AutoStartOfDay;
    
    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() => IsAvailable = true;
    public void Apply(PowerContext context)
    {
        IsAvailable = false;
        var interestAmount = Mathf.CeilToInt(context.UsingPlayer.BankedCash * 0.10f);
        Message.Publish(new ShowCharacterPowerExplanation($"{context.UsingPlayer.Player.Character.Name} gained ${interestAmount} interest.", context.UsingPlayer));
        CurrentGameState.UpdateState(gs =>
        {
            context.UsingPlayer.AddDirectlyToBank(interestAmount);
            context.UsingPlayer.RecordPowerUsed();
        });
    }
}
