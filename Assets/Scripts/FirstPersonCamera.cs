using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Look Settings")]
    public Transform playerBody;        // Reference to the player object
    public float mouseSensitivity = 100f;

    [Header("Vertical Look Clamp")]
    public float minY = -80f;           // How far down the player can look
    public float maxY = 80f;            // How far up the player can look

    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor for immersion
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);

        // Apply vertical rotation to camera only
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation to player body
        playerBody.Rotate(Vector3.up * mouseX);

        if(Input.GetKey(KeyCode.Q))
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0.03f, 2.3f, 0.69f), 0.25f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0.0f, 3.17f, 0.378f), 0.25f);
        }
    }
}
