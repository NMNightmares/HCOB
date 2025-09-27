using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSauceSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private GameObject hotSauceBottlePrefab;
    [SerializeField] private int maxBottlesOnMap = 3;
    [SerializeField] private float spawnCooldown = 5f;
    [SerializeField] private float minSpawnInterval = 10f;
    [SerializeField] private float maxSpawnInterval = 20f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10, -10);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(10, 10);
    [SerializeField] private LayerMask obstacleLayer = 1; // What layers to avoid spawning on
    [SerializeField] private float clearRadius = 1f; // Minimum clear space around spawn point

    [Header("Debug")]
    [SerializeField] private bool showSpawnArea = true;

    public static HotSauceSpawner Instance;

    private List<GameObject> activeBottles = new List<GameObject>();
    private Coroutine spawnCoroutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for random interval
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Try to spawn if we're under the limit
            if (activeBottles.Count < maxBottlesOnMap)
            {
                TrySpawnBottle();
            }
        }
    }

    private void TrySpawnBottle()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();

        if (IsValidSpawnPosition(spawnPosition))
        {
            GameObject newBottle = Instantiate(hotSauceBottlePrefab, spawnPosition, Quaternion.identity);
            activeBottles.Add(newBottle);

            Debug.Log($"🌶️ Hot sauce bottle spawned at {spawnPosition}!");
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(x, y);
    }

    private bool IsValidSpawnPosition(Vector2 position)
    {
        // Check if there's enough clear space
        Collider2D hit = Physics2D.OverlapCircle(position, clearRadius, obstacleLayer);
        return hit == null;
    }

    public void OnBottleCollected()
    {
        // Clean up null references from destroyed bottles
        activeBottles.RemoveAll(bottle => bottle == null);
    }

    // Manual spawn for testing
    [ContextMenu("Spawn Hot Sauce Bottle")]
    public void SpawnBottleNow()
    {
        if (activeBottles.Count < maxBottlesOnMap)
        {
            TrySpawnBottle();
        }
    }

    // Clear all bottles
    [ContextMenu("Clear All Bottles")]
    public void ClearAllBottles()
    {
        foreach (GameObject bottle in activeBottles)
        {
            if (bottle != null)
            {
                Destroy(bottle);
            }
        }
        activeBottles.Clear();
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnArea)
        {
            // Draw spawn area
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2f, (spawnAreaMin.y + spawnAreaMax.y) / 2f, 0);
            Vector3 size = new Vector3(spawnAreaMax.x - spawnAreaMin.x, spawnAreaMax.y - spawnAreaMin.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}