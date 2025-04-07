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
        PowerName = "Executive Ear",
        PowerDescription = "Passive: Asking for money never escalates the chief's mood.",
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

    public static Character NekoChan = new StaticCharacter
    {
        Name = "Neko-chan",
        PowerName = "Empathy Ally",
        PowerDescription = "When you accept an offer, if you don't have the most banked, bank +$10.",
        Department = "User Experience",
        SpritePathFace = "Sprites/PlayerCharacters/Face-NekoChan",
        Power = new SympathyBuddyPower()
    };

    public static Character SirPouncelot = new StaticCharacter
    {
        Name = "Sir Pouncelot",
        PowerName = "Party's Over",
        PowerDescription = "After you accept an offer, shuffle 7 snaps into the deck immediately.",
        Department = "Marketing",
        SpritePathFace = "Sprites/PlayerCharacters/Face-SirPouncelot",
        Power = new SabotageNegotiationPower()
    };
   
    public static Character RockyClawboa = new StaticCharacter
    {
        Name = "Rocky Clawboa",
        PowerName = "Hustle & Grind",
        PowerDescription = "Always draws two cards per ask, instead of one.",
        Department = "Product Development",
        SpritePathFace = "Sprites/PlayerCharacters/Face-RockyClawboa",
    };

    public static Character DJScratchy = new StaticCharacter
    {
        Name = "DJ Scratchy",
        PowerName = "Spin It Around",
        PowerDescription = "Gain more money for low value cards, and less for high value cards.",
        Department = "Social Media",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DJScratchy",
        Power = new OfferInverterPower()
    };
    
    public static Character DrPurrington = new StaticCharacter
    {
        Name = "Dr. Purrington",
        PowerName = "Brilliant Idea",
        PowerDescription = "Whenver you choose to draw a card, first shuffle a +$20 card into the deck.",
        Department = "Research & Development",
        SpritePathFace = "Sprites/PlayerCharacters/Face-DrPurrington",
        Power = new BrilliantIdeaPower()
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
