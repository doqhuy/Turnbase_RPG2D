using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRange = 10f;
    public Transform player;
    public float despawnTime = 10f; // Time in seconds before despawning
    [Range(0f, 1f)] public float spawnChance = 0.5f;

    private bool hasSpawned = false;
    private GameObject spawnedEnemy;
    private float despawnTimer = 0f;
    private float spawnTimer = 0f;

    private void Start()
    {
        // Assuming the player object is tagged as "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        // Check if an enemy has already been spawned
        if (!hasSpawned)
        {
            // Check the distance between the spawner and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (spawnTimer >= 3f)
            {
                // Reset the spawn timer
                spawnTimer = 0f;
                // If player is within spawnRange, spawn enemy
                if (distanceToPlayer < spawnRange)
                {
                    if (Random.value < spawnChance)
                    {
                        SpawnEnemy();
                    }                   
                }
            }
        }

        else
        {
            // If enemy has been spawned, start the despawn timer
            despawnTimer -= Time.deltaTime;
            if (despawnTimer <= 0)
            {
                DespawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        // Instantiate enemy prefab at the spawner's position
        spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        spawnedEnemy.transform.SetParent(transform.parent);
        // Set hasSpawned to true
        hasSpawned = true;

        // Start despawn timer
        despawnTimer = despawnTime;
    }

    private void DespawnEnemy()
    {
        // Destroy the spawned enemy
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
        }

        // Reset flags and timer
        hasSpawned = false;
        despawnTimer = 0f;
    }
}