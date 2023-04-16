using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [SerializeField]
    GameObject endScreen;

    public static GameStateHandler Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    public void EndGame()
    {
        endScreen.SetActive(true);
        Time.timeScale = 0f;
        

    }


}
