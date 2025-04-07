using System.Linq;
using UnityEngine;

public static class CharacterPowerProcessor    
{
    public static void TriggerStartOfWeekPowers()
    {
        var players = CurrentGameState.ReadOnly.PlayerStates.ToArray();
        foreach (var ps in players)
            if (ps.Player.Character.Power.PowerType == PowerType.AutoStartOfWeek)
                ps.Player.Character.Power.Apply(new PowerContext(CurrentGameState.ReadOnly, ps));
        Debug.Log("[CharacterPowerProcessor] - Applied all Start of Week Powers");
    }

    public static void TriggerStartOfDayPowers()
    {
        var players = CurrentGameState.ReadOnly.PlayerStates.ToArray();
        foreach (var ps in players)
            if (ps.Player.Character.Power.PowerType == PowerType.AutoStartOfDay)
                ps.Player.Character.Power.Apply(new PowerContext(CurrentGameState.ReadOnly, ps));
        Debug.Log("[CharacterPowerProcessor] - Applied all Start of Day Powers");
    }
}

