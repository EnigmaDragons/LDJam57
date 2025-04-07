using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelectionUI : MonoBehaviour
{
    [Header("Character Display")]
    [SerializeField] private Image characterFaceImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI departmentNameText;
    [SerializeField] private TextMeshProUGUI powerNameText;
    [SerializeField] private TextMeshProUGUI powerDescriptionText;
    [SerializeField] private GameObject lockedIcon;
    [SerializeField] private GameObject continueButton;
    
    [Header("Navigation")]
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    
    private int _currentCharacterIndex = 0;
    private Character[] _allCharacters;
    private Character _selectedCharacter;
    
    private void Start()
    {
        // Get all characters from the Characters class
        _allCharacters = Characters.AllCharacters.ToArray();
        
        // Initialize UI
        UpdateCharacterDisplay();
        
        // Set up button listeners
        leftArrowButton.onClick.AddListener(ShowPreviousCharacter);
        rightArrowButton.onClick.AddListener(ShowNextCharacter);
        
        // Set up continue button
        continueButton.GetComponent<Button>().onClick.AddListener(StartGameWithSelectedCharacter);
    }
    
    private void UpdateCharacterDisplay()
    {
        if (_allCharacters == null || _allCharacters.Length == 0)
            return;
            
        // Ensure index is within bounds
        _currentCharacterIndex = (_currentCharacterIndex + _allCharacters.Length) % _allCharacters.Length;
        _selectedCharacter = _allCharacters[_currentCharacterIndex];
        
        // Update UI elements
        characterFaceImage.sprite = _selectedCharacter.Face;
        characterNameText.text = _selectedCharacter.Name;
        departmentNameText.text = _selectedCharacter.Department;
        powerNameText.text = _selectedCharacter.PowerName;
        powerDescriptionText.text = _selectedCharacter.PowerDescription;
        
        // ATTN: Jam Version... just unlock everyone for now
        bool isUnlocked = true;
        // Check if character is unlocked
        //bool isUnlocked = CharacterUnlockManager.IsCharacterUnlocked(_selectedCharacter.Name);
        lockedIcon.SetActive(!isUnlocked);
        continueButton.SetActive(isUnlocked);
        
        // Add some animation
        characterFaceImage.transform.DOScale(1.1f, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => characterFaceImage.transform.DOScale(1f, 0.2f));
    }
    
    private void ShowPreviousCharacter()
    {
        _currentCharacterIndex--;
        UpdateCharacterDisplay();
    }
    
    private void ShowNextCharacter()
    {
        _currentCharacterIndex++;
        UpdateCharacterDisplay();
    }
    
    private void StartGameWithSelectedCharacter()
    {
        if (_selectedCharacter == null)
            return;
            
        // Store the selected character for the game to use
        PlayerPrefs.SetString("SelectedCharacter", _selectedCharacter.Name);
        PlayerPrefs.Save();
        
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
} 