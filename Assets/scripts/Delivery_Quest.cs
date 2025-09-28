using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Delivery_Quest : MonoBehaviour
{
   [Header("Delivery Setup")]
    public Transform chickenShop;        // The restaurant/pickup location
    public List<Transform> deliveryHouses;      // List of houses to deliver to
    public GameObject chickenOrder;      // The chicken package
    public Transform car;                // Your car

    private int currentDeliveryIndex = 0;
    
    [Header("Quest Display")]
    public Text questText;               // UI text element to update
    
    [Header("Settings")]
    public float deliveryDistance = 1f;  // How close car needs to be
    
    // Simple state tracking
    private bool hasChicken = false;     // Do we have chicken in the car?
    private bool questComplete = false;
    
    // Public properties for car controller
    public Vector3 CurrentTarget { get; private set; }
    public bool IsQuestActive { get; private set; }
    
    void Start()
    {
        // Start at chicken shop
        CurrentTarget = chickenShop.position;
        IsQuestActive = true;
        UpdateQuestText("Drive to Chicken Shop");
        
        // Test if text updates work
        Debug.Log("Quest started - checking if text updates...");
        currentDeliveryIndex = 0;

    }
    
    void Update()
    {
        if (questComplete) return;
        
        // Check if car is close enough to current target
        float distanceToTarget = Vector3.Distance(car.position, CurrentTarget);
        
        if (distanceToTarget <= deliveryDistance)
        {
            if (!hasChicken && CurrentTarget == chickenShop.position)
            {
                // At chicken shop - pick up
                PickupChicken();
            }
            else if (hasChicken && CurrentTarget == deliveryHouses[currentDeliveryIndex].position)
            {
                // At house - deliver
                DeliverChicken();
            }
        }
    }
    
    void PickupChicken()
    {
        if (currentDeliveryIndex >= deliveryHouses.Count)
        {
            return;
        }

        if (hasChicken)
        {
            UpdateQuestText("Order already in car!");
        }
        // Attach chicken to car
        if (chickenOrder != null)
        {
            chickenOrder.transform.SetParent(car);
            chickenOrder.transform.localPosition = new Vector3(0, 1, -1);
        }
        
        hasChicken = true;
        CurrentTarget = deliveryHouses[currentDeliveryIndex].position;
        UpdateQuestText($"Deliver chicken to house {currentDeliveryIndex + 1} of {deliveryHouses.Count}");
    }
    
    void DeliverChicken()
    {
        // Drop off chicken at house
        if (chickenOrder != null)
        {
            chickenOrder.transform.SetParent(null);
            chickenOrder.transform.position = deliveryHouses[currentDeliveryIndex].position + Vector3.up;
        }

        currentDeliveryIndex++;
        hasChicken = false;

        if (currentDeliveryIndex < deliveryHouses.Count)
        {
            CurrentTarget = chickenShop.position;
            UpdateQuestText($"Return to Chicken Shop for order {currentDeliveryIndex+1}");
        }
        else
        {
            questComplete = true;
            IsQuestActive = false;
            UpdateQuestText("All Deliveries Complete!");
            SceneManager.LoadScene("MainMenu");
        }

    }
    
    void UpdateQuestText(string message)
    {
        if (questText != null)
        {
            questText.text = message;
            Debug.Log("Quest text updated: " + message);
        }
        else
        {
            Debug.LogError("Quest Text is not assigned!");
        }
    }
    
    // Helper methods for car controller
    public float DistanceToTarget()
    {
        return Vector3.Distance(car.position, CurrentTarget);
    }
    
    public Vector3 DirectionToTarget()
    {
        return (CurrentTarget - car.position).normalized;
    }
}
