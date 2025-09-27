using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float steering = 2.5f;
    [SerializeField] private float driftFactor = 0.95f;
    [SerializeField] private float drag = 0.1f;

    [Header("Speed Boost Settings")]
    private float baseMaxSpeed; // Store original max speed
    private float currentSpeedMultiplier = 1f;
    private Coroutine speedBoostCoroutine;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private float steeringInput;
    private float accelerationInput;

    void Awake()
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        baseMaxSpeed = maxSpeed; // Store the original max speed
    }

    void Update()
    {
        steeringInput = Input.GetAxis("Horizontal");
        accelerationInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
        KillOrthogonalVelocity();
    }

    void ApplyEngineForce()
    {
        Vector2 forward = transform.up;
        float currentSpeed = Vector2.Dot(rb.velocity, forward);

        // Use boosted max speed
        float currentMaxSpeed = baseMaxSpeed * currentSpeedMultiplier;

        if (accelerationInput > 0 && currentSpeed >= currentMaxSpeed) return;
        if (accelerationInput < 0 && currentSpeed <= -currentMaxSpeed / 2f) return;

        Vector2 force = forward * accelerationInput * acceleration;
        rb.AddForce(force, ForceMode2D.Force);
        rb.velocity = rb.velocity * (1f - drag * Time.fixedDeltaTime);
    }

    void ApplySteering()
    {
        float currentMaxSpeed = baseMaxSpeed * currentSpeedMultiplier;
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / currentMaxSpeed);
        float rotationAmount = steeringInput * steering * speedFactor;
        rb.MoveRotation(rb.rotation - rotationAmount);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;
        float forwardVelocity = Vector2.Dot(rb.velocity, forward);
        float sidewaysVelocity = Vector2.Dot(rb.velocity, right);
        Vector2 newVelocity = forward * forwardVelocity + right * sidewaysVelocity * driftFactor;
        rb.velocity = newVelocity;
    }

    // Speed boost methods
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
        Debug.Log($"🌶️ Speed boost applied! Multiplier: {multiplier}x for {duration} seconds");
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        currentSpeedMultiplier = multiplier;

        yield return new WaitForSeconds(duration);

        currentSpeedMultiplier = 1f;
        speedBoostCoroutine = null;
        Debug.Log("Speed boost ended - back to normal speed");
    }

    public bool IsBoosted()
    {
        return currentSpeedMultiplier > 1f;
    }

    public float GetSpeedMultiplier()
    {
        return currentSpeedMultiplier;
    }
}