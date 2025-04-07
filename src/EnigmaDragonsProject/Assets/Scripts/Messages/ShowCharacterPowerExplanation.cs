
public class ShowCharacterPowerExplanation
{
    public string Explanation { get; }
    public PlayerState Player { get; }

    public ShowCharacterPowerExplanation(string explanation, PlayerState player)
    {
        Player = player;
        Explanation = explanation;
    }
}
