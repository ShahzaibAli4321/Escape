using UnityEngine;

public class Boy1 : MonoBehaviour
{
    Animator animator;
    float velocityX = 0.0f;
    float velocityZ = 0.0f;

    public float acceleration = 2f;
    public float deceleration = 2f;
    public float maxVelocity = 0.5f;
    public Transform cameraTransform; // Assign your camera here in the Inspector

    void Start()
    {
        animator = GetComponent<Animator>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);

        // Calculate camera directions on ground plane
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Movement vector relative to camera
        Vector3 move = camForward * velocityZ + camRight * velocityX;

        if (move.magnitude > 0.01f)
        {
            transform.Translate(move * Time.deltaTime, Space.World);

            // Optional: rotate character toward movement direction
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(move),
                Time.deltaTime * 10f
            );
        }

        // Forward
        if (Input.GetKey(KeyCode.UpArrow) && velocityZ < maxVelocity)
            velocityZ += Time.deltaTime * acceleration;
        else if (!Input.GetKey(KeyCode.UpArrow) && velocityZ > 0)
            velocityZ -= Time.deltaTime * deceleration;

        // Backward
        if (Input.GetKey(KeyCode.DownArrow) && velocityZ > -maxVelocity)
            velocityZ -= Time.deltaTime * acceleration;
        else if (!Input.GetKey(KeyCode.DownArrow) && velocityZ < 0)
            velocityZ += Time.deltaTime * deceleration;

        // Left
        if (Input.GetKey(KeyCode.LeftArrow) && velocityX > -maxVelocity)
            velocityX -= Time.deltaTime * acceleration;
        else if (!Input.GetKey(KeyCode.LeftArrow) && velocityX < 0)
            velocityX += Time.deltaTime * deceleration;

        // Right
        if (Input.GetKey(KeyCode.RightArrow) && velocityX < maxVelocity)
            velocityX += Time.deltaTime * acceleration;
        else if (!Input.GetKey(KeyCode.RightArrow) && velocityX > 0)
            velocityX -= Time.deltaTime * deceleration;
    }
}
