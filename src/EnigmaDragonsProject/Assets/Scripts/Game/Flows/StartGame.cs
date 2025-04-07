using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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
        
        // Create a list of random characters (excluding the selected character)
        var availableCharacters = new List<Character>(Characters.AllCharacters);
        string selectedCharacterName = PlayerPrefs.GetString("SelectedCharacter", "Tabby Tom");
        var selectedCharacter = Characters.AllCharacters.FirstOrDefault(c => c.Name == selectedCharacterName);
        if (selectedCharacter == null)
            selectedCharacter = Characters.TabbyTom;
            
        availableCharacters.Remove(selectedCharacter);
        ShuffleList(availableCharacters);
        
        // Create 1 human player with the selected character
        var humanPlayer = CreatePlayer(0, true, selectedCharacter);
        players.Add(humanPlayer);
        
        // Create 5 AI players, each with a unique random character
        for (int i = 1; i <= 5; i++)
        {
            var character = availableCharacters[i - 1]; // Get a character from the shuffled list
            var aiPlayer = CreatePlayer(i, false, character);
            players.Add(aiPlayer);
        }
        
        gameState.IsInitialized = true;
        gameState.PlayerStates = players.ToArray();
        gameState.CurrentDay = Day.Monday;
        gameState.BossState = new BossState(Bosses.DayBossMap[Day.Monday], 0);
        CurrentGameState.Init(gameState);
        
        // Play deck shuffled sound when game initializes
        Message.Publish(new PlayUiSound(SoundType.DeckShuffled));
        
        Debug.Log($"Start Game: Game initialized with 1 human player using {selectedCharacter.Name} and 5 AI players with unique characters. Current day is {gameState.CurrentDay}, current boss is {gameState.BossState.Boss.Name}");
    }
    
    private PlayerState CreatePlayer(int index, bool isHuman, Character character)
    {
        var player = new Player(index, character, isHuman ? PlayerType.Human : PlayerType.AI);
        var playerState = new PlayerState(player, startingCash, isHuman ? new NoAi() : new BasicRiskAwareAi());
        
        Debug.Log($"Start Game: Created player {index}: {(isHuman ? "Human" : "AI")} player using {character.Name}");
        
        return playerState;
    }
    
    // Fisher-Yates shuffle algorithm
    private void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random(Guid.NewGuid().GetHashCode());
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
