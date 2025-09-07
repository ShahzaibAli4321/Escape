using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller; // Drag your CharacterController here
    public Transform cameraTransform;      // Drag the Main Camera here
    public Animator animator;              // Drag your Animator here
    public Text ScoreText;                 // Drag the ScoreText here
    public AudioSource audio;
    public float speed = 10f;
    public float turnSmoothTime = 0.1f;
    int Score = 0;
    private float turnSmoothVelocity;

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("Forward", true);

            // Calculate target angle relative to camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate player to face movement direction
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move player in that direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Forward", false);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Coin"))
        {
            Score++;
            audio.Play();
            ScoreText.text = Score.ToString();

            Destroy(hit.gameObject);
        }
    }
}
