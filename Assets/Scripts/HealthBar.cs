using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Enemy enemy;

    private Transform m_enemyTransform;

    private Transform barTransform;

    public float hideTimer;
    private float hideTimerMax;

    private bool canCountdown;


    private void Awake()
    {
        barTransform = transform.Find("Bar").GetComponent<Transform>();
        m_enemyTransform = enemy.GetComponent<Transform>();
    }

    private void Start()
    {
        hideTimerMax = hideTimer;
        Hide();
    }

    private void Update()
    {
        HandleDirection();

        UpdateBar();

        // HandleHideUnhide();
        if (enemy.isTakingHit || enemy.didTakeDamage)
        {
            Show();

            canCountdown = true;
        }
        else if (enemy.GetCurrentHealth() <= 0)
        {
            Hide();
        }

        if (canCountdown)
        {
            hideTimer -= Time.deltaTime;

            if (enemy.isTakingHit || enemy.didTakeDamage)
            {
                hideTimer = hideTimerMax;
            }

            if (hideTimer <= 0)
            {
                Hide();
            }
        }

    }

    private void HandleDirection()
    {
        if (enemy != null)
        {
            if (m_enemyTransform.rotation == Quaternion.Euler(0, 180, 0))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void UpdateBar()
    {
        if (enemy.GetHealthNormalized() < 0)
        {
            barTransform.localScale = new Vector3(0, 1f);
        }
        else
        {
            barTransform.localScale = new Vector3(enemy.GetHealthNormalized(), 1f);
        }
    }

    private void Hide()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].enabled = false;
        }
    }

    private void Show()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].enabled = true;
        }
    }
}
