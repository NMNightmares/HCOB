using UnityEngine;
using UnityEngine.UI;

public class VisualSpeedometer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private car_move carController;
    [SerializeField] private Image speedBar; // A simple bar that fills up
    [SerializeField] private Text speedText; // Shows exact speed number

    [Header("Settings")]
    [SerializeField] private float maxSpeedForBar = 20f; // Max speed shown on bar

    private Rigidbody2D carRigidbody;

    void Start()
    {
        if (carController != null)
        {
            carRigidbody = carController.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (carController != null && carRigidbody != null)
        {
            UpdateSpeedometer();
        }
    }

    void UpdateSpeedometer()
    {
        // Get current speed
        float currentSpeed = carRigidbody.velocity.magnitude;

        // Update speed bar (if you have one)
        if (speedBar != null)
        {
            float fillAmount = Mathf.Clamp01(currentSpeed / maxSpeedForBar);
            speedBar.fillAmount = fillAmount;

            // Change color based on boost status
            if (carController.IsBoosted())
            {
                speedBar.color = Color.red; // Hot sauce red when boosted!
            }
            else
            {
                speedBar.color = Color.green; // Normal green
            }
        }

        // Update speed text
        if (speedText != null)
        {
            string displayText = $"Speed: {currentSpeed:F1}";

            // Add boost indicator
            if (carController.IsBoosted())
            {
                float multiplier = carController.GetSpeedMultiplier();
                displayText += $"\n🌶️ BOOST {multiplier:F1}X!";
                speedText.color = Color.red;
            }
            else
            {
                speedText.color = Color.white;
            }

            speedText.text = displayText;
        }
    }
}