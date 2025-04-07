using UnityEngine;

public class RotatePingPong : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private float intensity = 10f;
    [SerializeField] private float speed = 1f;
    
    private Quaternion initialRotation;
    
    private void Start()
    {
        initialRotation = transform.rotation;
    }
    
    private void Update()
    {
        float angle = Mathf.PingPong(Time.time * speed, intensity * 2) - intensity;
        transform.rotation = initialRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }
}
