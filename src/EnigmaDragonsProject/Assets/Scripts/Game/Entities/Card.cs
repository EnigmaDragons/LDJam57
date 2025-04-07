using System;
using UnityEngine;

public interface Card
{
    CardType Type { get; }
    int Id { get; }
    int BossMoodMod { get; }
    string Description { get; }

    void Apply(GameState gs, PlayerState actingPlayer, Func<int, int> valueModifier);
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

    public int Value => _value;

    public OfferCard(int value, int bossMoodMod)
    {
        _value = value;
        BossMoodMod = bossMoodMod;
    }

    public CardType Type => CardType.Offer;
    public int Id => _id;
    public int BossMoodMod { get; }
    public string Description => $"+${_value}";

    public void Apply(GameState gs, PlayerState actingPlayer, Func<int, int> valueModifier)
    {
        CurrentGameState.UpdateState(_ => actingPlayer.ChangeCurrentDayCash(valueModifier(_value)));
    }

    public int id => _id + 1000;
}

public class SnapCard : Card
{
    private static int _nextId = 1;
    private readonly int _id;
    
    public SnapCard()
    {
        _id = _nextId++;
    }
    
    public CardType Type => CardType.Snap;
    public int Id => _id;
    public int BossMoodMod => 0;
    public string Description => "SNAP! Lose today's cash!";
    
    public void Apply(GameState gs, PlayerState actingPlayer, Func<int, int> valueModifier)
    {
        // The player loses all cash from today and is knocked out
        CurrentGameState.UpdateState(_ => actingPlayer.NotifySnapped());
        
        // Publish a message about the snap
        Message.Publish(new PlayerSnapped(actingPlayer));
    }
}

