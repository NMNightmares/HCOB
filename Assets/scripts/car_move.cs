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
    [SerializeField] private float drag = 0.1f; // smaller value, applied smoothly

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private float steeringInput;
    private float accelerationInput;

    void Awake()
    {
        // Smooth out movement between physics frames
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // Get input
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
        // Calculate forward movement
        Vector2 forward = transform.up;
        float currentSpeed = Vector2.Dot(rb.velocity, forward);

        // Limit speed
        if (accelerationInput > 0 && currentSpeed >= maxSpeed) return;
        if (accelerationInput < 0 && currentSpeed <= -maxSpeed / 2f) return;

        // Apply acceleration force
        Vector2 force = forward * accelerationInput * acceleration;
        rb.AddForce(force, ForceMode2D.Force);

        // Apply drag smoothly (instead of multiplying velocity directly)
        rb.velocity = rb.velocity * (1f - drag * Time.fixedDeltaTime);
    }

    void ApplySteering()
    {
        // Rotate based on input and current speed
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
        float rotationAmount = steeringInput * steering * speedFactor;
        rb.MoveRotation(rb.rotation - rotationAmount);
    }

    void KillOrthogonalVelocity()
    {
        // Remove sideways velocity (simulate grip vs. drift)
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        float forwardVelocity = Vector2.Dot(rb.velocity, forward);
        float sidewaysVelocity = Vector2.Dot(rb.velocity, right);

        Vector2 newVelocity = forward * forwardVelocity + right * sidewaysVelocity * driftFactor;
        rb.velocity = newVelocity;
    }
}
