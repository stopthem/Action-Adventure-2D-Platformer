using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private BoxCollider2D m_boxCollider;

    private Animator m_animator;

    private bool m_canPickup;
    private bool m_isChestPickUp;

    public float chestCoinTimeToPickUp;
    public float pickupAmount;
    public float chestPickupAmount;

    public LayerMask groundedLayer;

    private Rigidbody2D m_rigidBody;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_boxCollider.bounds.center, m_boxCollider.bounds.size, 0f, Vector2.down, .1f, groundedLayer.value);
        Debug.DrawRay(m_boxCollider.bounds.center, Vector2.down * .5f, Color.green);


        if (hitInfo.collider != null)
        {
            m_rigidBody.velocity = Vector2.zero;
            m_rigidBody.isKinematic = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (m_isChestPickUp && m_canPickup)
            {
                GameManager.Instance.AddCoin(chestPickupAmount);
                PlayerAnimation.Instance.CoinPickupAnim();
                Destroy(gameObject);
            }
            else if (!m_isChestPickUp)
            {
                GameManager.Instance.AddCoin(pickupAmount);
                PlayerAnimation.Instance.CoinPickupAnim();
                Destroy(gameObject);
            }
        }
    }

    public void ForceCoin()
    {
        StartCoroutine(ForceCoinRoutine());
    }

    private IEnumerator ForceCoinRoutine()
    {
        m_isChestPickUp = true;

        m_canPickup = false;

        m_rigidBody.velocity = Vector2.up * 3;

        yield return new WaitForSeconds(chestCoinTimeToPickUp);

        m_canPickup = true;
    }
}
