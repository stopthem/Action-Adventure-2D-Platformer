using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPlayer : MonoBehaviour
{
    private PlayerController m_playerController;

    public float shrinkSpeed;

    public Image barImage;
    private Image damagedBarImage;

    private void Awake()
    {
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        damagedBarImage = transform.Find("barBackground").GetComponent<Image>();
    }

    private void Update()
    {
        SetHealthBar();

        if (m_playerController.isPoisoned)
        {
            barImage.color = Color.green;
        }
        else
        {
            barImage.color = Color.red;
        }

        if (barImage.fillAmount < damagedBarImage.fillAmount)
        {
            damagedBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
        }
    }

    private void SetHealthBar()
    {
        barImage.fillAmount = m_playerController.GetHealthNormalized();
    }
}
