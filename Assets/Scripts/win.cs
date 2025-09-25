using UnityEngine;

public class win : MonoBehaviour
{
    public GameObject YouWin;
    public Transform dog;        // Reference to the dog object
    public float triggerDistance = 1f; // How close player must be to "win"

    private bool hasWon = false;

    void Start()
    {
        YouWin.SetActive(false);
    }

    void Update()
    {
        if (!hasWon && dog != null)
        {
            float distance = Vector3.Distance(transform.position, dog.position);

            if (distance <= triggerDistance)
            {
                WinGame();
            }
        }
    }

    private void WinGame()
    {
        hasWon = true;
        Time.timeScale = 0f;
        YouWin.SetActive(true);
    }
}
