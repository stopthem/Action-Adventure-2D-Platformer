using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float startingCoins;
    private float m_currentCoins;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_currentCoins = startingCoins;
    }

    private void Update()
    {
        UIHandler.Instance.UpdateCoinText(m_currentCoins);
    }

    public void AddCoin(float amount)
    {
        m_currentCoins += amount;
        // play sound
    }
}
