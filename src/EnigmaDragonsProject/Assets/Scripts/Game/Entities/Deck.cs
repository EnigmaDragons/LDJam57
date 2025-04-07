using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class Deck
{
    private List<Card> _cards;
    private RNGCryptoServiceProvider _cryptoRng;

    public Deck(List<Card> initialCards)
    {
        _cards = initialCards.ToList();
        _cryptoRng = new RNGCryptoServiceProvider();
    }

    // Get a cryptographically secure random integer between min (inclusive) and max (exclusive)
    private int GetSecureRandomNumber(int min, int max)
    {
        if (min >= max)
            throw new ArgumentOutOfRangeException("Min must be less than max.");

        // Create a span to receive the random bytes
        Span<byte> randomBytes = stackalloc byte[sizeof(uint)];
        uint scale = uint.MaxValue;
        uint range = (uint)(max - min);
        
        uint result;
        do
        {
            // Fill the buffer with random bytes
            _cryptoRng.GetBytes(randomBytes.ToArray());
            
            // Convert bytes to uint
            result = BitConverter.ToUInt32(randomBytes);
            
        } while (result >= scale - (scale % range)); // Reject results that would create bias
        
        return min + (int)(result % range);
    }

    public void Shuffle()
    {
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = GetSecureRandomNumber(0, n + 1);
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
            PutCardOnTop(card);
            Shuffle();
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
