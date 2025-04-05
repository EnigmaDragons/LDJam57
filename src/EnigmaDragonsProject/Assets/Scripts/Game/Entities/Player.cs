using System;

[Serializable]
public class Player
{
    public Player(int index, Character c)
    {
        Index = index;
        Character = c;
    }

    public int Index { get; }
    public Character Character { get; }
}
