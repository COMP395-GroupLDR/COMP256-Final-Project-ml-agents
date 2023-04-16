using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoundownTimer : MonoBehaviour
{
    [SerializeField] private float durationInSeconds;
    [SerializeField] private Text timerLabel;

    private float timeLeft;

    void Start()
    {
        timeLeft = durationInSeconds;
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerLabel.text = $"{Mathf.FloorToInt(timeLeft / 60):0}:{Mathf.FloorToInt(timeLeft % 60):00}";
        }
        else if (timeLeft < 0)
        {
            timeLeft = 0;
            timerLabel.text = "0:00";
            GameStateHandler.Instance.EndGame();
        }
    }
}
