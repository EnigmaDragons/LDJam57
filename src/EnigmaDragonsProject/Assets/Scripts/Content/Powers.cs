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
        CurrentGameState.UpdateState(gs => gs);
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
        if (interestAmount == 0)
            return;
        
        Message.Publish(new ShowCharacterPowerExplanation($"{context.UsingPlayer.Player.Character.Name} gained ${interestAmount} interest.", context.UsingPlayer));
        CurrentGameState.UpdateState(gs =>
        {
            context.UsingPlayer.AddDirectlyToBank(interestAmount);
        });
    }
}

public abstract class PassivePower : CharacterPower
{
    public bool IsImplemented => true;
    public bool IsAvailable { get; private set; } = true;
    public PowerType PowerType { get; private set; }

    public PassivePower(PowerType powerType)
    {
        PowerType = powerType;
    }

    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() => IsAvailable = true;
    public void Apply(PowerContext context) { }
}

public class NoMoodEscalationPower : PassivePower
{
    public NoMoodEscalationPower() : base(PowerType.NoMoodEscalation) { }
}

public class SeeSnapOddsPower : PassivePower
{
    public SeeSnapOddsPower() : base(PowerType.SeeSnapOdds) { }
}

public class SympathyBuddyPower : CharacterPower
{
    public bool IsImplemented => true;
    public bool IsAvailable => true;
    public PowerType PowerType => PowerType.AfterBankPower;
    
    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() {}
    
    public void Apply(PowerContext context) 
    { 
        // Check if player is not in first place and find the richest player in one pass
        bool isNotInFirstPlace = false;
        PlayerState richestPlayer = null;
        int currentPlayerBankedCash = context.UsingPlayer.BankedCash;
        int highestBankedCash = currentPlayerBankedCash;
        
        foreach (var playerState in context.GameState.PlayerStates)
        {
            // Skip comparing with self
            if (playerState == context.UsingPlayer)
                continue;
                
            // Track if we're not in first place
            if (playerState.BankedCash > currentPlayerBankedCash)
                isNotInFirstPlace = true;
                
            // Track the richest player
            if (playerState.BankedCash > highestBankedCash)
            {
                highestBankedCash = playerState.BankedCash;
                richestPlayer = playerState;
            }
        }
        
        if (isNotInFirstPlace)
        {
            string richestPlayerName = richestPlayer != null 
                ? richestPlayer.Player.Character.Name 
                : "another player";
                
            Message.Publish(new ShowCharacterPowerExplanation($"{context.UsingPlayer.Player.Character.Name} gained $10 by buddying up to {richestPlayerName}.", context.UsingPlayer));
            CurrentGameState.UpdateState(gs =>
            {
                context.UsingPlayer.AddDirectlyToBank(10);
            });
        }
    }
}

public class SabotageNegotiationPower : CharacterPower
{
    public bool IsImplemented => true;
    public bool IsAvailable => true;
    public PowerType PowerType => PowerType.AfterBankPower;

    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() { }
        
    public int SnapsToAdd = 7;

    public void Apply(PowerContext ctx)
    {
        Debug.Log($"Sabotage Negotiations power activated! Adding {SnapsToAdd} snap(s) to the deck");
        CurrentGameState.UpdateState(gs =>
        {
            // Create and add snap cards to the deck
            for (int i = 0; i < SnapsToAdd; i++)
            {
                gs.CurrentDeck.ShuffleCardIn(new SnapCard());
            }
        });

        // Notify of new snaps
        Message.Publish(new SnapsAddedToDeck(SnapsToAdd));
        
        // Play deck shuffled sound
        Message.Publish(new PlayUiSound(SoundType.DeckShuffledShort));
        
        // Show power usage message
        Message.Publish(new ShowCharacterPowerExplanation($"After {ctx.UsingPlayer.Player.Character.Name} left, {ctx.GameState.BossState.Boss.Name} added {SnapsToAdd} Snap cards to the deck!", ctx.UsingPlayer));
    }
}     

public class BrilliantIdeaPower : CharacterPower
{
    public bool IsImplemented => true;
    public bool IsAvailable => true;
    public PowerType PowerType => PowerType.AfterDrawCardSelectedBeforeDraw;

    public void NotifyNewDayStarted() { }
    public void NotifyNewGameStarted() { }
    
    public void Apply(PowerContext ctx)
    {
      Debug.Log("Brilliant Idea power activated! Adding a +$20 card to the deck");
      
      // Create a high value card (+$20)
      var bonusCard = new OfferCard(20, 0);
      
      // Shuffle it into the deck
      CurrentGameState.UpdateState(gs =>
      {
          gs.CurrentDeck.ShuffleCardIn(bonusCard);
      });
      
      // Play deck shuffled sound
      Message.Publish(new PlayUiSound(SoundType.DeckShuffledShort));

      Message.Publish(new ShowCharacterPowerExplanation($"{ctx.UsingPlayer.Player.Character.Name} had a brilliant idea! A +$20 card was added to the deck.", ctx.UsingPlayer));
    }
}

