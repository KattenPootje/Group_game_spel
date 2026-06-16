using System.Collections;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("enemies in this level")]
    public GameObject[] Enemies;

    [Header("Spawn")]
    public Transform[] spawnPoint;
    public GroundElevator groundElevator;

    [Header("Wave Settings")]
    public int waveAmount = 5;
    public int EnemiesPerWave = 5;
    public int extraEnemiesPerWave = 2;
    public float timeBetweenWaves = 10f;
    public int currentWave = 0;
    public int currentAliveEnemies = 0;
    public float waveStartTime = 0f;
    public bool levelEnding = false;
    public int CurrentLevel = 1;

    void Start()
    {
        StartCoroutine(StartWaveSystem());
    }

    IEnumerator StartWaveSystem()
    {
        while (true)
        {
            waveStartTime = Time.time;
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;

            yield return StartCoroutine(SpawnWave());

            while (currentAliveEnemies > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (currentWave >= waveAmount*CurrentLevel)
            {

                levelEnding = true;
                groundElevator.isAtNewLevel = false;
                while (groundElevator.isAtNewLevel == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                CurrentLevel ++;
                levelEnding = false;
            }
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < EnemiesPerWave + extraEnemiesPerWave*(currentWave-1); i++)
        {
            SpawnCube();
            float timeBetweenSpawns = Random.Range(0.1f, 1f);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnCube()
    {
        int spawnIndex = Random.Range(0, spawnPoint.Length);
        currentAliveEnemies += 1;
        GameObject cube = Instantiate(
            Enemies[Random.Range(0, Enemies.Length)],
            spawnPoint[spawnIndex].position,
            Quaternion.identity
        );
    }
}