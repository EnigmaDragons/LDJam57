using System.Collections.Generic;

public class Characters
{
    // Power Concept Designed
    public static Character TabbyTom = new StaticCharacter
    {
        Name = "Tabby Tom",
        PowerName = "Early Bird",
        PowerDescription = "Each day, gain $5 before the negotiations start",
        Department = "Customer Service",
        SpritePathFace = "Sprites/PlayerCharacters/Face-TabbyTom",
        Power = new GainMoneyAtStartOfDayPower(5)
    };
    
    public static Character MissMeowgan = new StaticCharacter
    {
        Name = "Miss Meowgan",
        PowerName = "Spin Doctor",  
        PowerDescription = "Once per game, the first time you draw a snap card, ignore it.",
        Department = "Public Relations",
        SpritePathFace = "Sprites/PlayerCharacters/Face-MissMeowgan",
        Power = new IgnoreOneSnapCardEver()
    };
    
    public static Character WhiskersMcNumbers = new StaticCharacter
    {
        Name = "Whiskers McNumbers",
        PowerName = "Saavy Investor",
        PowerDescription = "Each day, bank 10% interest before negotiations start.",
        Department = "Finance",
        SpritePathFace = "Sprites/PlayerCharacters/Face-WhiskersMcNumbers",
        Power = new BankInterestPower()
    };

    public static Character DuchessFluffington = new StaticCharacter
    {
        Name = "Duchess Fluffington",
        PowerName = "Executive Connection",
        PowerDescription = "Passive: You negotiating for more money never escalates the chief's mood.",
        Department = "Executive Relations",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DuchessFluffington",
        Power = new NoMoodEscalationPower()
    };
    
    public static Character ProfessorWhiskerton = new StaticCharacter
    {
        Name = "Professor Whiskerton",
        PowerName = "Data-Driven Analyst",
        PowerDescription = "Passive: You can always see the EXACT Snap Odds.",
        Department = "Analytics",
        SpritePathFace = "Sprites/PlayerCharacters/Face-ProfessorWhiskerton",
        Power = new SeeSnapOddsPower()
    };
    
    public static Character SirPouncelot = new StaticCharacter
    {
        Name = "Sir Pouncelot",
        PowerName = "Risk Taker",
        PowerDescription = "Can draw 1 extra card beyond the limit, but doubles Snap risk.",
        Department = "Marketing",
        SpritePathFace = "Sprites/PlayerCharacters/Face-SirPouncelot",
    };
    
    public static Character NekoChan = new StaticCharacter
    {
        Name = "Neko-chan",
        PowerName = "Kawaii Distraction",
        PowerDescription = "Can steal 1 Offer Card from another cat once per round by being adorable.",
        Department = "User Experience",
        SpritePathFace = "Sprites/PlayerCharacters/Face-NekoChan",
    };
    
    public static Character RockyClawboa = new StaticCharacter
    {
        Name = "Rocky Clawboa",
        PowerName = "Hustle & Grind",
        PowerDescription = "Draws two cards at once.",
        Department = "Product Development",
        SpritePathFace = "Sprites/PlayerCharacters/Face-RockyClawboa",
    };
    
    public static Character DrPurrington = new StaticCharacter
    {
        Name = "Dr. Purrington",
        PowerName = "Eureka Moment",
        PowerDescription = "TBD",
        Department = "Research & Development",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DrPurrington",
    };
    
    public static Character DJScratchy = new StaticCharacter
    {
        Name = "DJ Scratchy",
        PowerName = "Trending Topic",
        PowerDescription = "Can peek at the next three cards in the deck to see what's coming.",
        Department = "Social Media",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DJScratchy",
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
