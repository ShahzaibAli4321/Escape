using UnityEngine;

public class Boy : MonoBehaviour
{
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            if (animator.GetBool("Forward") == false)
            {
                Debug.Log("Walking");
            }
            animator.SetBool("Forward", true);
        }

        if (!Input.GetKey(KeyCode.UpArrow))
        {
            if (animator.GetBool("Forward") == true)
            {
                Debug.Log("Not Walking");
            }
            animator.SetBool("Forward", false);
        }
    }
}
