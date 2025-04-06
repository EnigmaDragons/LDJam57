public class DayFinished 
{
    public Day Day { get; }
    public PlayerState[] PlayerStates { get; }
    
    public DayFinished(Day day, PlayerState[] playerStates)
    {
        Day = day;
        PlayerStates = playerStates;
    }
}
