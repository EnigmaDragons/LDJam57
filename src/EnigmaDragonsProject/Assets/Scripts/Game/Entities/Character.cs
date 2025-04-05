using UnityEngine;

// Design Thought: Should powers be unlockable... should we do XP, etc?
public interface Character
{
    public string Name { get; }
    public string Department { get; }
    
    public string PowerName { get; }
    public string PowerDescription { get; }

    public Sprite Profile { get; }
    public Sprite Face { get; }
}

public class StaticCharacter : Character
{
    public string Name { get; set; }
    public string Department { get; set; }

    public string PowerName { get; set; }
    public string PowerDescription { get; set; }

    public string SpritePathProfile { get; set; }
    public string SpritePathFace { get; set; }
    
    public Sprite Profile => Resources.Load<Sprite>(SpritePathProfile);
    public Sprite Face => Resources.Load<Sprite>(SpritePathFace);
}
