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

    public LayerMask collisionMask;  // Layers the camera should collide with

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

        // Check for collisions
        RaycastHit hit;
        if (Physics.Linecast(target.position + Vector3.up * 3.5f, desiredPosition, out hit, collisionMask))
        {
            // If something is blocking, move camera closer to hit point
            desiredPosition = hit.point + hit.normal * 0.2f;
        }

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 3.5f); // Slightly above target’s center
    }
}





//using UnityEngine;

//public class Camera_Movement : MonoBehaviour
//{
//    [Header("Target Settings")]
//    public Transform target;        // The player or object the camera follows
//    public Vector3 offset = new Vector3(0, 4, -6); // Default offset position

//    [Header("Camera Controls")]
//    public float rotationSpeed = 3.0f;  // Mouse sensitivity
//    public float minY = -20f;           // Min vertical angle
//    public float maxY = 60f;            // Max vertical angle

//    private float currentX = 0f;
//    private float currentY = 0f;

//    public LayerMask collisionMask;  // Layers the camera should collide with

//    // New variable for smoothing
//    private Vector3 pivotSmooth;

//    void LateUpdate()
//    {
//        if (target == null) return;

//        // --- Smooth pivot follow (stops snapping on jump) ---
//        Vector3 pivotTarget = target.position + Vector3.up * 1.5f;
//        pivotSmooth = Vector3.Lerp(pivotSmooth, pivotTarget, Time.deltaTime * 2.5f);

//        Vector3 lookTarget = target.position + Vector3.up * 2f; // around head height

//        // Get mouse input
//        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
//        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
//        currentY = Mathf.Clamp(currentY, minY, maxY);

//        // Calculate rotation
//        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

//        // Desired camera position
//        Vector3 desiredPosition = pivotSmooth + rotation * offset;

//        // Collision check
//        RaycastHit hit;
//        Vector3 camDirection = (desiredPosition - pivotSmooth).normalized;
//        float camDistance = (desiredPosition - pivotSmooth).magnitude;

//        if (Physics.Raycast(pivotSmooth, camDirection, out hit, camDistance, collisionMask))
//        {
//            // Slide camera closer along the same line
//            desiredPosition = target.position + camDirection * (hit.distance - 0.2f);
//        }

//        // Apply final transform
//        transform.position = desiredPosition;
//        transform.LookAt(lookTarget); // look slightly above the character

//    }
//}
