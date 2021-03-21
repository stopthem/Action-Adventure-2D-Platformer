using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject coin;

    private Animator m_animator;

    private bool m_isOpened;
    private bool m_canThrow;
    private bool m_canPickup;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_canPickup = true;
        }

        if (m_isOpened)
        {
            UIHandler.Instance.ShowChestText(false);
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
            UIHandler.Instance.ShowChestText(false);
        }
    }

    // animation event
    private void ThrowCoin()
    {
        m_canThrow = true;
    }
}