using UnityEngine;

// Design Thought: Should powers be unlockable... should we do XP, etc?
public interface Character
{
    public string Name { get; }
    public string Department { get; }
    
    public string PowerName { get; }
    public string PowerDescription { get; }
    public CharacterPower Power { get; }

    public Sprite Face { get; }
}

public class StaticCharacter : Character
{
    public string Name { get; set; }
    public string Department { get; set; }

    public string PowerName { get; set; }
    public string PowerDescription { get; set; }
    public CharacterPower Power { get; set; } = new NoPower();

    public string SpritePathFace { get; set; }
    
    public Sprite Face => ResourceUtils.LoadOrThrow<Sprite>(SpritePathFace);
}
