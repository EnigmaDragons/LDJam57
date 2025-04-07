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
        public GameObject backgroundObject;
        public List<string> textLines;
        public float duration;
    }

    [Header("Scene Configuration")]
    [SerializeField] private List<CutsceneScene> scenes = new List<CutsceneScene>
    {
        new CutsceneScene
        {
            textLines = new List<string> { "DEEP MEOWGOTIATIONS" },
            duration = 2f
        },
        new CutsceneScene
        {
            textLines = new List<string> 
            { 
                "Welcome to WHISKR",
                "The most PAW-some social media company in the world!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            textLines = new List<string> 
            { 
                "Every cat with a smartphone uses Whiskr",
                "to share their napping spots and cute selfies!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            textLines = new List<string> 
            { 
                "Your team's MEOW-velous new project",
                "SnapCat - The next big thing in feline social media!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            textLines = new List<string> 
            { 
                "But there's a cat-astrophe brewing!",
                "Six department teams are all fighting over the budget!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
            textLines = new List<string> 
            { 
                "The CEO has approved your project...",
                "But he's watching your every move!"
            },
            duration = 3f
        },
        new CutsceneScene
        {
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
            textLines = new List<string> 
            { 
                "Will you play it safe with a small pile of coins?",
                "Or risk it all for a kitty bank that overflows?"
            },
            duration = 4f
        },
        new CutsceneScene
        {
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
            textLines = new List<string> 
            { 
                "GOOD LUCK, BUSINESS CAT!",
                "Make your department purr-oud!"
            },
            duration = 3f
        }
    };

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

    private int _currentSceneIndex = 0;
    private Sequence _currentSequence;
    private bool _isPlaying = false;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
            
        // Initialize all scenes as inactive
        foreach (var scene in scenes)
        {
            if (scene.backgroundObject != null)
                scene.backgroundObject.SetActive(false);
        }
        
        // Start the cutscene
        PlayCutscene();
    }

    public void PlayCutscene()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        _currentSceneIndex = 0;
            
        PlayNextScene();
    }

    private void PlayNextScene()
    {
        if (_currentSceneIndex >= scenes.Count)
        {
            EndCutscene();
            return;
        }

        var currentScene = scenes[_currentSceneIndex];
        
        // Kill any existing sequence
        if (_currentSequence != null)
            _currentSequence.Kill();

        // Create new sequence
        _currentSequence = DOTween.Sequence();

        // Play transition sound
        if (sceneTransitionSound != null)
            _audioSource.PlayOneShot(sceneTransitionSound);

        // Activate current scene
        if (currentScene.backgroundObject != null)
        {
            currentScene.backgroundObject.SetActive(true);
            _currentSequence.Append(currentScene.backgroundObject.transform.DOScale(1.1f, sceneTransitionDuration)
                .From(1f)
                .SetEase(Ease.OutQuad));
        }

        // Play text lines
        for (int i = 0; i < currentScene.textLines.Count; i++)
        {
            var line = currentScene.textLines[i];
            
            // Fade out previous text
            _currentSequence.Append(textCanvasGroup.DOFade(0, textFadeDuration));
            
            // Set new text and type it out
            _currentSequence.AppendCallback(() => {
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
        _currentSequence.AppendInterval(currentScene.duration);

        // Fade out text
        _currentSequence.Append(textCanvasGroup.DOFade(0, textFadeDuration));

        // Deactivate current scene
        if (currentScene.backgroundObject != null)
        {
            _currentSequence.Append(currentScene.backgroundObject.transform.DOScale(1f, sceneTransitionDuration)
                .SetEase(Ease.InQuad));
            _currentSequence.AppendCallback(() => currentScene.backgroundObject.SetActive(false));
        }

        // Move to next scene
        _currentSequence.OnComplete(() => {
            _currentSceneIndex++;
            PlayNextScene();
        });
    }

    private void EndCutscene()
    {
        _isPlaying = false;
        
        // Load the character selection scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // Optional: Add skip functionality
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (_currentSequence != null)
            {
                _currentSequence.Complete();
                _currentSequence.Kill();
            }
            EndCutscene();
        }
    }
} 