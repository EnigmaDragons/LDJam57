using UnityEngine;
using System.Collections.Generic;

public class StartGame : MonoBehaviour
{
    [Tooltip("Starting cash amount for all players")]
    [SerializeField] private int startingCash = 0;
    
    [Tooltip("Whether to initialize the game when this script starts")]
    [SerializeField] private bool initializeOnStartIfNotInitialized = true;

    private void Awake()
    {
        if (initializeOnStartIfNotInitialized && CurrentGameState.ReadOnly == null || !CurrentGameState.ReadOnly.IsInitialized)
        {
            InitializeGame();
        }
    }

    public void InitializeGame()
    {
        var gameState = new GameState();
        var players = new List<PlayerState>();
        
        // Create 1 human player with Tabby Tom
        var humanPlayer = CreatePlayer(0, true);
        players.Add(humanPlayer);
        
        // Create 5 AI players, all with Tabby Tom
        for (int i = 1; i <= 5; i++)
        {
            var aiPlayer = CreatePlayer(i, false);
            players.Add(aiPlayer);
        }
        
        gameState.IsInitialized = true;
        gameState.PlayerStates = players.ToArray();
        gameState.CurrentDay = Day.Monday;
        gameState.BossState = new BossState(Bosses.DayBossMap[Day.Monday], 0);
        CurrentGameState.Init(gameState);
        
        Debug.Log($"Start Game: Game initialized with 1 human player and 5 AI players, all using Tabby Tom. Current day is {gameState.CurrentDay}, current boss is {gameState.BossState.Boss.Name}");
    }
    
    private PlayerState CreatePlayer(int index, bool isHuman)
    {
        var player = new Player(index, Characters.TabbyTom, isHuman ? PlayerType.Human : PlayerType.AI);
        var playerState = new PlayerState(player, startingCash, isHuman ? new NoAi() : new BasicRiskAwareAi());
        
        Debug.Log($"Start Game: Created player {index}: {(isHuman ? "Human" : "AI")} player using {Characters.TabbyTom.Name}");
        
        return playerState;
    }
}
