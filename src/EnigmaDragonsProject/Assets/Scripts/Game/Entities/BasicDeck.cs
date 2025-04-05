using System.Collections.Generic;

public static class BasicDeck
{
    private static readonly Dictionary<Card, int> OfferCardCounts = new Dictionary<Card, int>
    {
        { new OfferCard(5, -1), 10 },
        { new OfferCard(10, +1), 32 },
        { new OfferCard(15, +2), 8 },
        { new OfferCard(20, +3), 5 },
        { new OfferCard(25, +4), 2 }
    };

    public static Deck CreateStandardDeck()
    {
        var cards = new List<Card>();

        foreach (var offerCard in OfferCardCounts)
          for (int i = 0; i < offerCard.Value; i++)
            cards.Add(offerCard.Key);

        // Create and shuffle the deck
        var deck = new Deck(cards);
        deck.Shuffle();
        return deck;
    }
}
