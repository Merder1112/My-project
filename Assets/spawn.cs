using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;

    private HashSet<GameObject> activeEnemies = new HashSet<GameObject>();

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            UnityEngine.Debug.LogWarning("No spawn points assigned to EnemySpawner. Attempting to find spawn points in children.");
            spawnPoints = GetComponentsInChildren<Transform>();

            // Remove this transform from the array
            List<Transform> spawnPointsList = new List<Transform>(spawnPoints);
            spawnPointsList.Remove(transform); // Remove the spawner's transform
            spawnPoints = spawnPointsList.ToArray();

            if (spawnPoints.Length == 0)
            {
                UnityEngine.Debug.LogError("No spawn points found. EnemySpawner will not function correctly.");
                enabled = false;
                return;
            }
        }

        if (enemyPrefab == null)
        {
            UnityEngine.Debug.LogError("Enemy prefab not assigned to EnemySpawner. Spawner will not function.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    public void SpawnEnemy()
    {
        UnityEngine.Debug.Log($"Spawn Points Count: {spawnPoints.Length}, Enemy Prefab: {enemyPrefab}");
        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            UnityEngine.Debug.LogWarning("Cannot spawn enemy: Missing spawn points or enemy prefab.");
            return;
        }

        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            UnityEngine.Debug.LogWarning("Cannot spawn enemy: Missing spawn points or enemy prefab.");
            return;
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enemySpawner = this; // Set the enemySpawner reference
        }
        else
        {
            UnityEngine.Debug.LogWarning("Spawned enemy does not have an EnemyController component.");
        }

        activeEnemies.Add(enemy);
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (activeEnemies.Remove(enemy))
        {
            // Optional: Additional code when enemy is defeated
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                }
            }
        }
    }
}
