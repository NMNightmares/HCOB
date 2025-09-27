using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class timer_handler : MonoBehaviour
{
    [SerializeField] private float timeRemaining;
    [SerializeField] TextMeshProUGUI timerText;
    private bool timerIsRunning = false;
    
    void Start()
    {
        timerIsRunning = true;
    }

    //TODO Add in .000 to timer
    
    /// <summary>
    /// Converts the time to minutes and seconds. Displays them in the format (00:00)
    /// </summary>
    /// <param name="timeToDisplay">The amount of time in seconds to be displayed</param>
    /// <returns>None</returns>
        void DisplayTime(float timeToDisplay)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            if (minutes == 0)
            {
                
            }
            
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        /// <summary>
        ///  Updates the value of the timer every tick
        /// </summary>
        /// <returns></returns>
    void Update()
    {
        if (timerIsRunning)
        {
            DisplayTime(timeRemaining);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
}
