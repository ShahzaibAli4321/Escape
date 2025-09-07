using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    public GameObject coin;
    public Vector3 center;
    public Vector3 size;
    public Text ScoreText;
    private int counter = 1;
    private int lastCheckedScore = 0;

    void Start()
    {
        Spawn(10);
    }

    void Update()
    {
        int currentScore;
        if (int.TryParse(ScoreText.text, out currentScore))
        {
            // Check only when score changes
            if (currentScore != lastCheckedScore)
            {
                lastCheckedScore = currentScore;

                if (currentScore % 10 == 0 && currentScore != 0)
                {
                    counter++;
                    Debug.Log("Spawning " + (10 * counter) + " coins");
                    Spawn(10 * counter);
                }
            }
        }
    }

    void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-size.x / 2, size.x / 2),
                0.0f,
                Random.Range(-size.z / 2, size.z / 2)
            );

            Instantiate(coin, pos, Quaternion.identity);
        }
    }
}
