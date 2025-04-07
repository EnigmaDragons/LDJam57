using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq; // Add this for Sum extension method

public class IntroCutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneScene
    {
        public string sceneName;
        public string setupInstructions;
        public List<string> textLines;
        public float duration;
    }

    [Header("Scene Configuration")]
    [SerializeField] private List<CutsceneScene> scenes = new List<CutsceneScene>
    {
        new CutsceneScene
        {
            sceneName = "Welcome to WHISKR",
            setupInstructions = "WHISKR logo centered, dark background, subtle glow effect",
            textLines = new List<string> 
            { 
                "Welcome to WHISKR",
                "The most PAW-some social media company in the world!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "The Project",
            setupInstructions = "Innovation lab with SnapCat mockups, modern tech environment",
            textLines = new List<string> 
            { 
                "Your team's MEOW-velous new project",
                "SnapCat - The next big thing in feline social media!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "The Challenge",
            setupInstructions = "Boardroom with money pile, dramatic lighting",
            textLines = new List<string> 
            { 
                "But there's a cat-astrophe brewing!",
                "Six department teams are fighting over the budget!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "The Game",
            setupInstructions = "Conference room with cards and money, dramatic angle",
            textLines = new List<string> 
            { 
                "Draw cards to collect cash for your department",
                "But if the CEO catches you being too greedy...",
                "You'll lose everything you've collected that day!"
            },
            duration = 4f
        },
        new CutsceneScene
        {
            sceneName = "The Call to Action",
            setupInstructions = "Grand auditorium with dramatic lighting",
            textLines = new List<string> 
            { 
                "Will you play it safe with a small pile of coins?",
                "Or risk it all for a kitty bank that overflows?",
                "GOOD LUCK, BUSINESS CAT!",
                "Make your department purr-oud!"
            },
            duration = 4f
        }
    };

    [Header("Scene Objects")]
    [SerializeField] private Transform sceneContainer;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private RectTransform textContainer;
    
    [Header("Animation Settings")]
    [SerializeField] private float textFadeDuration = 0.3f;
    [SerializeField] private float textTypeSpeed = 0.02f;
    [SerializeField] private float sceneTransitionDuration = 0.5f;
    [SerializeField] private float textDisplayDelay = 1.5f; // Time to display text before fading out
    
    [Header("Audio")]
    [SerializeField] private AudioClip typeSound;
    [SerializeField] private AudioClip sceneTransitionSound;
    
    private List<GameObject> _sceneObjects;
    private int _currentSceneIndex = 0;
    private Sequence _currentSequence;
    private bool _isPlaying = false;
    private AudioSource _audioSource;

    private void Start()
    {
        Debug.Log("[IntroCutscene] Initializing cutscene manager");
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.Log("[IntroCutscene] Adding AudioSource component");
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
            
        // Initialize all scenes as inactive
        _sceneObjects = new List<GameObject>();
        for (int i = 0; i < sceneContainer.childCount; i++)
        {
            Transform child = sceneContainer.GetChild(i);
            if (child != null)
            {
                GameObject sceneObject = child.gameObject;
                _sceneObjects.Add(sceneObject);
                sceneObject.SetActive(false);
                Debug.Log($"[IntroCutscene] Deactivated scene object: {sceneObject.name}");
            }
            else
            {
                Debug.LogWarning("[IntroCutscene] Found null child in scene container!");
            }
        }
        Debug.Log($"[IntroCutscene] Initialized {_sceneObjects.Count} scene objects");
        
        // Initialize text display
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f; // Start invisible
            Debug.Log("[IntroCutscene] Initialized text canvas group alpha to 0");
        }
        else
        {
            Debug.LogError("[IntroCutscene] Text Canvas Group is not assigned!");
        }
        
        // Start the cutscene
        PlayCutscene();
    }

    public void PlayCutscene()
    {
        if (_isPlaying)
        {
            Debug.Log("[IntroCutscene] Cutscene already playing, ignoring PlayCutscene call");
            return;
        }
        
        Debug.Log("[IntroCutscene] Starting cutscene");
        _isPlaying = true;
        _currentSceneIndex = 0;
            
        PlayNextScene();
    }

    private void PlayNextScene()
    {
        if (_currentSceneIndex >= scenes.Count)
        {
            Debug.Log("[IntroCutscene] Reached end of scenes, ending cutscene");
            EndCutscene();
            return;
        }

        var currentScene = scenes[_currentSceneIndex];
        Debug.Log($"[IntroCutscene] Playing scene {_currentSceneIndex + 1}/{scenes.Count}: {currentScene.sceneName}");
        
        // Kill any existing sequence
        if (_currentSequence != null)
        {
            Debug.Log("[IntroCutscene] Killing existing sequence");
            _currentSequence.Kill();
        }

        // Create new sequence
        _currentSequence = DOTween.Sequence();
        Debug.Log("[IntroCutscene] Created new sequence");

        // Play transition sound
        if (sceneTransitionSound != null)
        {
            Debug.Log("[IntroCutscene] Playing transition sound");
            _audioSource.PlayOneShot(sceneTransitionSound);
        }

        // Activate current scene
        if (_currentSceneIndex < _sceneObjects.Count && _sceneObjects[_currentSceneIndex] != null)
        {
            var currentSceneObject = _sceneObjects[_currentSceneIndex];
            Debug.Log($"[IntroCutscene] Activating scene object: {currentSceneObject.name}");
            currentSceneObject.SetActive(true);
            _currentSequence.Append(currentSceneObject.transform.DOScale(1.1f, sceneTransitionDuration)
                .From(1f)
                .SetEase(Ease.OutQuad));
        }
        else
        {
            Debug.LogWarning($"[IntroCutscene] No scene object found for index {_currentSceneIndex}");
        }

        // Play text lines
        Debug.Log($"[IntroCutscene] Playing {currentScene.textLines.Count} text lines");
        for (int i = 0; i < currentScene.textLines.Count; i++)
        {
            var line = currentScene.textLines[i];
            Debug.Log($"[IntroCutscene] Queueing text line {i + 1}: {line}");
            
            // Set new text and type it out
            _currentSequence.AppendCallback(() => {
                Debug.Log($"[IntroCutscene] Starting to type line: {line}");
                textDisplay.text = "";
            });

            // Fade in before typing starts
            _currentSequence.Append(textCanvasGroup.DOFade(1, textFadeDuration));
            
            // Type out the text with sound
            float typeDuration = line.Length * textTypeSpeed;
            _currentSequence.Append(DOTween.To(
                () => 0,
                (progress) => {
                    // Use linear interpolation for consistent speed
                    int charsToShow = Mathf.Min(line.Length, Mathf.FloorToInt(progress * line.Length));
                    textDisplay.text = line.Substring(0, charsToShow);
                    if (typeSound != null && charsToShow > 0 && charsToShow <= line.Length)
                        _audioSource.PlayOneShot(typeSound);
                },
                1f,
                typeDuration
            ).SetEase(Ease.Linear)); // Force linear easing for consistent speed

            // Add the configurable delay after typing
            _currentSequence.AppendInterval(textDisplayDelay);

            // Fade out text (for all lines except the last one)
            if (i < currentScene.textLines.Count - 1)
            {
                _currentSequence.Append(textCanvasGroup.DOFade(0, textFadeDuration));
                _currentSequence.AppendInterval(0.1f); // Small pause between fade out and in
            }
        }

        // Wait for scene duration (minus time already spent on text)
        float textTime = currentScene.textLines.Count * (textFadeDuration * 2 + textDisplayDelay) + 
                        currentScene.textLines.Sum(line => line.Length) * textTypeSpeed;
        float remainingDuration = Mathf.Max(0.5f, currentScene.duration - textTime);
        Debug.Log($"[IntroCutscene] Waiting for remaining duration: {remainingDuration}s");
        _currentSequence.AppendInterval(remainingDuration);

        // Add the configurable delay before final fadeout
        _currentSequence.AppendInterval(textDisplayDelay);

        // Fade out text
        _currentSequence.Append(textCanvasGroup.DOFade(0, textFadeDuration));

        // Deactivate current scene
        if (_currentSceneIndex < _sceneObjects.Count && _sceneObjects[_currentSceneIndex] != null)
        {
            var currentSceneObject = _sceneObjects[_currentSceneIndex];
            Debug.Log($"[IntroCutscene] Deactivating scene object: {currentSceneObject.name}");
            _currentSequence.Append(currentSceneObject.transform.DOScale(1f, sceneTransitionDuration)
                .SetEase(Ease.InQuad));
            _currentSequence.AppendCallback(() => currentSceneObject.SetActive(false));
        }

        // Move to next scene
        _currentSequence.OnComplete(() => {
            Debug.Log($"[IntroCutscene] Completed scene {_currentSceneIndex + 1}, moving to next");
            _currentSceneIndex++;
            PlayNextScene();
        });
    }

    private void EndCutscene()
    {
        Debug.Log("[IntroCutscene] Ending cutscene");
        _isPlaying = false;
        
        // Load the character selection scene
        Debug.Log("[IntroCutscene] Loading MainMenu scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Debug.Log("[IntroCutscene] Skip requested by user");
            if (_currentSequence != null)
            {
                Debug.Log("[IntroCutscene] Completing and killing current sequence");
                _currentSequence.Complete();
                _currentSequence.Kill();
            }
            EndCutscene();
        }
    }
} 