using UnityEngine;
using System.Collections.Generic;

public class CharacterUnlockManager : MonoBehaviour
{
    private const string UNLOCKED_CHARACTERS_KEY = "UnlockedCharacters";
    private static HashSet<string> _unlockedCharacters;

    private void Awake()
    {
        LoadUnlockedCharacters();
    }

    private void LoadUnlockedCharacters()
    {
        _unlockedCharacters = new HashSet<string>();
        string unlockedChars = PlayerPrefs.GetString(UNLOCKED_CHARACTERS_KEY, "");
        
        if (!string.IsNullOrEmpty(unlockedChars))
        {
            string[] chars = unlockedChars.Split(',');
            foreach (string charName in chars)
            {
                if (!string.IsNullOrEmpty(charName))
                    _unlockedCharacters.Add(charName);
            }
        }
        
        // Always unlock Tabby Tom by default
        if (!_unlockedCharacters.Contains("Tabby Tom"))
        {
            _unlockedCharacters.Add("Tabby Tom");
            SaveUnlockedCharacters();
        }
    }

    private static void SaveUnlockedCharacters()
    {
        string chars = string.Join(",", _unlockedCharacters);
        PlayerPrefs.SetString(UNLOCKED_CHARACTERS_KEY, chars);
        PlayerPrefs.Save();
    }

    public static bool IsCharacterUnlocked(string characterName)
    {
        return _unlockedCharacters.Contains(characterName);
    }

    public static void UnlockCharacter(string characterName)
    {
        if (!_unlockedCharacters.Contains(characterName))
        {
            _unlockedCharacters.Add(characterName);
            string chars = string.Join(",", _unlockedCharacters);
            PlayerPrefs.SetString(UNLOCKED_CHARACTERS_KEY, chars);
            PlayerPrefs.Save();
        }
    }

    public static void ResetUnlocks()
    {
        _unlockedCharacters.Clear();
        _unlockedCharacters.Add("Tabby Tom"); // Always keep Tabby Tom unlocked
        SaveUnlockedCharacters();
    }
} 