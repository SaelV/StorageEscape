using UnityEngine;

public class MovPendulante : MonoBehaviour
{
    [Header("Pendulum Settings")]
    [Tooltip("Ángulo máximo de oscilación (grados)")]
    [SerializeField] private float maxAngle = 45f;
    [Tooltip("Velocidad de oscilación (ciclos por segundo)")]
    [SerializeField] private float speed = 1f;
    [Tooltip("Eje de rotación (por defecto Z)")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 0f, 1f);
    [Tooltip("Desfase inicial (en segundos)")]
    [SerializeField] private float phaseOffset = 0f;

    private Quaternion initialRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin((Time.time + phaseOffset) * speed * Mathf.PI * 2f) * maxAngle;
        transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }
}
