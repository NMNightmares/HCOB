using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class timer_handler : MonoBehaviour
{
    public Slider timerSlider;
    [SerializeField] private float duration;

    private float remaining;
    private bool isRunning = false;
    [SerializeField] private Gradient coolingGradient;

    [SerializeField] private Image fillImage;

    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            remaining -= Time.deltaTime;
            if (remaining <= 0)
            {
                remaining = 0;
                isRunning = false;
                OnTimerEnd();
            }
            timerSlider.value = remaining;
            Color blue = new Color(0.6f, 1f, 1f);
            Color red = new Color(1f, 0f, 0f);
            Color orange = new Color(1f, 0.5f, 0f);
            float normalTime = remaining / duration;
            fillImage.color = coolingGradient.Evaluate(1f - normalTime);
        }
    }

    public void StartTimer()
    {
        remaining = duration;
        timerSlider.maxValue = duration;
        timerSlider.value = duration;
        isRunning = true;
    }

    private void OnTimerEnd()
    {
        Debug.Log("Timer Finished!");
        SceneManager.LoadScene("MainMenu"); 
    }
    
}
