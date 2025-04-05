using UnityEngine;

public interface Boss
{
    public string Name { get; }

    public string EnvironmentName { get; }
    public Sprite Environment { get; }
    
    public Sprite Profile { get; }
    
    public AudioClip Music { get; }
}

public class StaticBoss : Boss
{
    public string Name { get; set; }
    public string EnvironmentName { get; set; }
    
    public string SpriteEnvironmentPath { get; set; }
    public string SpriteProfilePath { get; set; }
    public string MusicPath { get; set; }
    
    public Sprite Environment => Resources.Load<Sprite>(SpriteEnvironmentPath);
    public Sprite Profile => Resources.Load<Sprite>(SpriteProfilePath);
    public AudioClip Music => Resources.Load<AudioClip>(MusicPath);
}
