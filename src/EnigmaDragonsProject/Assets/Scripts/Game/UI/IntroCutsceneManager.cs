using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

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
            sceneName = "Title Screen",
            setupInstructions = "WHISKR logo centered, dark background, subtle glow effect",
            textLines = new List<string> { "DEEP MEOWGOTIATIONS" },
            duration = 2f
        },
        new CutsceneScene
        {
            sceneName = "Company Introduction",
            setupInstructions = "WHISKR office building exterior, daytime, corporate setting",
            textLines = new List<string> 
            { 
                "Welcome to WHISKR",
                "The most PAW-some social media company in the world!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "Product Showcase",
            setupInstructions = "Cat using smartphone, app interface visible, happy cat expression",
            textLines = new List<string> 
            { 
                "Every cat with a smartphone uses Whiskr",
                "to share their napping spots and cute selfies!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "Project Announcement",
            setupInstructions = "SnapCat app mockup, floating UI elements, modern design",
            textLines = new List<string> 
            { 
                "Your team's MEOW-velous new project",
                "SnapCat - The next big thing in feline social media!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "Conflict Setup",
            setupInstructions = "Money pile with cat paws reaching, dramatic lighting",
            textLines = new List<string> 
            { 
                "But there's a cat-astrophe brewing!",
                "Six department teams are all fighting over the budget!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "CEO Introduction",
            setupInstructions = "Serious cat in business suit, imposing pose, office background",
            textLines = new List<string> 
            { 
                "The CEO has approved your project...",
                "But he's watching your every move!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            sceneName = "Game Rules",
            setupInstructions = "Card being drawn, Snap card visible, dramatic angle",
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
            sceneName = "Risk vs Reward",
            setupInstructions = "Split screen: safe small pile vs risky large pile of money",
            textLines = new List<string> 
            { 
                "Will you play it safe with a small pile of coins?",
                "Or risk it all for a kitty bank that overflows?"
            },
            duration = 4f
        },
        new CutsceneScene
        {
            sceneName = "Final Warning",
            setupInstructions = "Your character looking determined, office background",
            textLines = new List<string> 
            { 
                "Remember: In the corporate cat world...",
                "Fortune favors the BOLD...",
                "But curiosity killed the quarterly bonus!"
            },
            duration = 4f
        },
        new CutsceneScene
        {
            sceneName = "Call to Action",
            setupInstructions = "WHISKR logo with dramatic lighting, motivational angle",
            textLines = new List<string> 
            { 
                "GOOD LUCK, BUSINESS CAT!",
                "Make your department purr-oud!"
            },
            duration = 3f
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
    [SerializeField] private float lineSpacing = 1.2f;
    
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
            
            // Fade out previous text
            _currentSequence.Append(textCanvasGroup.DOFade(0, textFadeDuration));
            
            // Set new text and type it out
            _currentSequence.AppendCallback(() => {
                Debug.Log($"[IntroCutscene] Starting to type line: {line}");
                textDisplay.text = "";
                textDisplay.rectTransform.anchoredPosition = new Vector2(0, -i * lineSpacing * textDisplay.fontSize);
                
                // Type out the text with sound
                var typeSequence = DOTween.Sequence();
                for (int j = 0; j < line.Length; j++)
                {
                    int currentIndex = j;
                    typeSequence.AppendCallback(() => {
                        textDisplay.text = line.Substring(0, currentIndex + 1);
                        if (typeSound != null)
                            _audioSource.PlayOneShot(typeSound);
                    });
                    typeSequence.AppendInterval(textTypeSpeed);
                }
            });
            
            // Fade in new text
            _currentSequence.Append(textCanvasGroup.DOFade(1, textFadeDuration));
            
            // Wait before next line
            _currentSequence.AppendInterval(0.3f);
        }

        // Wait for scene duration
        Debug.Log($"[IntroCutscene] Waiting for scene duration: {currentScene.duration}s");
        _currentSequence.AppendInterval(currentScene.duration);

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