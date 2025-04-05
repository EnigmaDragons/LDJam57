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
        MusicPath = "Music/V3 - FRIDAY - MR. MITTENS' EXECUTIVE SUITE",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 6 },
          { 10, 4 },
          { 15, 6 },
          { 20, 8 },
          { 25, 10 },
          { 30, 12 },
          { 35, 14 },
          { 40, 16 },
          { 45, 18 },
          { 50, 20 },
          { 55, 22 },
          { 60, 24 },
        }
    };
    
    public static readonly Boss VictoriaClawford = new StaticBoss
    {
        Name = "Victoria Clawford",
        EnvironmentName = "Finance Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-VictoriaClawford",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-VictoriaClawford",
        MusicPath = "Music/V3 - THURSDAY - VICTORIA CLAWFORD'S FINANCE DEPARTMENT",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 4 },
          { 10, 5 },
          { 15, 6 },
          { 20, 7 },
          { 25, 8 },
          { 30, 9 },
          { 35, 10 },
          { 40, 11 },
          { 45, 12 },
          { 50, 13 },
          { 55, 14 },
          { 60, 15 },
        }
    };
    
    public static readonly Boss RexGrowlington = new StaticBoss
    {
        Name = "Rex Growlington",
        EnvironmentName = "Security Office",
        SpriteEnvironmentPath = "Sprites/Environments/Env-RexGrowlington",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-RexGrowlington",
        MusicPath = "Music/V3 - WEDNESDAY - REX GROWLINGTON'S SECURITY OPERATIONS CENTER",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 5 },
          { 10, 5 },
          { 15, 5 },
          { 20, 5 },
          { 25, 5 },
          { 30, 5 },
          { 35, 5 },
          { 40, 5 },
          { 45, 5 },
          { 50, 5 },
          { 55, 5 },
          { 60, 5 },
        }
    };
    
    public static readonly Boss PawlaWhiskerberg = new StaticBoss
    {
        Name = "Pawla Whiskerberg",
        EnvironmentName = "Legal Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-PawlaWhiskerberg",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-PawlaWhiskerberg",
        MusicPath = "Music/V3 - TUESDAY - PAWLA WHISKERBERG'S LEGAL DEPARTMENT",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 5 },
          { 10, 5 },
          { 15, 4 },
          { 20, 4 },
          { 25, 3 },
          { 30, 3 },
          { 35, 2 },
          { 40, 2 },
          { 45, 2 },
          { 50, 2 },
          { 55, 2 },
          { 60, 2 },
        }
    };
    
    public static readonly Boss DrFelixBytepaws = new StaticBoss
    {
        Name = "Dr. Felix Bytepaws",
        EnvironmentName = "Tech Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-DrFelixBytepaws",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-DrFelixBytepaws",
        MusicPath = "Music/V3 - MONDAY - DR. FELIX BYTEPAWS' TECH DEVELOPMENT LAB",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 5 },
          { 10, 4 },
          { 15, 3 },
          { 20, 2 },
          { 25, 1 },
          { 30, 1 },
          { 35, 1 },
          { 40, 1 },
          { 45, 1 },
          { 50, 1 },
          { 55, 1 },
          { 60, 1 },
        }
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