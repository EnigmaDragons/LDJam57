using System;

[Serializable]
public sealed class GameState
{
    // Should consist of only serializable primitives.
    // Any logic or non-trivial data should be enriched in CurrentGameState.
    // Except for Save/Load Systems, everything should use CurrentGameState,
    // instead of this pure data structure.
    
    // All enums used in this class should have specified integer values.
    // This is necessary to preserve backwards save compatibility.

    public bool IsInitialized = false;
    public Day CurrentDay = Day.MONDAY;
    public Boss CurrentBoss = Bosses.DrFelixBytepaws;
    public PlayerState[] PlayerStates = Array.Empty<PlayerState>();
    public Deck CurrentDeck = BasicDeck.CreateStandardDeck();
    public bool IsGameOver = false;
    
    public void AdvanceToNextDay()
    {
        // Advance to the next day in the work week
        if (CurrentDay == Day.MONDAY)
            CurrentDay = Day.TUESDAY;
        else if (CurrentDay == Day.TUESDAY)
            CurrentDay = Day.WEDNESDAY;
        else if (CurrentDay == Day.WEDNESDAY)
            CurrentDay = Day.THURSDAY;
        else if (CurrentDay == Day.THURSDAY)
            CurrentDay = Day.FRIDAY;
            
        // Notify all players of the day change
        foreach (var player in PlayerStates)
        {
            player.NotifyDayChanged();
        }
        
        // Create a new deck for the day
        CurrentDeck = BasicDeck.CreateStandardDeck();
    }
}


public class PlayerState
{
    public Player Player { get; }
    public int Cash { get; private set; }
    public bool PowerUsedToday { get; private set; }
    public bool PowerUsedEver { get; private set; }

    public PlayerState(Player p, int currentCash)
    {
        Player = p;
        Cash = currentCash;
    }

    public void ChangeCash(int byAmount)
    {
        Cash += byAmount;
    }

    public void NotifyDayChanged()
    {
        PowerUsedToday = false;
    }

    public void RecordPowerUser()
    {
        PowerUsedToday = true;
        PowerUsedEver = true;
    }
}
