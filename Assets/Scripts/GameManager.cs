using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float startingCoins;
    [HideInInspector] public float m_currentCoins;

    public int wormsKilled;

    public bool canFinish;

    public bool isPaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1;
        m_currentCoins = startingCoins;
        UIHandler.Instance.UpdateCoinText(m_currentCoins);
    }

    private void Update()
    {
        if (wormsKilled == 2)
        {
            canFinish = true;
        }
    }

    public void AddCoin(float amount)
    {
        m_currentCoins += amount;
        UIHandler.Instance.UpdateCoinText(m_currentCoins);
        // play sound
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        isPaused = true;

        UIHandler.Instance.ShowMenu(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        isPaused = false;

        UIHandler.Instance.ShowMenu(false);
    }

    public void MainMenu()
    {
        SceneHandler.Instance.LoadMainMenu();
    }

    public void RestartGame()
    {
        SceneHandler.Instance.LoadGame();
    }
}
