using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hot_sauce : MonoBehaviour
{

    [Header("Hot Sauce Boost Settings")]
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float boostDuration = 10f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem spiceEffect;
    [SerializeField] private AudioClip sipSound;
    [SerializeField] private Color hotSauceGlow = Color.red;

    [Header("Animation")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;
    [SerializeField] private float rotationSpeed = 30f;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Add a slight glow effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hotSauceGlow;
        }
    }

    void Update()
    {
        // Floating animation
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, transform.position.z);

        // Gentle rotation
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Change from car_move to car_move_2
        car_move_2 car = other.GetComponent<car_move_2>();

        if (car != null)
        {
            // Apply the hot sauce speed boost!
            car.ApplySpeedBoost(speedMultiplier, boostDuration);

            // Play spicy effects
            PlaySpicyEffects();

            // Notify the spawner that this bottle was collected
            //HotSauceSpawner.Instance?.OnBottleCollected();

            // Destroy the bottle
            Destroy(gameObject);
        }
    }

    private void PlaySpicyEffects()
    {
        // Spawn spice particle effect
        if (spiceEffect != null)
        {
            ParticleSystem effect = Instantiate(spiceEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 3f);
        }

        // Play drinking/sipping sound
        if (sipSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(sipSound, transform.position);
        }

        Debug.Log("🌶️ Hot sauce consumed! Speed boost activated!");
    }
}