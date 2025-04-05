using UnityEngine;
using System.Collections.Generic;

public static class Bosses
{
    public static readonly Boss MrMittens = new StaticBoss
    {
        Name = "Mr. Mittens",
        EnvironmentName = "CEO Office",
        SpriteEnvironmentPath = "Sprites/Environments/Env-MrMittens",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-MrMittens",
        MusicPath = "Music/V3 - FRIDAY - MR. MITTENS' EXECUTIVE SUITE"
    };
    
    public static readonly Boss VictoriaClawford = new StaticBoss
    {
        Name = "Victoria Clawford",
        EnvironmentName = "Finance Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-VictoriaClawford",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-VictoriaClawford",
        MusicPath = "Music/V3 - THURSDAY - VICTORIA CLAWFORD'S FINANCE DEPARTMENT"
    };
    
    public static readonly Boss RexGrowlington = new StaticBoss
    {
        Name = "Rex Growlington",
        EnvironmentName = "Security Office",
        SpriteEnvironmentPath = "Sprites/Environments/Env-RexGrowlington",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-RexGrowlington",
        MusicPath = "Music/V3 - WEDNESDAY - REX GROWLINGTON'S SECURITY OPERATIONS CENTER"
    };
    
    public static readonly Boss PawlaWhiskerberg = new StaticBoss
    {
        Name = "Pawla Whiskerberg",
        EnvironmentName = "Legal Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-PawlaWhiskerberg",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-PawlaWhiskerberg",
        MusicPath = "Music/V3 - TUESDAY - PAWLA WHISKERBERG'S LEGAL DEPARTMENT"
    };
    
    public static readonly Boss DrFelixBytepaws = new StaticBoss
    {
        Name = "Dr. Felix Bytepaws",
        EnvironmentName = "Tech Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-DrFelixBytepaws",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-DrFelixBytepaws",
        MusicPath = "Music/V3 - MONDAY - DR. FELIX BYTEPAWS' TECH DEVELOPMENT LAB"
    };
    
    public static readonly Dictionary<Day, Boss> DayBossMap = new Dictionary<Day, Boss>
    {
        { Day.MONDAY, DrFelixBytepaws },
        { Day.TUESDAY, PawlaWhiskerberg },
        { Day.WEDNESDAY, RexGrowlington },
        { Day.THURSDAY, VictoriaClawford },
        { Day.FRIDAY, MrMittens }
    };

    public static readonly Boss[] All = {
        DrFelixBytepaws,
        PawlaWhiskerberg,
        RexGrowlington,
        VictoriaClawford,
        MrMittens
    };
}