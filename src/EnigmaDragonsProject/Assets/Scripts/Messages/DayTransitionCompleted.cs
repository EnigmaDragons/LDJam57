using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("")]
public class DayTransitionCompletedDefinition : MonoBehaviour
{
    // This is just a dummy class to ensure the compiler recognizes the message class
    private void Awake()
    {
        // Self-destruct immediately
        Destroy(this);
    }
}

// This message is published when a day transition is completed
// and a new day is starting
public class DayTransitionCompleted
{
    public Day NewDay { get; }
    
    public DayTransitionCompleted(Day newDay)
    {
        NewDay = newDay;
    }
} 