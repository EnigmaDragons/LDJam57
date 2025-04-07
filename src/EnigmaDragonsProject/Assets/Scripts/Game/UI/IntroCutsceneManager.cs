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
    [SerializeField] private List<CutsceneScene> scenes;
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private float textFadeDuration = 0.5f;
    [SerializeField] private float textTypeSpeed = 0.05f;
    [SerializeField] private float sceneTransitionDuration = 0.5f;

    private int _currentSceneIndex = 0;
    private Sequence _currentSequence;
    private bool _isPlaying = false;

    private void Start()
    {
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

        // Activate current scene
        if (currentScene.backgroundObject != null)
        {
            currentScene.backgroundObject.SetActive(true);
            _currentSequence.Append(currentScene.backgroundObject.transform.DOScale(1.1f, sceneTransitionDuration)
                .From(1f)
                .SetEase(Ease.OutQuad));
        }

        // Play text lines
        foreach (var line in currentScene.textLines)
        {
            // Fade out previous text
            _currentSequence.Append(textCanvasGroup.DOFade(0, textFadeDuration));
            
            // Set new text and type it out
            _currentSequence.AppendCallback(() => {
                textDisplay.text = "";
                textDisplay.DOText(line, line.Length * textTypeSpeed)
                    .SetEase(Ease.Linear);
            });
            
            // Fade in new text
            _currentSequence.Append(textCanvasGroup.DOFade(1, textFadeDuration));
            
            // Wait before next line
            _currentSequence.AppendInterval(0.5f);
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