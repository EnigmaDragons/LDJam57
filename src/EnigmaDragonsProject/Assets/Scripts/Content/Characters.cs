using System.Collections.Generic;

public class Characters
{
    public static Character TabbyTom = new StaticCharacter
    {
        Name = "Tabby Tom",
        PowerDescription = "Beginner's Luck - Once per game, can reroll a bad draw.",
        Department = "Customer Service",
        SpritePathFace = "Sprites/PlayerCharacters/Face-TabbyTom",
    };
    

    public static List<Character> AllCharacters { get; } = new List<Character>
    {
      TabbyTom,
    };
}
