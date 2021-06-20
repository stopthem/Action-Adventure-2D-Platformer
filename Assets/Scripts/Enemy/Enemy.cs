using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Enemy : MonoBehaviour, IDamageableEnemy<float>, IKillable
{
    protected EnemyAnimation m_enemyAnimation;
    protected EnemyMovement m_enemyMovement;

    [Header("General")]
    [SerializeField] protected float health;
    protected float currentHealth;
    [SerializeField] protected float enemyDamage;
    [SerializeField] protected float enemyRange;
    protected float knockbackIteration;

    [HideInInspector] public bool isDead;
    protected bool canDamage;
    [HideInInspector] public bool didTakeDamage;
    [HideInInspector] public bool isTakingHit;
    [HideInInspector] public bool isAttacking;

    protected Rigidbody2D theRB2D;

    [Header("Attacking")]
    [SerializeField] protected Transform hitPoint;
    [SerializeField] protected LayerMask playerLayer;

    [Header("DamagePopup")]
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private Transform damagePopupTransform;

    protected Collider2D[] colliders;

    [Header("Blood Animation")]
    [SerializeField] private GameObject bloodAnimation;

    private void Awake()
    {
        m_enemyMovement = GetComponent<EnemyMovement>();

        theRB2D = GetComponent<Rigidbody2D>();

        m_enemyAnimation = GetComponent<EnemyAnimation>();

        colliders = GetComponents<Collider2D>();
    }

    private void Start()
    {
        bloodAnimation.SetActive(false);
        currentHealth = health;
    }


    public virtual void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        StartCoroutine(m_enemyAnimation.AttackAnimationRoutine());

        // waiting untill a spesific animation frame
        while (canDamage == false)
        {
            yield return null;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, enemyRange, playerLayer);

        if (hitEnemies != null && canDamage)
        {
            for (int i = 0; i < hitEnemies.Length; i++)
            {
                hitEnemies[i].gameObject.GetComponent<PlayerController>().Damage(enemyDamage, transform.position);
            }
        }
        canDamage = false;
    }

    // animation event
    private void CanDamage()
    {
        canDamage = true;
    }

    public void Damage(float damageTaken)
    {
        didTakeDamage = true;

        if (!PlayerController.Instance.isDashAttacking)
        {
            knockbackIteration++;
        }
        else
        {
            knockbackIteration = 2;
        }

        currentHealth -= damageTaken;

        if (!isDead)
        {
            HandlePopup(damageTaken);
        }


        if (bloodAnimation != null)
        {
            StartCoroutine(PlayBloodAnim());
        }

        if (m_enemyMovement.canKnockBack && knockbackIteration == 2)
        {
            knockbackIteration = 0;
            m_enemyMovement.m_canKnockBack = true;
        }

        if (m_enemyAnimation.takeHitAnimation != null)
        {
            m_enemyMovement.horizontalMove = 0;
            m_enemyMovement.moveDirection = Vector3.zero;
            StartCoroutine(m_enemyAnimation.TakeHitAnimRoutine());
        }

        if (currentHealth <= 0)
        {
            Killed();
        }
    }

    private void HandlePopup(float damageTaken)
    {
        GameObject damagePopupG = Instantiate(damagePopup, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        damagePopupG.GetComponent<TextMeshPro>().text = damageTaken.ToString();
    }

    private IEnumerator PlayBloodAnim()
    {
        bloodAnimation.SetActive(true);
        Animator bloodAnimator = bloodAnimation.GetComponent<Animator>();
        bloodAnimator.SetTrigger("Blood");

        yield return new WaitForSeconds(.35f);

        didTakeDamage = false;

        bloodAnimation.SetActive(false);

    }

    public virtual void Killed()
    {
        m_enemyMovement.isWalking = false;
        isAttacking = false;

        m_enemyAnimation.DeathAnim();

        isDead = true;

        StartCoroutine(m_enemyMovement.DeathSequence());

        DisableColliders();

        if (gameObject.name == "Worm" || gameObject.name == "Worm (1)")
        {
            GameManager.Instance.wormsKilled++;
        }
    }

    private void DisableColliders()
    {
        theRB2D.isKinematic = true;
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public float GetHealthNormalized()
    {
        return currentHealth / health;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
