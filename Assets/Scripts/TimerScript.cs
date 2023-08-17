using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public int intialTime = 240;
    private int timeLeft = 240;
    private float timer = 0.0f;

    private TextMeshProUGUI timerText;

    [SerializeField] private float timerSpeed = 1.0f;

    private void Start()
    {
        timeLeft = intialTime;
        timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerSpeed)
        {
            timeLeft--;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                // call the endgame from game manager
                GameManager.instance.EndGame();
                // stop the game time
                Time.timeScale = 0;
            }

            timer = 0.0f;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = timeLeft / 60;
        int seconds = timeLeft % 60;

        timerText.text = minutes.ToString("0") + ":" + seconds.ToString("00");
    }

    public int TimeLeft
    {
        get { return timeLeft; }
    }
}
