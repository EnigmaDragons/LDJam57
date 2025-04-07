using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public Day CurrentDay = Day.Monday;
    public BossState BossState = new BossState(Bosses.DrFelixBytepaws, 0);
    public PlayerState[] PlayerStates = Array.Empty<PlayerState>();
    public Deck CurrentDeck = BasicDeck.CreateStandardDeck();
    public bool IsGameOver = false;
    public int PlayerTurnIndex = 0;
    public bool IsDayFinished = false;

    public PlayerState ActivePlayer => PlayerTurnIndex > -1 ? PlayerStates[PlayerTurnIndex] : null;
    public int ActivePlayerCount => PlayerStates.Count(x => x.IsActiveInDay);
    
    public void AdvanceToNextDay()
    {
        if (CurrentDay == Day.Friday)
        {
            IsGameOver = true;
            Message.Publish(new GameOver(this));
            return;
        }
        
        // Make sure all CurrentRoundCash gets added to BankedCash before advancing
        foreach (var player in PlayerStates)
        {
            // Ensure any outstanding CurrentRoundCash gets banked
            if (player.CurrentRoundCash > 0)
            {
                Debug.Log($"Banking {player.CurrentRoundCash} cash for player {player.Player.Index} before advancing day");
                player.BankCash();
            }
        }
        
        // Advance to the next day in the work week
        if (CurrentDay == Day.Monday)
            CurrentDay = Day.Tuesday;
        else if (CurrentDay == Day.Tuesday)
            CurrentDay = Day.Wednesday;
        else if (CurrentDay == Day.Wednesday)
            CurrentDay = Day.Thursday;
        else if (CurrentDay == Day.Thursday)
            CurrentDay = Day.Friday;
            
        // Notify all players of the day change
        foreach (var player in PlayerStates)
        {
            // Log for debugging
            int beforeBankedCash = player.BankedCash;
            int beforeRoundCash = player.CurrentRoundCash;
            
            player.NotifyDayChanged();
            
            // Verify banked cash is preserved
            Debug.Log($"Player {player.Player.Index} - Before: Banked=${beforeBankedCash}, Round=${beforeRoundCash} | After: Banked=${player.BankedCash}");
        }
        
        // Create a new deck for the day
        CurrentDeck = BasicDeck.CreateStandardDeck();
        BossState = new BossState(Bosses.DayBossMap[CurrentDay], 0);
        IsDayFinished = false;
    }

    public void MoveToNextActivePlayer()
    {
        if (PlayerStates.Length == 0)
            return;
            
        if (ActivePlayerCount == 0)
            return;
            
        int startingIndex = PlayerTurnIndex;
        int nextIndex = (PlayerTurnIndex + 1) % PlayerStates.Length;
        
        // Keep looking for the next active player
        while (nextIndex != startingIndex)
        {
            if (PlayerStates[nextIndex].IsActiveInDay)
            {
                PlayerTurnIndex = nextIndex;
                return;
            }
            nextIndex = (nextIndex + 1) % PlayerStates.Length;
        }
    }
}

public class PlayerState
{
    public Player Player { get; }
    public Ai Ai { get; }
    public int BankedCash { get; private set; }
    public int CurrentRoundCash { get; private set; }
    public bool PowerUsedToday { get; private set; }
    public bool PowerUsedEver { get; private set; }
    public bool IsActiveInDay { get; private set; }
    public bool HasDrawnCardToday { get; private set; }
    public int CashBankedToday { get; private set; }

    public PlayerState(Player p, int currentCash, Ai ai)
    {
        Player = p;
        BankedCash = currentCash;
        Ai = ai;
    }

    public void ChangeCurrentDayCash(int byAmount)
    {
        CurrentRoundCash += byAmount;
        HasDrawnCardToday = true;
    }

    public void ChangeCashFromEffect(int amount)
    {
        CurrentRoundCash += amount;
    }

    public void NotifyDayChanged()
    {
        PowerUsedToday = false;
        BankedCash += CurrentRoundCash;
        CurrentRoundCash = 0;
        IsActiveInDay = true;
        HasDrawnCardToday = false;
        CashBankedToday = 0;
        Player.Character.Power.NotifyNewDayStarted();
    }

    public void RecordPowerUser()
    {
        PowerUsedToday = true;
        PowerUsedEver = true;
    }

    public void BankCash()
    {
        // Add current round cash to banked cash
        CashBankedToday += CurrentRoundCash;
        BankedCash += CurrentRoundCash;
        Debug.Log($"Player {Player.Index} banked ${CurrentRoundCash}, new total: ${BankedCash}");
        CurrentRoundCash = 0;
        IsActiveInDay = false;    
    }

    public void NotifySnapped()
    {
        CurrentRoundCash = 0;
        IsActiveInDay = false;
    }
}

public class BossState
{
    public Boss Boss { get; private set; }
    public int CurrentMood { get; private set; }
    public int CurrentMoodTier { get; private set; }
    
    public BossState(Boss boss, int initialMood)
    {
        Boss = boss;
        CurrentMood = initialMood;
        CurrentMoodTier = GetMoodTier(initialMood);
    }

    public int UpdateMoodAndGetSnapsChanges(int moodChange)
    {
        // Apply the mood change but ensure it doesn't go below 0
        CurrentMood = Mathf.Max(0, CurrentMood + moodChange);
        int newMoodTier = GetMoodTier(CurrentMood);
        
        // Check if we've crossed a mood threshold to a higher tier
        if (newMoodTier > CurrentMoodTier)
        {
            CurrentMoodTier = newMoodTier;
            
            // Return the number of snaps for the new tier
            if (Boss.MoodSnapsTable.TryGetValue(newMoodTier, out int snapsForTier))
            {
                return snapsForTier;
            }
        }
        
        // No tier change or no snaps defined for this tier
        return 0;
    }
    
    private int GetMoodTier(int mood)
    {
        // Find the highest tier that the current mood exceeds
        int highestTier = 0;
        
        foreach (var tier in Boss.MoodSnapsTable.Keys)
        {
            if (mood >= tier && tier > highestTier)
            {
                highestTier = tier;
            }
        }
        
        return highestTier;
    }
}
