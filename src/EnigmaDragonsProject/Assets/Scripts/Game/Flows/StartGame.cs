using UnityEngine;
using System.Collections.Generic;

public class StartGame : MonoBehaviour
{
    [Tooltip("Starting cash amount for all players")]
    [SerializeField] private int startingCash = 0;
    
    [Tooltip("Whether to initialize the game when this script starts")]
    [SerializeField] private bool initializeOnStart = true;

    private void Start()
    {
        if (initializeOnStart)
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
        
        gameState.PlayerStates = players.ToArray();
        CurrentGameState.Init(gameState);
        
        Debug.Log("Game initialized with 1 human player and 5 AI players, all using Tabby Tom");
    }
    
    private PlayerState CreatePlayer(int index, bool isHuman)
    {
        var player = new Player(index, Characters.TabbyTom);
        var playerState = new PlayerState(player, startingCash);
        
        Debug.Log($"Created player {index}: {(isHuman ? "Human" : "AI")} player using {Characters.TabbyTom.Name}");
        
        return playerState;
    }
}
