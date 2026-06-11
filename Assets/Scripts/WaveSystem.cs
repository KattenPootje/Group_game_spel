using System.Collections;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("enemies in this level")]
    public GameObject[] Enemies;

    [Header("Spawn")]
    public Transform[] spawnPoint;

    [Header("Wave Settings")]
    public int waveAmount = 5;
    public int EnemiesPerWave = 5;
    public int extraEnemiesPerWave = 2;
    public float timeBetweenWaves = 10f;
    public int currentWave = 0;
    public int currentAliveEnemies = 0;
    public float waveStartTime = 0f;
    public bool levelEnding = false;

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

            if (currentWave == waveAmount)
            {
                levelEnding = true;

                yield return new WaitForSeconds(3f);
                

                break;
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
            Enemies[Random.Range(0,2)],
            spawnPoint[spawnIndex].position,
            Quaternion.identity
        );
    }
}