public class ShowDayResults
{
    public Day Day { get; }
    public PlayerState[] PlayerStates { get; }
    
    public ShowDayResults(Day day, PlayerState[] playerStates)
    {
        Day = day;
        PlayerStates = playerStates;
    }
} 