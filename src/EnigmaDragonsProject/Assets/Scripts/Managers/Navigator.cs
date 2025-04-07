using UnityEngine;

[CreateAssetMenu]
public sealed class Navigator : ScriptableObject
{
    [SerializeField] private bool loggingEnabled;
    
    public void NavigateToMainMenu() => NavigateTo("MainMenu");
    public void NavigateToScene(string sceneName) => NavigateTo(sceneName);
    public void NavigateToCatPickerScene() => NavigateTo("CatPickerScene");

    private void NavigateTo(string sceneName)
    {
        if (loggingEnabled)
            Log.Info($"Navigating to {sceneName}");
        Message.Publish(new NavigateToSceneRequested(sceneName));
    }
}
