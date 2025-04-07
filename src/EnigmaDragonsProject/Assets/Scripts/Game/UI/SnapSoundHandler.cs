using UnityEngine;

public class SnapSoundHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<SnapsAddedToDeck>(OnSnapsAdded, this);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    
    private void OnSnapsAdded(SnapsAddedToDeck msg)
    {
        // Play a snap sound to emphasize the danger when snaps are added
        Message.Publish(new PlayUiSound(SoundType.SnapCard));
    }
} 