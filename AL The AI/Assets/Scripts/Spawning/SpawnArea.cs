using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnArea : MonoBehaviour
{
    [System.Serializable]
    public struct WaveEnemyData
    {
        public enum EnemyType
        {
            Melee,
            Melee_S,
            Melee_I,
            Ranged,
            Ranged_S,
            Ranged_I
        }
        public EnemyType enemyType;
        public int amountToSpawn;
        public float spawnTime; // how quickly to spawn each enemy
        public float spawnDelay; // how long to wait before initially spawning
        public bool spawnInGroup; // multiple spawns of the same enemy
        public int groupSize; // how many to spawn at once
    }

    [System.Serializable]
    public struct WaveSpawnData
    {
        public WaveEnemyData[] waveEnemyData;
    }

    [Header("Next Wave Portraits")]
    [SerializeField] private GameObject NextWaveText;
    [SerializeField] private GameObject[] enemyPortraits;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Details")]
    [SerializeField] private WaveSpawnData[] waveData;

    private int currentWave;

    public int GetTotalEnemies()
    {
        int total = 0;

        for (int i = 0; i < waveData[currentWave].waveEnemyData.Length; i++)
        {
            total += waveData[currentWave].waveEnemyData[i].amountToSpawn + DifficultyManager.instance.difficulty_SpawnAmountModifier[DifficultyManager.instance.difficulty];
        }

        return total;
    }

    public void StartWave(int waveNumber) // called by waveManager at start of a wave
    {
        currentWave = waveNumber;
        HideAllPortraits();

        for (int i = 0; i < waveData[currentWave].waveEnemyData.Length; i++)
        {
            StartCoroutine("SpawnEnemies", i);
        }
    }

    private IEnumerator SpawnEnemies(int enemy)
    {
        int amountSpawned = 0;

        int amountToSpawn = waveData[currentWave].waveEnemyData[enemy].amountToSpawn + DifficultyManager.instance.difficulty_SpawnAmountModifier[DifficultyManager.instance.difficulty];
        float enemySpawnTime = waveData[currentWave].waveEnemyData[enemy].spawnTime;
        float spawnDelay = waveData[currentWave].waveEnemyData[enemy].spawnDelay;
        string poolTag = (waveData[currentWave].waveEnemyData[enemy].enemyType).ToString();
        bool spawnInGroup = waveData[currentWave].waveEnemyData[enemy].spawnInGroup;
        int groupSize = waveData[currentWave].waveEnemyData[enemy].groupSize;

        yield return new WaitForSeconds(spawnDelay); // initial spawn delay

        while (amountSpawned < amountToSpawn)
        {
            yield return new WaitForSeconds(enemySpawnTime); // spawn time between new spawns

            if (spawnInGroup)
            {
                for (int i = 0; i < groupSize; i++)
                {
                    if (amountToSpawn - amountSpawned > 0) // make sure to not over spawn if group size is not even with amount to spawn
                    {
                        SpawnEnemy(poolTag);
                        amountSpawned++;
                    }
                }
            }
            else
            {
                SpawnEnemy(poolTag);
                amountSpawned++;
            }
        }
        amountSpawned = 0;
    }

    private void SpawnEnemy(string poolTag)
    {
        GameObject newEnemy = ObjectPool.Instance.SpawnFromPool(poolTag);

        if (newEnemy != null)
        {
            // increase diffuculty per wave by increasing enemy health by 5.
            Health newEnemyHealth = newEnemy.GetComponent<Health>();
            newEnemyHealth.maxHealth = newEnemyHealth.initialMaxHealth + (DifficultyManager.instance.difficulty_HealthModifier[DifficultyManager.instance.difficulty] * currentWave);

            int rand = Random.Range(0, spawnPoints.Length);
            newEnemy.transform.position = spawnPoints[rand].position;
            newEnemy.transform.rotation = spawnPoints[rand].rotation;
            newEnemy.SetActive(true);
        }
    }

    public void SetNextWavePortraits(int nextWave)
    {
        NextWaveText.SetActive(true);
        Dictionary<string, int> waveDetails = new Dictionary<string, int>(); // store a temp dictionary of types and amounts of enemies

        for (int i = 0; i < waveData[nextWave].waveEnemyData.Length; i++)
        {
            string enemyType = waveData[nextWave].waveEnemyData[i].enemyType.ToString();
            int amountToSpawn = (waveData[nextWave].waveEnemyData[i].amountToSpawn + DifficultyManager.instance.difficulty_SpawnAmountModifier[DifficultyManager.instance.difficulty]);

            if (waveDetails.ContainsKey(enemyType))
            {
                waveDetails[enemyType] += amountToSpawn; // increase amount to spawn
            }
            else
            {
                waveDetails.Add(enemyType, amountToSpawn); // add to dictionary
            }
        }

        foreach (GameObject enemyPortrait in enemyPortraits) // set details of portraits
        {
            if (waveDetails.ContainsKey(enemyPortrait.name))
            {
                enemyPortrait.GetComponentInChildren<TextMeshPro>().text = waveDetails[enemyPortrait.name].ToString();
                enemyPortrait.SetActive(true);
            }
        }
    }

    private void HideAllPortraits()
    {
        NextWaveText.SetActive(false);
        foreach (GameObject enemyPortrait in enemyPortraits)
        {
            enemyPortrait.SetActive(false);
        }
    }
}
