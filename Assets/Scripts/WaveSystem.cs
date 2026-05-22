using System.Collections;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject cubePrefab;

    [Header("Spawn")]
    public Transform[] spawnPoint;

    [Header("Wave Settings")]
    public int cubesPerWave = 5;
    public float timeBetweenWaves = 3f;

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(StartWaveSystem());
    }

    IEnumerator StartWaveSystem()
    {
        while (true)
        {
            currentWave++;

            Debug.Log("Wave " + currentWave);

            yield return StartCoroutine(SpawnWave());

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < cubesPerWave; i++)
        {
            SpawnCube();
            float timeBetweenSpawns = Random.Range(0.1f, 1f);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnCube()
    {
        int spawnIndex = Random.Range(0, spawnPoint.Length);

        GameObject cube = Instantiate(
            cubePrefab,
            spawnPoint[spawnIndex].position,
            Quaternion.identity
        );
    }
}