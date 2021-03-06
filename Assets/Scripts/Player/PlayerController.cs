﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerController : MonoBehaviour, IDamageable<float>, IKillable
{
    public static PlayerController Instance { get; private set; }

    // public InputDetection inputDetection;

    [HideInInspector] public CapsuleCollider2D capsuleCollider;

    [SerializeField] private GameObject bloodAnimation;


    [Header("Moving Atack")]
    [SerializeField] private float speedAfterMovingAttack;
    [SerializeField] private float dashAttackDamage;

    [Header("General")]
    [SerializeField] private float attackRange;
    public float damage;
    public float health;
    [SerializeField] private float knockBackPowerHit;
    public float invincibleDuration;
    [SerializeField] private float timeToWaitAfterDeath;
    [HideInInspector] public float currentHealth;
    private float m_knockbackDirection;
    [Header("CameraShake")]
    [SerializeField] private float cameraShakeIntensity;
    [SerializeField] private float cameraShakeTime;

    private int enemyLayerIndex;
    private int playerLayerIndex;

    private bool m_candamage;
    private bool m_canTakeDamage = true;
    private bool m_poisonedDamage;
    [HideInInspector] public bool canOpenChest;
    [HideInInspector] public bool invincible;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isDashAttacking;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public bool isCriticalHealth;

    [Header("Attacking")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Damage Popup")]
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private Transform damagePopupTransform;

    [HideInInspector] public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Instance = this;

        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
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
            else
            {
                isCriticalHealth = false;
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
        if (m_canTakeDamage)
        {
            isPoisoned = true;
            CameraPostProcess.Instance.Poisoned(true);
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
            CameraPostProcess.Instance.Poisoned(false);
            isPoisoned = false;
        }

    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !PlayerMovement.Instance.isDashing)
        {
            StartCoroutine(AttackRoutine());
        }

        if (PlayerMovement.Instance.isDashing && Input.GetKeyDown(KeyCode.Space) && PlayerMovement.Instance.isGrounded)
        {
            isDashAttacking = true;
        }

        if (isDashAttacking)
        {
            if (PlayerMovement.Instance.direction == 1)
            {
                PlayerMovement.Instance.horizontalMove = -speedAfterMovingAttack;
            }
            else if (PlayerMovement.Instance.direction == 2)
            {
                PlayerMovement.Instance.horizontalMove = speedAfterMovingAttack;
            }

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
            PlayerAnimation.Instance.AttackAnim();
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
        yield return new WaitForSeconds(PlayerAnimation.Instance.dashAttackAnimation.length);
        yield return new WaitForSeconds(.5f);
        capsuleCollider.enabled = true;
    }

    //animation event for attack
    private void CanDamage()
    {
        m_candamage = true;
    }
    private void StopDashAttack()
    {
        isDashAttacking = false;
    }

    public void Damage(float damageTaken)
    {
        if (m_canTakeDamage)
        {
            isDashAttacking = false;

            CinemachineShake.Instance.ShakeCamera(cameraShakeIntensity, cameraShakeTime);

            currentHealth -= damageTaken;

            if (!isDead)
            {
                HandlePopup(damageTaken);
            }


            if (PlayerMovement.Instance.canKnockBack == false && !m_poisonedDamage)
            {
                PlayerMovement.Instance.canKnockBack = true;
            }

            if (PlayerMovement.Instance.direction == 1 && !m_poisonedDamage && PlayerMovement.Instance.canKnockBack)
            {
                StartCoroutine(PlayerMovement.Instance.KnockbackRoutine(-knockBackPowerHit));
            }
            else if (PlayerMovement.Instance.direction == 2 && !m_poisonedDamage && PlayerMovement.Instance.canKnockBack)
            {
                StartCoroutine(PlayerMovement.Instance.KnockbackRoutine(knockBackPowerHit));
            }

            if (!isPoisoned)
            {
                StartCoroutine(Invincible(invincibleDuration));
            }

            StartCoroutine(PlayBloodAnim());

            if (!isPoisoned)
            {
                PlayerAnimation.Instance.TakeHit();
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

            isDashAttacking = false;

            CinemachineShake.Instance.ShakeCamera(cameraShakeIntensity, cameraShakeTime);

            currentHealth -= damageTaken;
            HandlePopup(damageTaken);

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

            PlayerMovement.Instance.HandleHitKnockback(m_knockbackDirection, m_poisonedDamage, knockBackPowerHit);

            if (!isPoisoned)
            {
                StartCoroutine(Invincible(invincibleDuration));
            }

            StartCoroutine(PlayBloodAnim());

            if (!isPoisoned)
            {
                PlayerAnimation.Instance.TakeHit();
            }

            if (currentHealth <= 0)
            {
                Killed();
            }
        }
    }

    private void HandlePopup(float damageTaken)
    {
        GameObject damagePopupG = Instantiate(damagePopup, damagePopupTransform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        damagePopupG.GetComponent<TextMeshPro>().text = damageTaken.ToString();

        if (isPoisoned)
        {
            damagePopupG.GetComponent<TextMeshPro>().color = Color.green;
        }
        else
        {
            damagePopupG.GetComponent<TextMeshPro>().color = Color.yellow;
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
        StartCoroutine(KilledRoutine());
    }

    private IEnumerator KilledRoutine()
    {
        PlayerAnimation.Instance.DeathAnimation();
        isDead = true;
        yield return new WaitForSeconds(timeToWaitAfterDeath);
        yield return SceneHandler.Instance.FadeImageRoutine(false);
        UIHandler.Instance.ShowFailedScreen(true);
    }

    public float GetHealthNormalized()
    {
        return currentHealth / health;
    }

    //ONCLICK
    public void DashAttackButton()
    {
        PlayerMovement.Instance.dashButtonUsed++;

        if (PlayerMovement.Instance.isDashing && PlayerMovement.Instance.dashButtonUsed == 3 && PlayerMovement.Instance.isGrounded)
        {
            PlayerMovement.Instance.dashButtonUsed = 0;
            isDashAttacking = true;
            HandleAttack();
            UIHandler.Instance.DashButton(false, false);
        }
    }
    //ONCLICK
    public void AttackButton()
    {
        if (!PlayerMovement.Instance.isDashing)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    //ONCLICK
    public void InteractButton()
    {
        canOpenChest = true;
    }
}
