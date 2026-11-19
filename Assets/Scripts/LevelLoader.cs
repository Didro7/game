
using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelData[] levels;

#if UNITY_EDITOR
    [Header("Testing Only")]
    [SerializeField] private int testLevelIndex = -1;
#endif


    [Header("Scene Objects")]
    [SerializeField] private Transform player;

    [SerializeField] private Transform finish;


    [SerializeField] private GameTimer gameTimer;


    public LevelData CurrentLevel { get; private set; }

    public int CurrentIndex { get; private set; }

    public int LevelCount =>
        levels == null ? 0 : levels.Length;

    private IEnumerator Start()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No levels assigned!");
            yield break;
        }

        if (player == null)
        {
            Debug.LogError("Player is not assigned!");
            yield break;
        }



        int index = GameSession.levelIndex;


        if (index < 0 || index >= levels.Length)
        {
            Debug.LogError("Invalid level index!");
            yield break;
        }

        LevelData level = levels[index];

        if (level == null)
        {
            Debug.LogError("LevelData is missing!");
            yield break;
        }

        CurrentLevel = level;
        CurrentIndex = index;



        if (finish != null)
        {
            Vector3 finishPos = finish.position;

            finishPos.x = level.exitPosition.x;
            finishPos.y = level.exitPosition.y;

            finish.position = finishPos;
        }
        else
        {
            Debug.LogWarning(
                "Finish is not assigned in LevelLoader!"
            );
        }


        Time.timeScale = 1f;

        if (level.environmentPrefab != null)
        {
            Instantiate(
                level.environmentPrefab,
                Vector3.zero,
                Quaternion.identity
            );
        }

        Vector3 playerPosition = player.position;

        playerPosition.x = level.playerSpawnPoint.x;
        playerPosition.y = level.playerSpawnPoint.y;

        player.position = playerPosition;

        Rigidbody2D playerRb =
            player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Loading Level " + level.levelNumber);

        if (gameTimer != null)
        {
            gameTimer.Begin(level.timeLimit);
        }
        else
        {
            Debug.LogWarning("GameTimer is not assigned!");
        }


        if (level.spawnPoints == null ||
            level.spawnPoints.Length == 0)
        {
            Debug.LogWarning("No enemy spawn points!");
            yield break;
        }

        int spawnIndex = 0;

        foreach (EnemyWave wave in level.waves)
        {
            if (wave == null || wave.enemyPrefab == null)
            {
                continue;
            }

            yield return new WaitForSeconds(
                Mathf.Max(0f, wave.delay)
            );

            for (int i = 0; i < wave.count; i++)
            {
                Vector2 spawnPosition =
                    level.spawnPoints[
                        spawnIndex % level.spawnPoints.Length
                    ];

                spawnIndex++;

                GameObject enemy = Instantiate(
                    wave.enemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                GoblinPatrol[] goblins =
                    enemy.GetComponentsInChildren<GoblinPatrol>(true);

                foreach (GoblinPatrol goblin in goblins)
                {
                    goblin.SetPlayer(player);
                }

                GhostEnemy[] ghosts =
                    enemy.GetComponentsInChildren<GhostEnemy>(true);

                foreach (GhostEnemy ghost in ghosts)
                {
                    ghost.SetPlayer(player);
                }

                OrcEnemy[] orcs =
                    enemy.GetComponentsInChildren<OrcEnemy>(true);

                foreach (OrcEnemy orc in orcs)
                {
                    orc.SetPlayer(player);
                }
            }
        }

        Debug.Log("Level enemies spawned!");
    }
}
