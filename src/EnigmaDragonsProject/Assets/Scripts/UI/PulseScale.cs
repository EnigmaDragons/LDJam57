using UnityEngine;

public class PulseScale : MonoBehaviour
{
    [SerializeField] private float pulseFactor = 0.15f;
    [SerializeField] private float pulseSpeed = 1f;
    
    private Vector3 originalScale;
    private float time;
    
    private void Start()
    {
        originalScale = transform.localScale;
    }
    
    private void Update()
    {
        time += Time.deltaTime * pulseSpeed;
        float pulseMagnitude = Mathf.Sin(time) * pulseFactor;
        transform.localScale = originalScale * (1f + pulseMagnitude);
    }
}