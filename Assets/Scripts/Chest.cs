using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Chest : MonoBehaviour
{
    public GameObject coin;

    private Animator m_animator;

    private Button interactButton;
    private Image interactButtonImage;


    private bool m_isOpened;
    private bool m_canThrow;
    private bool m_canPickup;
    [HideInInspector] public bool m_ontriggerEntered;

    private void Awake()
    {
        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            interactButton = GameObject.Find("JoystickInteract").GetComponent<Button>();
            interactButtonImage = interactButton.GetComponent<Image>();
        }

        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            if (m_ontriggerEntered)
            {
                UIHandler.Instance.MobileButtonHandler(true,false,false,true);
            }
            else
            {
                UIHandler.Instance.MobileButtonHandler(false,false,false,true);
            }

            if (m_isOpened)
            {
                UIHandler.Instance.MobileButtonHandler(false,false,false,true);
                UIHandler.Instance.ShowChestText(false);
            }

            CheckForInteract();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && m_ontriggerEntered)
            {
                m_canPickup = true;
            }
            else if (PlayerController.Instance.canOpenChest && m_ontriggerEntered)
            {
                m_canPickup = true;
            }

            if (m_isOpened)
            {
                UIHandler.Instance.ShowChestText(false);
            }
        }
    }

    private void SpawnCoin()
    {
        if (!m_isOpened)
        {
            StartCoroutine(SpawnCoinRoutine());
        }
    }

    private IEnumerator SpawnCoinRoutine()
    {
        while (m_canThrow == false)
        {
            yield return null;
        }

        GameObject coinInstance = Instantiate(coin, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.rotation) as GameObject;
        coinInstance.GetComponent<Coin>().ForceCoin();

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !m_isOpened)
        {
            m_ontriggerEntered = true;
            UIHandler.Instance.ShowChestText(true);

            if (m_canPickup)
            {

                m_animator.SetTrigger("ChestOpen");
                SpawnCoin();
                m_isOpened = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_ontriggerEntered = false;
            UIHandler.Instance.ShowChestText(false);
        }
    }

    // animation event
    private void ThrowCoin()
    {
        m_canThrow = true;
    }

    public void CheckForInteract()
    {
        #if UNITY_EDITOR
        if (interactButton == null)
        {
            interactButton = GameObject.Find("JoystickInteract").GetComponent<Button>();
        }
        #endif
        
        interactButton.onClick.AddListener(delegate { OnButtonTapped(); });
    }

    public void OnButtonTapped()
    {
        if (m_ontriggerEntered)
        {
            m_canPickup = true;
        }
    }
}
