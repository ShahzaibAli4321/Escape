using UnityEngine;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    public Transform player;
    public Transform dog;
    public List<Enemy> enemies;   // drag enemies here in Inspector

    void Update()
    {
        // Testing keybinds
        if (Input.GetKeyDown(KeyCode.Alpha1)) Save("slot1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) Save("slot2");

        if (Input.GetKeyDown(KeyCode.F1)) Load("slot1");
        if (Input.GetKeyDown(KeyCode.F2)) Load("slot2");
        if (Input.GetKeyDown(KeyCode.L))  LoadCheckpoint();
    }

    public void Save(string slotName)
    {
        GameData data = new GameData
        {
            playerPosition = new float[] { player.position.x, player.position.y, player.position.z },
            dogPosition = new float[] { dog.position.x, dog.position.y, dog.position.z },
            enemies = new List<EnemyData>()
        };

        foreach (Enemy e in enemies)
        {
            EnemyData ed = new EnemyData
            {
                position = new float[] { e.transform.position.x, e.transform.position.y, e.transform.position.z },
                isChasing = e.isChasing
            };
            data.enemies.Add(ed);
        }

        SaveSystem.Save(data, slotName);
        Debug.Log("Game Saved in slot: " + slotName);
    }

    public void Load(string slotName)
    {
        GameData data = SaveSystem.Load(slotName);
        if (data == null)
        {
            Debug.Log("No save file found in slot: " + slotName);
            return;
        }

        // Restore player
        // If the player has a CharacterController, disable it before teleporting
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.position = new Vector3(
            data.playerPosition[0],
            data.playerPosition[1],
            data.playerPosition[2]
        );

        // Re-enable the controller after setting the position
        if (controller != null) controller.enabled = true;

        Debug.Log("Player moved to: " + player.position);

        //player.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);

        // Restore dog
        dog.position = new Vector3(data.dogPosition[0], data.dogPosition[1], data.dogPosition[2]);

        // Restore enemies
        for (int i = 0; i < data.enemies.Count && i < enemies.Count; i++)
        {
            EnemyData ed = data.enemies[i];
            enemies[i].transform.position = new Vector3(ed.position[0], ed.position[1], ed.position[2]);
            enemies[i].isChasing = ed.isChasing;

            // Optional: restart chase if loaded in chasing state
            if (ed.isChasing)
            {
                enemies[i].isChasing = true;
            }
            else
            {
                enemies[i].isChasing = false;
            }
        }

        Debug.Log("Game Loaded from slot: " + slotName);
    }

    public void LoadCheckpoint()
    {
        Load("checkpoint");
    }

}
