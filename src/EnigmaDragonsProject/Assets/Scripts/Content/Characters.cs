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
    
    public static Character MissMeowgan = new StaticCharacter
    {
        Name = "Miss Meowgan",
        PowerDescription = "Spin Doctor - Can discard 1 Snap card per round by explaining it away as a \"strategic pivot.\"",
        Department = "Public Relations",
        SpritePathFace = "Sprites/PlayerCharacters/Face-MissMeowgan",
    };
    
    public static Character WhiskersMcNumbers = new StaticCharacter
    {
        Name = "Whiskers McNumbers",
        PowerDescription = "Financial Acumen - Gets +10% bonus on all Offer Cards because he knows where the money is hidden.",
        Department = "Finance",
        SpritePathFace = "Sprites/PlayerCharacters/Face-WhiskersMcNumbers",
    };
    
    public static Character SirPouncelot = new StaticCharacter
    {
        Name = "Sir Pouncelot",
        PowerDescription = "Risk Taker - Can draw 1 extra card beyond the limit, but doubles Snap risk.",
        Department = "Marketing",
        SpritePathFace = "Sprites/PlayerCharacters/Face-SirPouncelot",
    };
    
    public static Character NekoChan = new StaticCharacter
    {
        Name = "Neko-chan",
        PowerDescription = "Kawaii Distraction - Can steal 1 Offer Card from another cat once per round by being adorable.",
        Department = "User Experience",
        SpritePathFace = "Sprites/PlayerCharacters/Face-NekoChan",
    };
    
    public static Character RockyClawboa = new StaticCharacter
    {
        Name = "Rocky Clawboa",
        PowerDescription = "Hustle & Grind - Can draw two cards at once, must keep both (good or bad).",
        Department = "Product Development",
        SpritePathFace = "Sprites/PlayerCharacters/Face-RockyClawboa",
    };
    
    public static Character DrPurrington = new StaticCharacter
    {
        Name = "Dr. Purrington",
        PowerDescription = "Eureka Moment - Once per game, after seeing a Snap Card, can declare \"EUREKA!\" and shuffle it back into the deck.",
        Department = "Research & Development",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DrPurrington",
    };
    
    public static Character DJScratchy = new StaticCharacter
    {
        Name = "DJ Scratchy",
        PowerDescription = "Trending Topic - Can peek at the next three cards in the deck to see what's coming.",
        Department = "Social Media",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DJScratchy",
    };
    
    public static Character ProfessorWhiskerton = new StaticCharacter
    {
        Name = "Professor Whiskerton",
        PowerDescription = "Data-Driven - After seeing 3 cards, can accurately predict the probability of drawing a Snap card.",
        Department = "Analytics",
        SpritePathFace = "Sprites/PlayerCharacters/Face-ProfessorWhiskerton",
    };
    
    public static Character DuchessFluffington = new StaticCharacter
    {
        Name = "Duchess Fluffington",
        PowerDescription = "Executive Connection - Once per game, can make a direct appeal to Mr. Mittens to reverse a Snap.",
        Department = "Executive Relations",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DuchessFluffington",
    };

    public static List<Character> AllCharacters { get; } = new List<Character>
    {
        TabbyTom,
        MissMeowgan,
        WhiskersMcNumbers,
        SirPouncelot,
        NekoChan,
        RockyClawboa,
        DrPurrington,
        DJScratchy,
        ProfessorWhiskerton,
        DuchessFluffington
    };
}
