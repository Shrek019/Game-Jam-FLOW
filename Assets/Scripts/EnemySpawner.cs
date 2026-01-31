using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Transform player;

    [Header("Enemy Prefabs")]
    public GameObject enemyTypeA;
    public GameObject enemyTypeB;

    [Header("Spawn Timing")]
    public float spawnInterval = 2f;        // Time between waves
    public float enemySpacingDelay = 0.4f;  // Time between enemies in a wave

    [Header("Difficulty")]
    public int maxDifficultyTier = 3;

    private float timer;
    private bool isSpawningWave;

    void Update()
    {
        if (isSpawningWave)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            StartCoroutine(SpawnWaveCoroutine());
        }
    }

    IEnumerator SpawnWaveCoroutine()
    {
        isSpawningWave = true;

        int score = ScoreManager.Instance.Score;
        int difficultyTier = Mathf.Clamp(score / 500, 0, maxDifficultyTier);

        // Randomize number of enemies per wave (1-4) and add difficulty
        int enemiesToSpawn = Random.Range(1, 5) + difficultyTier;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(enemySpacingDelay);
        }

        isSpawningWave = false;
    }

    void SpawnEnemy()
    {
        // Randomly choose enemy type for each spawn
        GameObject enemyPrefab = Random.value < 0.5f ? enemyTypeA : enemyTypeB;

        Vector3 spawnPos = GetSpawnPositionOutsideCamera();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    Vector3 GetSpawnPositionOutsideCamera()
    {
        // Calculate camera width in world units
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // Spawn just outside the right side of the camera
        float x = mainCamera.transform.position.x + camWidth / 2f + 2f;

        // Lock height to player
        float y = player.position.y;

        return new Vector3(x, y, 0f);
    }
}
