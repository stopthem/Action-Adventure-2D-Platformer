using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable<float>, IKillable
{
    // public InputDetection inputDetection;

    private PlayerAnimation m_playerAnimation;
    private PlayerMovement m_playerMovement;

    [HideInInspector] public CapsuleCollider2D capsuleCollider;

    [SerializeField] private GameObject bloodAnimation;


    [Header("Moving Atack")]
    [SerializeField] private float speedAfterMovingAttack;
    [SerializeField] private float dashAttackDamage;

    [Header("General")]
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float health;
    [SerializeField] private float knockBackPowerHit;
    [SerializeField] private float invincibleDuration;
    private float currentHealth;
    private float m_knockbackDirection;

    private int enemyLayerIndex;
    private int playerLayerIndex;

    private bool m_candamage;
    private bool m_canTakeDamage = true;
    private bool m_poisonedDamage;
    [HideInInspector] public bool invincible;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isDashAttacking;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public bool isCriticalHealth;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    private void Awake()
    {
        m_playerAnimation = GetComponent<PlayerAnimation>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        m_playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        playerLayerIndex = LayerMask.NameToLayer("Player");
        enemyLayerIndex = LayerMask.NameToLayer("Enemy");

        currentHealth = health;
    }

    private void Update()
    {
        if (!isDead)
        {
            HandleAttack();

            if (currentHealth <= (health * 20) / 100)
            {
                isCriticalHealth = true;
            }
        }
        else
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isCriticalHealth = false;

        isPoisoned = false;
    }

    public IEnumerator IsPoisonedRoutine(float perTick, float duration, Vector3 position, float damage)
    {
        Damage(damage, position);

        float durationPerTick = duration / perTick;

        for (int i = 1; i <= durationPerTick; i++)
        {

            m_poisonedDamage = true;
            Damage(.5f);
            if (isDead)
            {
                isPoisoned = false;
                break;
            }
            m_poisonedDamage = false;
            yield return new WaitForSeconds(perTick);

        }

        isPoisoned = false;
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !m_playerMovement.isDashing)
        {
            StartCoroutine(AttackRoutine());
        }

        if (m_playerMovement.isDashing && Input.GetKeyDown(KeyCode.Space) && m_playerMovement.isGrounded)
        {
            isDashAttacking = true;
        }

        if (isDashAttacking)
        {
            if (m_playerMovement.direction == 1)
            {
                m_playerMovement.horizontalMove = -speedAfterMovingAttack;
            }
            else if (m_playerMovement.direction == 2)
            {
                m_playerMovement.horizontalMove = speedAfterMovingAttack;
            }

            isDashAttacking = true;

            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        if (isDashAttacking)
        {
            StartCoroutine(IsMovingAttackRoutine());
        }
        else
        {
            m_playerAnimation.AttackAnim();
        }

        // waits untill specific attack animation frame
        while (m_candamage == false)
        {
            yield return null;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies != null && m_candamage && isAttacking || isDashAttacking)
        {
            for (int i = 0; i < hitEnemies.Length; i++)
            {
                if (isDashAttacking)
                {
                    hitEnemies[i].gameObject.GetComponent<Enemy>().Damage(dashAttackDamage);
                }
                else
                {
                    hitEnemies[i].gameObject.GetComponent<Enemy>().Damage(damage);
                }

            }

            // waits for second spesific animation frame and damages twice
            if (!isDashAttacking)
            {
                m_candamage = false;

                while (m_candamage == false)
                {
                    yield return null;
                }

                hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

                if (m_candamage && hitEnemies != null)
                {
                    for (int i = 0; i < hitEnemies.Length; i++)
                    {
                        hitEnemies[i].GetComponent<Enemy>().Damage(damage);
                    }
                }
            }
        }
        m_candamage = false;
    }

    private IEnumerator IsMovingAttackRoutine()
    {
        capsuleCollider.enabled = false;
        yield return m_playerAnimation.MovingAttackRoutine();
        yield return new WaitForSeconds(.5f);
        capsuleCollider.enabled = true;
    }

    //animation event for attack
    public void CanDamage()
    {
        m_candamage = true;
    }

    public void Damage(float damageTaken)
    {
        if (m_canTakeDamage)
        {
            currentHealth -= damageTaken;

            if (m_playerMovement.canKnockBack == false && !m_poisonedDamage)
            {
                m_playerMovement.canKnockBack = true;
            }

            if (m_playerMovement.direction == 1 && !m_poisonedDamage && m_playerMovement.canKnockBack)
            {
                StartCoroutine(m_playerMovement.KnockbackRoutine(-knockBackPowerHit));
            }
            else if (m_playerMovement.direction == 2 && !m_poisonedDamage && m_playerMovement.canKnockBack)
            {
                StartCoroutine(m_playerMovement.KnockbackRoutine(knockBackPowerHit));
            }

            if (!isPoisoned)
            {
                StartCoroutine(Invincible(invincibleDuration));
            }

            StartCoroutine(PlayBloodAnim());

            if (!isPoisoned)
            {
                m_playerAnimation.TakeHit();
            }

            if (currentHealth <= 0)
            {
                Killed();
            }
        }
    }

    public void Damage(float damageTaken, Vector3 whoDamaged)
    {
        if (m_canTakeDamage)
        {
            currentHealth -= damageTaken;

            if (whoDamaged != transform.position)
            {
                if (whoDamaged.x < transform.position.x)
                {
                    m_knockbackDirection = 1;
                }
                else if (whoDamaged.x > transform.position.x)
                {
                    m_knockbackDirection = 2;
                }
            }

            m_playerMovement.HandleHitKnockback(m_knockbackDirection, m_poisonedDamage, knockBackPowerHit);

            if (!isPoisoned)
            {
                StartCoroutine(Invincible(invincibleDuration));
            }

            StartCoroutine(PlayBloodAnim());

            if (!isPoisoned)
            {
                m_playerAnimation.TakeHit();
            }

            if (currentHealth <= 0)
            {
                Killed();
            }
        }
    }

    private IEnumerator Invincible(float duration)
    {
        Physics2D.IgnoreLayerCollision(enemyLayerIndex, playerLayerIndex, true);
        invincible = true;
        m_canTakeDamage = false;


        yield return new WaitForSeconds(duration);

        m_canTakeDamage = true;
        invincible = false;
        Physics2D.IgnoreLayerCollision(enemyLayerIndex, playerLayerIndex, false);
    }

    private IEnumerator PlayBloodAnim()
    {
        bloodAnimation.SetActive(true);
        Animator bloodAnimator = bloodAnimation.GetComponent<Animator>();
        bloodAnimator.SetTrigger("Blood");

        yield return new WaitForSeconds(.2f);

        bloodAnimation.SetActive(false);
    }

    public void Killed()
    {
        m_playerAnimation.DeathAnimation();
        isDead = true;
    }

    public float GetHealthNormalized()
    {
        return currentHealth / health;
    }
}
