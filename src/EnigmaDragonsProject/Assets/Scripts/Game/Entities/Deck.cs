using System;
using System.Collections.Generic;
using System.Linq;

public class Deck
{
    private List<Card> _cards;
    private System.Random _rng;

    public Deck(List<Card> initialCards)
    {
        _cards = initialCards.ToList();
        _rng = new System.Random(Guid.NewGuid().GetHashCode());
    }

    public void Shuffle()
    {
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
    }

    public Deck Shuffled()
    {
        Shuffle();
        return this;
    }

    public Card DrawOne()
    {
        if (_cards.Count == 0)
            return null;

        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void PutCardOnBottom(Card card)
    {
        if (card != null)
            _cards.Add(card);
    }

    public void ShuffleCardIn(Card card)
    {
        if (card != null)
        {
            int position = _rng.Next(_cards.Count + 1);
            _cards.Insert(position, card);
        }
    }

    public void PutCardOnTop(Card card)
    {
        if (card != null)
            _cards.Insert(0, card);
    }

    public int Count => _cards.Count;
    
    public int SnapCount => _cards.Count(c => c.Type == CardType.Snap);
}
