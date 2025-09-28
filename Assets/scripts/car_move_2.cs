using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move_2 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float baseMaxSpeed = 20f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float steeringSpeed = 3000f;
    [SerializeField] private float deceleration = 5f;
    
    private float steeringInput;
    private float accelerationInput;
    private Coroutine speedBoostCoroutine;
    private float currentSpeedMultiplier = 1f;
    
    // Start is called before the first frame update
    [SerializeField] private float dragAmount = 10f; // Realistic drag value
    [SerializeField] private float tireGrip = 0.8f; // Adjust in inspector

    void Awake()
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.drag = dragAmount; // Set the rigidbody's drag
    }

    // Update is called once per frame
    void Update()
    {
        steeringInput = Input.GetAxis("Horizontal");
        accelerationInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        if (accelerationInput != 0)
        {
            // Add force to build up velocity gradually
            rb.AddForce(transform.up * accelerationInput * maxSpeed * 2f);
        
            // Cap the max speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
        // No else block - let momentum carry naturally

        ApplySteering();
        KillOrthogonalVelocity();
    }

    void ApplySteering()
    {
        // Remove the accelerationInput check - allow steering anytime
        if (steeringInput != 0)
        {
            float steeringAmount = steeringInput * steeringSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, 0, -steeringAmount);
        }
    }
    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = Vector2.Dot(rb.velocity, transform.up) * transform.up;
        Vector2 sidewaysVelocity = rb.velocity - forwardVelocity;

        // Reduce grip during hard turns for sharper angles
        float currentGrip = tireGrip;
        if (Mathf.Abs(steeringInput) > 0.7f)
        {
            currentGrip *= 0.5f; // Less grip = sharper turns
        }
    
        rb.velocity = forwardVelocity + sidewaysVelocity * (1f - currentGrip);
    }
    
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
        maxSpeed = baseMaxSpeed * multiplier;  // Apply the boost
        yield return new WaitForSeconds(duration);  // Wait for duration
        maxSpeed = baseMaxSpeed;  // Reset to normal speed
        speedBoostCoroutine = null;  // Clear the reference
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
