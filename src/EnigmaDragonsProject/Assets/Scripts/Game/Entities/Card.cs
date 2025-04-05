using UnityEngine;

public interface Card
{
    CardType Type { get; }
    int Id { get; }
    int BossMoodMod { get; }

    void Apply(GameState gs, PlayerState actingPlayer);
}

public enum CardType 
{
    Offer,      // Money value cards
    Snap,       // CEO snap cards that end turn
    Perk,       // Special ability cards
    Shenanigan  // Random event cards
}

public class OfferCard : Card
{
    private readonly int _id;
    private readonly int _value;

    public OfferCard(int value, int bossMoodMod)
    {
        _value = value;
        BossMoodMod = bossMoodMod;
    }

    public CardType Type => CardType.Offer;
    public int Id => _id;
    public int BossMoodMod { get; private set;  }

    public void Apply(GameState gs, PlayerState actingPlayer)
    {
        CurrentGameState.UpdateState(_ => actingPlayer.ChangeCurrentDayCash(_value));
    }

    public int id => _id + 1000;
}

