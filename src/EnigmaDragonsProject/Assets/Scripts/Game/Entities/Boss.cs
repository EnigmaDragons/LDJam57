using System.Collections.Generic;
using UnityEngine;

public interface Boss
{
    public int Id { get; }
    public string Name { get; }
    public Dictionary<int, int> MoodSnapsTable { get; }

    public string EnvironmentName { get; }
    public Sprite Environment { get; }
    
    public Sprite Profile { get; }
    
    public AudioClip Music { get; }
}

public class StaticBoss : Boss
{
    public int Id { get; set; } = -1;
    public string Name { get; set; }
    public string EnvironmentName { get; set; }
    public Dictionary<int, int> MoodSnapsTable { get; set; }
    
    public string SpriteEnvironmentPath { get; set; }
    public string SpriteProfilePath { get; set; }
    public string MusicPath { get; set; }
    
    public Sprite Environment => ResourceUtils.LoadOrThrow<Sprite>(SpriteEnvironmentPath);
    public Sprite Profile =>  ResourceUtils.LoadOrThrow<Sprite>(SpriteProfilePath);
    public AudioClip Music => ResourceUtils.LoadOrThrow<AudioClip>(MusicPath);
}
