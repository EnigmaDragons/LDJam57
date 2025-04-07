using UnityEngine;

public enum SoundType
{
    DeckShuffled = 0,
    CardDraw = 1,
    CardFlip = 2,
    HappyCard = 3,
    SnapCard = 4,
}

public class SoundGuy : OnMessage<PlayUiSound>
{
    [SerializeField] private UiSfxPlayer sfxPlayer;
    [SerializeField] private AudioClipVolume deckShuffled;
    [SerializeField] private AudioClipVolume cardDraw;
    [SerializeField] private AudioClipVolume cardFlip;
    [SerializeField] private AudioClipVolume happyCard;
    [SerializeField] private AudioClipVolume snapCard;
    
    protected override void Execute(PlayUiSound msg)
    {
        switch (msg.SoundType)
        {
            case SoundType.DeckShuffled:
                sfxPlayer.Play(deckShuffled);
                break;
            case SoundType.CardDraw:
                sfxPlayer.Play(cardDraw);
                break;
            case SoundType.CardFlip:
                sfxPlayer.Play(cardFlip);
                break;
            case SoundType.HappyCard:
                sfxPlayer.Play(happyCard);
                break;
            case SoundType.SnapCard:
                sfxPlayer.Play(snapCard);
                break;
            default:
                Debug.LogWarning($"Unknown sound type: {msg.SoundType}");
                break;
        }
    }
}
