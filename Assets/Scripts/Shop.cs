using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    private Button m_interactButton;

    private bool m_playerInArea;
    private bool m_open;

    private float startingHealth;

    [SerializeField] private GameObject healthP;
    [SerializeField] private GameObject interactText;
    private TextMeshPro interactTexttext;

    [Header("Speed Pickup")]
    [SerializeField] private GameObject speedP;
    [SerializeField] private float speedAmount;

    [Header("DamagePickup")]
    [SerializeField] private GameObject damageP;
    [SerializeField] private float damageAmount;

    [Header("InvincPickup")]
    [SerializeField] private GameObject invincP;
    [SerializeField] private float invincAmount;

    [Header("Cost")]
    [SerializeField] private float coinCost;

    private void Start()
    {
        interactTexttext = interactText.GetComponent<TextMeshPro>();
        startingHealth = PlayerController.Instance.health;
        interactText.SetActive(false);
    }

    private void Update()
    {
        if (m_playerInArea && Input.GetKeyDown(KeyCode.E))
        {
            UIHandler.Instance.shopScreen.SetActive(true);
        }

        if (InputDetection.instance.isJoystickControlsForMobileEnabled && m_playerInArea)
        {
            interactTexttext.text = "Press 'Interact' To interact";
            if (m_playerInArea)
            {
                UIHandler.Instance.MobileButtonHandler(true, false, false, true);
            }
            else
            {
                UIHandler.Instance.shopScreen.SetActive(false);
                UIHandler.Instance.MobileButtonHandler(false, false, false, true);
            }

            if (m_open)
            {
                UIHandler.Instance.shopScreen.SetActive(true);
                UIHandler.Instance.MobileButtonHandler(false, false, false, true);
            }

            CheckForInteract();
        }
    }

    public void CheckForInteract()
    {
#if UNITY_EDITOR
        if (m_interactButton == null)
        {
            m_interactButton = GameObject.Find("JoystickInteract").GetComponent<Button>();
        }
#endif

        m_interactButton.onClick.AddListener(delegate { OnButtonTapped(); });
    }

    public void OnButtonTapped()
    {
        if (m_playerInArea)
        {
            m_open = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactText.SetActive(true);
            m_playerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactText.SetActive(false);
            UIHandler.Instance.shopScreen.SetActive(false);
            m_playerInArea = false;
        }
    }

    public void CloseShop()
    {
        UIHandler.Instance.shopScreen.SetActive(false);
        m_open = false;
    }

    public void SpeedPickup()
    {
        if (coinCost <= GameManager.Instance.m_currentCoins)
        {
            GameManager.Instance.m_currentCoins -= coinCost;
            UIHandler.Instance.UpdateCoinText(GameManager.Instance.m_currentCoins);
            PlayerMovement.Instance.moveSpeed = speedAmount;
            speedP.SetActive(false);
        }
    }

    public void InvicPickup()
    {
        if (coinCost <= GameManager.Instance.m_currentCoins)
        {
            GameManager.Instance.m_currentCoins -= coinCost;
            UIHandler.Instance.UpdateCoinText(GameManager.Instance.m_currentCoins);
            PlayerController.Instance.invincibleDuration = invincAmount;
            invincP.SetActive(false);
        }
    }

    public void DamagePickup()
    {
        if (coinCost <= GameManager.Instance.m_currentCoins)
        {
            GameManager.Instance.m_currentCoins -= coinCost;
            UIHandler.Instance.UpdateCoinText(GameManager.Instance.m_currentCoins);
            PlayerController.Instance.damage = damageAmount;
            damageP.SetActive(false);
        }
    }

    public void HealthPickup()
    {
        if (coinCost <= GameManager.Instance.m_currentCoins)
        {
            GameManager.Instance.m_currentCoins -= coinCost;
            UIHandler.Instance.UpdateCoinText(GameManager.Instance.m_currentCoins);
            PlayerController.Instance.currentHealth = startingHealth;
            healthP.SetActive(false);
        }
    }
}
