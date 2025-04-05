using UnityEngine;

public interface Card
{
    CardType Type { get; }
    int Id { get; }

    void Apply(GameState gs);
}

public enum CardType 
{
    Offer,      // Money value cards
    Perk,       // Special ability cards
    Snap,       // CEO snap cards that end turn
    Shenanigan  // Random event cards
}

public class OfferCard : Card
{
    private readonly int _id;
    private readonly int _value;

    public OfferCard(int value)
    {
        _value = value;
    }

    public CardType Type => CardType.Offer;
    public int Id { get; }
    
    public void Apply(GameState gs)
    {
        throw new System.NotImplementedException();
    }

    public int id => _id + 1000;
}

