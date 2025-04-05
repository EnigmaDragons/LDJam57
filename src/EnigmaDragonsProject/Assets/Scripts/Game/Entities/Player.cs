using System;

[Serializable]
public class Player
{
    public Player(int index, Character c, PlayerType playerType)
    {
        Index = index;
        Character = c;
        PlayerType = playerType;
    }

    public int Index { get; }
    public Character Character { get; }
    public PlayerType PlayerType { get; }
}

public enum PlayerType
{
    Human,
    AI
}

