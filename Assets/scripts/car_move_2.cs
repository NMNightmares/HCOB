using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move_2 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float baseMaxSpeed = 50f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float steeringSpeed = 150f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float dragAmount = 0.1f;
    [SerializeField] private float tireGrip = 0.95f;
    [SerializeField] private float accelerationForce = 20f;
    
    private float steeringInput;
    private float accelerationInput;
    private Coroutine speedBoostCoroutine;
    private float currentSpeedMultiplier = 1f;
    

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
            // Passive slowdown when no throttle
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, (deceleration / 4) * Time.fixedDeltaTime);
        }
        
        if (accelerationInput != 0)
        {
            // "Gas pedal"
            rb.AddForce(transform.up * accelerationInput * accelerationForce);
        
            // TODO - adjust max speed & apply hot sauce modifier
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

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
    
    /**
     * AI orthogonal velocity because I don't understand it well enough
     */
    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = Vector2.Dot(rb.velocity, transform.up) * transform.up;
        Vector2 sidewaysVelocity = rb.velocity - forwardVelocity;

        // Grip should be a slow decay, not a hard snap.
        float grip = Mathf.Abs(steeringInput) > 0.1f ? tireGrip : 0.02f;

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
        yield return new WaitForSeconds(duration);  // Wait for duration
        maxSpeed = baseMaxSpeed;  // Reset to normal speed
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
}
