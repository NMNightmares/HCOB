using System.Collections;
using UnityEngine;

public class SHSS : MonoBehaviour
{
    [Header("Required References")]
    public GameObject hotSauceBottlePrefab;
    public Transform carTransform;

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxBottles = 2;
    public float spawnDistance = 10f;

    private int currentBottleCount = 0;

    void Start()
    {
        // Auto-find car if not assigned
        if (carTransform == null)
        {
            GameObject carObject = GameObject.FindWithTag("Player");
            if (carObject == null)
            {
                // Try to find by component
                car_move carScript = FindObjectOfType<car_move>();
                if (carScript != null)
                {
                    carTransform = carScript.transform;
                }
            }
            else
            {
                carTransform = carObject.transform;
            }
        }

        // Start spawning
        if (carTransform != null && hotSauceBottlePrefab != null)
        {
            InvokeRepeating("TrySpawnBottle", 2f, spawnInterval);
            Debug.Log("Spawner started successfully");
        }
        else
        {
            Debug.LogError("Spawner missing references! Car: " + (carTransform != null) + " Prefab: " + (hotSauceBottlePrefab != null));
        }
    }

    void TrySpawnBottle()
    {
        if (currentBottleCount >= maxBottles)
        {
            Debug.Log("Max bottles reached, not spawning");
            return;
        }

        if (carTransform == null)
        {
            Debug.Log("No car found, cannot spawn");
            return;
        }

        // Random position around car
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = carTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnDistance;

        // Spawn the bottle
        GameObject newBottle = Instantiate(hotSauceBottlePrefab, spawnPosition, Quaternion.identity);
        currentBottleCount++;

        // Add a component to track this bottle
        BottleTracker tracker = newBottle.AddComponent<BottleTracker>();
        tracker.spawner = this;

        Debug.Log("Hot sauce bottle spawned at " + spawnPosition + " (Total: " + currentBottleCount + ")");
    }

    public void OnBottleDestroyed()
    {
        currentBottleCount--;
        Debug.Log("Bottle destroyed. Remaining: " + currentBottleCount);
    }

    // Manual spawn for testing
    [ContextMenu("Spawn Bottle Now")]
    public void SpawnBottleNow()
    {
        TrySpawnBottle();
    }
}

// Simple component to track when bottles are destroyed
public class BottleTracker : MonoBehaviour
{
    public SHSS spawner;

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnBottleDestroyed();
        }
    }
}