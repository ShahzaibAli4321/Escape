using UnityEngine;

public class Boy : MonoBehaviour
{
    Animator animator;
    float velocityX = 0.0f;
    float velocityZ = 0.0f;
    public float acceleration = 2f;
    public float deceleration = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Velocity X", velocityX);
        animator.SetFloat("Velocity Z", velocityZ);
        transform.Translate(new Vector3(velocityX, 0, velocityZ) * Time.deltaTime * 20f);

        // Forward Movement
        if (Input.GetKey(KeyCode.UpArrow) && velocityZ <= 0.5f)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (!Input.GetKey(KeyCode.UpArrow) && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }


        // Backwards movement
        if (Input.GetKey(KeyCode.DownArrow) && velocityZ >= -0.5f)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        if (!Input.GetKey(KeyCode.DownArrow) && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }


        // Left movement
        if (Input.GetKey(KeyCode.LeftArrow) && velocityX >= -0.5f)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (!Input.GetKey(KeyCode.LeftArrow) && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        // Right movement
        if (Input.GetKey(KeyCode.RightArrow) && velocityX <= 0.5f)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        if (!Input.GetKey(KeyCode.RightArrow) && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }
}
