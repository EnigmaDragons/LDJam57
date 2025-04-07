using UnityEngine;
using System.Collections.Generic;

public static class Bosses
{
    public static readonly Boss MrMittens = new StaticBoss
    {
        Id = 4,
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
        Id = 3,
        Name = "Victoria Clawford",
        EnvironmentName = "Finance Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-VictoriaClawford",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-VictoriaClawford",
        MusicPath = "Music/V3 - THURSDAY - VICTORIA CLAWFORD'S FINANCE DEPARTMENT",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 3 },
          { 10, 4 },
          { 15, 5 },
          { 20, 6 },
          { 25, 7 },
          { 30, 8 },
          { 35, 9 },
          { 40, 10 },
          { 45, 11 },
          { 50, 12 },
          { 55, 13 },
          { 60, 14 },
        }
    };
    
    public static readonly Boss RexGrowlington = new StaticBoss
    {
        Id = 2,
        Name = "Rex Growlington",
        EnvironmentName = "Security Office",
        SpriteEnvironmentPath = "Sprites/Environments/Env-RexGrowlington",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-RexGrowlington",
        MusicPath = "Music/V3 - WEDNESDAY - REX GROWLINGTON'S SECURITY OPERATIONS CENTER",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 6 },
          { 10, 4 },
          { 15, 4 },
          { 20, 4 },
          { 25, 4 },
          { 30, 4 },
          { 35, 4 },
          { 40, 4 },
          { 45, 4 },
          { 50, 4 },
          { 55, 4 },
          { 60, 4 },
        }
    };
    
    public static readonly Boss PawlaWhiskerberg = new StaticBoss
    {
        Id = 1,
        Name = "Pawla Whiskerberg",
        EnvironmentName = "Legal Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-PawlaWhiskerberg",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-PawlaWhiskerberg",
        MusicPath = "Music/V3 - TUESDAY - PAWLA WHISKERBERG'S LEGAL DEPARTMENT",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 4 },
          { 10, 4 },
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
        Id = 0,
        Name = "Dr. Felix Bytepaws",
        EnvironmentName = "Tech Department",
        SpriteEnvironmentPath = "Sprites/Environments/Env-DrFelixBytepaws",
        SpriteProfilePath = "Sprites/BossCharacters/Profile-DrFelixBytepaws",
        MusicPath = "Music/V3 - MONDAY - DR. FELIX BYTEPAWS' TECH DEVELOPMENT LAB",
        MoodSnapsTable = new Dictionary<int, int>
        {
          { 5, 4 },
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
        { Day.Monday, DrFelixBytepaws },
        { Day.Tuesday, PawlaWhiskerberg },
        { Day.Wednesday, RexGrowlington },
        { Day.Thursday, VictoriaClawford },
        { Day.Friday, MrMittens }
    };

    public static readonly Boss[] All = {
        DrFelixBytepaws,
        PawlaWhiskerberg,
        RexGrowlington,
        VictoriaClawford,
        MrMittens
    };
}