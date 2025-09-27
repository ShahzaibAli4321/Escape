using System;
using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public float[] position;   // x, y, z
    public bool isChasing;
}

[System.Serializable]
public class GameData
{
    public float[] playerPosition;
    public float[] dogPosition;
    public bool isFollowing;
    public List<EnemyData> enemies;
}
