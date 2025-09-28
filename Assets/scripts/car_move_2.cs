using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_move_2 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float baseMaxSpeed = 10f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float steeringSpeed = 200f;
    
    private float steeringInput;
    private float accelerationInput;
    private Coroutine speedBoostCoroutine;
    
    // Start is called before the first frame update
    void Awake()
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    // Update is called once per frame
    void Update()
    {
        steeringInput = Input.GetAxis("Horizontal");
        accelerationInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        rb.velocity = transform.up * accelerationInput * maxSpeed;
        ApplySteering();
    }

    void ApplySteering()
    {
        if (accelerationInput != 0)
        {
            float steeringAmount = steeringInput * steeringSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, 0, -steeringAmount);
        }
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
