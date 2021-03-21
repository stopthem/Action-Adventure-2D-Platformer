using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable<float>, IKillable
{
    protected EnemyAnimation m_enemyAnimation;
    protected PlayerController m_playerController;
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

    protected Collider2D[] colliders;

    [SerializeField] private GameObject bloodAnimation;

    private void Awake()
    {
        m_enemyMovement = GetComponent<EnemyMovement>();

        theRB2D = GetComponent<Rigidbody2D>();

        m_enemyAnimation = GetComponent<EnemyAnimation>();

        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        colliders = GetComponents<Collider2D>();
    }

    private void Start()
    {
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

        if (!m_playerController.isDashAttacking)
        {
            knockbackIteration++;
        }
        else
        {
            knockbackIteration = 2;
        }

        currentHealth -= damageTaken;

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
        m_enemyAnimation.DeathAnim();

        isDead = true;

        theRB2D.velocity = Vector2.zero;

        Destroy(gameObject, 5f);

        DisableColliders();
    }

    protected void DisableColliders()
    {
        theRB2D.isKinematic = true;
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void Damage(float damageTaken, Vector3 whoDamaged)
    {
        // not usable right now.
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
