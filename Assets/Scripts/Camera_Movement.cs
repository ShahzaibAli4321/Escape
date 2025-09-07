using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;        // The player or object the camera follows
    public Vector3 offset = new Vector3(0, 4, -12); // Default offset position

    [Header("Camera Controls")]
    public float rotationSpeed = 3.0f;  // Mouse sensitivity
    public float minY = -20f;           // Min vertical angle
    public float maxY = 60f;            // Max vertical angle

    private float currentX = 0f;
    private float currentY = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Get mouse input
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp vertical rotation
        currentY = Mathf.Clamp(currentY, minY, maxY);

        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Set camera position and look at target
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // Slightly above target’s center
    }
}
