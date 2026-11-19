
using System.Collections.Generic;
using UnityEngine;

public enum LevelGoal
{
    ReachExit,
    CollectCoins,
    Survive
}

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int count;
    public float delay;
}

[CreateAssetMenu(
    fileName = "LevelData",
    menuName = "Coin Adventure/Level Data"
)]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int levelNumber;
    public float timeLimit;

    [Header("Level Goal")]
    public LevelGoal goal;
    public int goalAmount;

    [Header("Environment")]
    public GameObject environmentPrefab;

    [Header("Player")]
    public Vector2 playerSpawnPoint;

    [Header("Finish")]
    public Vector2 exitPosition;


    [Header("Enemy Spawn Points")]
    public Vector2[] spawnPoints;

    [Header("Enemy Waves")]
    public List<EnemyWave> waves = new();
}
