using UnityEngine;
using UnityEngine.UI;

public sealed class OnClickPlayUiSound : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private FloatReference volume = new FloatReference(1f);

    void Awake() => button.onClick.AddListener(() => player.Play(clickSound, volume));

    public void PlayClickSound() => player.Play(clickSound, volume);
    public void PlayHoverSound() => player.Play(hoverSound, volume);
}
