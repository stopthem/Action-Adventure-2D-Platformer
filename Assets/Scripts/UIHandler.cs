using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance {get; private set;}
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI chestText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCoinText(float number)
    {
        coinText.text = number.ToString();
    }

    public void ShowChestText(bool status)
    {
        chestText.gameObject.SetActive(status);
    }
}
