using System.Collections.Generic;
using UnityEngine;

public interface Ai
{
    ActionType SelectAction(GameState gs, PlayerState ps);
}

public class NoAi : Ai
{
    public ActionType SelectAction(GameState gs, PlayerState ps)
    {
        return ActionType.DrawCard;
    }
}

public class BasicRiskAwareAi : Ai
{
    // Risk tolerance parameters (can be tuned)
    private const float LowCashThreshold = 20f;      // Amount considered "low" cash
    private const float HighCashThreshold = 55f;     // Amount considered "high" cash
    private const float SnapRiskThreshold = 0.3f;    // 30% chance of snap is considered risky
    private const float LeadThreshold = 15f;         // How much we need to be ahead to feel safe
    
    // State tracking
    private readonly Dictionary<Day, int> _riskTakenPerDay = new Dictionary<Day, int>();
    private readonly Dictionary<Day, int> _cardsDrawnPerDay = new Dictionary<Day, int>();
    
    public ActionType SelectAction(GameState gs, PlayerState ps)
    {
        // Initialize tracking for this day if needed
        if (!_riskTakenPerDay.ContainsKey(gs.CurrentDay))
        {
            _riskTakenPerDay[gs.CurrentDay] = 0;
            _cardsDrawnPerDay[gs.CurrentDay] = 0;
        }
        
        // Get current game state information
        int currentCash = ps.CurrentRoundCash;
        int remainingCards = gs.CurrentDeck.Count;
        
        // Calculate the highest banked cash among opponents
        int highestOpponentCash = 0;
        foreach (var player in gs.PlayerStates)
          if (player != ps && player.BankedCash > highestOpponentCash)
            highestOpponentCash = player.BankedCash;
        
        int totalCash = ps.BankedCash + currentCash;
        int cashLead = totalCash - highestOpponentCash;
        float estimatedSnapRisk = EstimateSnapRisk(gs);
        
        // Log the current state for debugging
        Debug.Log($"AI for {ps.Player.Index}: Current Cash: {currentCash}, " +
                             $"Banked: {ps.BankedCash}, Cards Left: {remainingCards}, " +
                             $"Snap Risk: {estimatedSnapRisk:P2}, Cash Lead: {cashLead}");
               
        // 1. If we have nothing, always draw
        if (currentCash <= 0)
        {
            Debug.Log("AI: No cash to lose, drawing card");
            _cardsDrawnPerDay[gs.CurrentDay]++;
            return ActionType.DrawCard;
        }
        
        // 2. If we have a very comfortable lead and decent cash, bank it
        if (cashLead > LeadThreshold && currentCash >= HighCashThreshold)
        {
            Debug.Log("AI: Has good lead and high cash, banking");
            return ActionType.BankCash;
        }
        
        // 3. If the snap risk is high and we have a decent amount, consider banking
        if (estimatedSnapRisk > SnapRiskThreshold && currentCash >= LowCashThreshold)
        {
            // If we have a lot of cash or we're behind, be more cautious
            if (currentCash >= HighCashThreshold || cashLead < 0)
            {
                Debug.Log("AI: High snap risk with good cash, banking");
                return ActionType.BankCash;
            }
        }
        
        // 4. If we have drawn many cards this day, consider banking
        if (_cardsDrawnPerDay[gs.CurrentDay] >= 3 && currentCash >= LowCashThreshold)
        {
            // The more cards drawn, the more likely to bank
            float bankProbability = (_cardsDrawnPerDay[gs.CurrentDay] - 2) * 0.2f;
            if (Random.value < bankProbability)
            {
                Debug.Log($"AI: Drawn {_cardsDrawnPerDay[gs.CurrentDay]} cards, deciding to bank");
                return ActionType.BankCash;
            }
        }
        
        // 5. Default: Draw another card
        _cardsDrawnPerDay[gs.CurrentDay]++;
        _riskTakenPerDay[gs.CurrentDay]++;
        Debug.Log("AI: Taking a risk and drawing another card");
        return ActionType.DrawCard;
    }
    
    private float EstimateSnapRisk(GameState gs)
    {
        var deck = gs.CurrentDeck;
        return deck.SnapCount / (float)deck.Count;
    }
}
