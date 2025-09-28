using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move_2 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float baseMaxSpeed = 50f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float maxReverseSpeed = 25f; // Slower reverse speed
    [SerializeField] private float steeringSpeed = 150f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float dragAmount = 0.1f;
    [SerializeField] private float tireGrip = 0.95f;
    [SerializeField] private float accelerationForce = 20f;
    [SerializeField] private float reverseAccelerationForce = 12f; // Weaker reverse acceleration
    [SerializeField] private float directionChangeResistance = 8f; // Resistance when changing direction
    
    private float steeringInput;
    private float accelerationInput;
    private Coroutine speedBoostCoroutine;
    private float currentSpeedMultiplier = 1f;
    private bool wasMovingForward = true; // Track previous direction
    

    void Awake()
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // Setting drag
        rb.drag = dragAmount; 
    }
    
    void Update()
    {
        // Applying A & D and <- & -> for horizontal movement
        steeringInput = Input.GetAxis("Horizontal");
        
        // Applying W & S and ^ & ? lol for horizontal movement
        accelerationInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        if (accelerationInput == 0)
        {
            // Calculate how sideways the car is relative to its velocity
            float velocityMagnitude = rb.velocity.magnitude;
            if (velocityMagnitude > 0.1f)
            {
                Vector2 velocityDirection = rb.velocity.normalized;
                float alignment = Mathf.Abs(Vector2.Dot(velocityDirection, transform.up));
                
                // When car is sideways (alignment close to 0), increase deceleration significantly
                float sidewaysMultiplier = Mathf.Lerp(4f, 1f, alignment); // 4x deceleration when sideways, normal when aligned
                
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, (deceleration * sidewaysMultiplier / 4) * Time.fixedDeltaTime);
            }
            else
            {
                // Normal passive slowdown when barely moving
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, (deceleration / 4) * Time.fixedDeltaTime);
            }
        }
        
        if (accelerationInput != 0)
        {
            HandleAcceleration();
        }

        ApplySteering();
        KillOrthogonalVelocity();
    }

    void HandleAcceleration()
    {
        bool isMovingForward = Vector2.Dot(rb.velocity, transform.up) >= 0;
        bool wantsToGoForward = accelerationInput > 0;
        
        // Check if we're trying to change direction
        bool changingDirection = false;
        if (rb.velocity.magnitude > 1f) // Only check if we're actually moving
        {
            changingDirection = (isMovingForward && !wantsToGoForward) || (!isMovingForward && wantsToGoForward);
        }
        
        // Apply extra resistance when changing direction
        if (changingDirection)
        {
            // Separate forward/backward momentum from sideways momentum
            Vector2 forwardVelocity = Vector2.Dot(rb.velocity, transform.up) * transform.up;
            Vector2 sidewaysVelocity = rb.velocity - forwardVelocity;
            
            // Kill forward momentum more aggressively, preserve more sideways momentum
            forwardVelocity = Vector2.Lerp(forwardVelocity, Vector2.zero, directionChangeResistance * Time.fixedDeltaTime);
            sidewaysVelocity = Vector2.Lerp(sidewaysVelocity, Vector2.zero, (directionChangeResistance * 0.3f) * Time.fixedDeltaTime);
            
            rb.velocity = forwardVelocity + sidewaysVelocity;
        }
        
        // Determine force and max speed based on direction
        float currentForce = accelerationForce;
        float currentMaxSpeed = maxSpeed;
        
        if (accelerationInput < 0) // Going reverse
        {
            currentForce = reverseAccelerationForce;
            currentMaxSpeed = maxReverseSpeed;
        }
        
        // Apply force
        rb.AddForce(transform.up * accelerationInput * currentForce);
        
        // Enforce speed limits
        if (accelerationInput > 0 && rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }
        else if (accelerationInput < 0)
        {
            // For reverse, check the reverse component specifically
            float reverseSpeed = Vector2.Dot(rb.velocity, -transform.up);
            if (reverseSpeed > currentMaxSpeed)
            {
                Vector2 forwardComponent = Vector2.Dot(rb.velocity, transform.up) * transform.up;
                Vector2 reverseComponent = Vector2.Dot(rb.velocity, -transform.up) * (-transform.up);
                Vector2 sidewaysComponent = rb.velocity - forwardComponent - reverseComponent;
                
                // Clamp reverse speed
                reverseComponent = reverseComponent.normalized * currentMaxSpeed;
                rb.velocity = forwardComponent + reverseComponent + sidewaysComponent;
            }
        }
        
        wasMovingForward = wantsToGoForward;
    }

    void ApplySteering()
    {
        if (steeringInput != 0)
        {
            float steeringAmount = steeringInput * steeringSpeed * Time.fixedDeltaTime;
            
            // Reduce steering effectiveness when going in reverse
            if (accelerationInput < 0)
            {
                steeringAmount *= 0.7f; // 30% reduction in reverse steering
            }
            
            // Reduce steering when moving very slowly
            if (rb.velocity.magnitude < 2f)
            {
                steeringAmount *= 0.3f;
            }
            
            transform.Rotate(0, 0, -steeringAmount);
        }
    }
    
    /**
     * AI orthogonal velocity because I don't understand it well enough
     */
    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = Vector2.Dot(rb.velocity, transform.up) * transform.up;
        Vector2 sidewaysVelocity = rb.velocity - forwardVelocity;

        // Grip should be a slow decay, not a hard snap.
        float grip = Mathf.Abs(steeringInput) > 0.1f ? tireGrip : 0.02f;
        
        // Less grip when in reverse
        if (accelerationInput < 0)
        {
            grip *= 0.8f;
        }

        // Apply only a fraction of correction each physics step
        rb.velocity = forwardVelocity + Vector2.Lerp(sidewaysVelocity, Vector2.zero, grip * Time.fixedDeltaTime);
    }
    
    /**
     * Copied Chris's speed boost routine
     * MAY NEED TO FIX!
     */
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
        maxReverseSpeed = (baseMaxSpeed * 0.5f) * multiplier; // Reverse speed scales too but stays proportionally slower
        yield return new WaitForSeconds(duration);  // Wait for duration
        maxSpeed = baseMaxSpeed;  // Reset to normal speed
        maxReverseSpeed = baseMaxSpeed * 0.5f; // Reset reverse speed
        speedBoostCoroutine = null;  // Clear the reference
    }
    
    // Boolean for UI
    public bool IsBoosted()
    {
        return currentSpeedMultiplier > 1f;
    }
    
    // Speed Multiplier for UI
    public float GetSpeedMultiplier()
    {
        return currentSpeedMultiplier;
    }
    
    // Helper method to check if car is moving in reverse
    public bool IsMovingInReverse()
    {
        return Vector2.Dot(rb.velocity, transform.up) < 0;
    }
}