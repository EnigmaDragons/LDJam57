using System.Collections.Generic;

public static class BasicDeck
{
    private static readonly Dictionary<Card, int> OfferCardCounts = new Dictionary<Card, int>
    {
        { new OfferCard(5), 10 },   // Common offers ($5) - 20 cards
        { new OfferCard(10), 32 },  // Uncommon offers ($10) - 15 cards
        { new OfferCard(15), 8 },   // Rare offers ($15) - 8 cards
        { new OfferCard(20), 5 },   // Very Rare offers ($20) - 5 cards
        { new OfferCard(25), 2 }    // Legendary offers ($25) - 2 cards
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
