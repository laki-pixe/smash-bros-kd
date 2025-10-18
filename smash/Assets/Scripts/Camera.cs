using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [Header("Jugadores")]
    public Transform player1;
    public Transform player2;

    [Header("Configuración de cámara")]
    public float smoothSpeed = 0.2f;      // Qué tan rápido sigue la cámara
    public float minZoom = 5f;            // Zoom mínimo
    public float maxZoom = 12f;           // Zoom máximo
    public float zoomLimiter = 5f;        // Qué tan rápido cambia el zoom

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint;
        newPosition.z = transform.position.z; // Mantén la profundidad actual

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothSpeed);
    }

    void Zoom()
    {
        float distance = GetGreatestDistance();
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        return Vector3.Distance(player1.position, player2.position);
    }

    Vector3 GetCenterPoint()
    {
        return (player1.position + player2.position) / 2f;
    }
}
