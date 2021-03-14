using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable<float>, IKillable
{
    [Header("General")]
    [SerializeField] protected float speed;
    [SerializeField] protected float health;
    protected float currentHealth;

    [Header("Waypoint Movement")]
    [SerializeField] protected bool isWaypointMovement;
    [SerializeField] protected Transform pointA, pointB;

    [Header("Aggro Movement")]
    [SerializeField] protected bool isAggroMovement;
    [SerializeField] protected float rangeToAggro;
    [SerializeField] protected float rangeToAttack;

    protected float distanceToPlayer;
    protected float m_horizontalMove;
    [SerializeField] protected float enemyDamage;
    [SerializeField] protected float enemyRange;
    protected float iteration;

    protected bool canMove = true;
    protected bool targetIsPlayer;
    [HideInInspector] public bool isDead;
    protected bool canDamage;
    protected bool m_canKnockBack;
    [HideInInspector] public bool isTakingHit;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isIdle;

    [Header("Knockback")]
    public bool canKnockBack;
    [SerializeField] protected float knockbackPower;


    protected Rigidbody2D theRB2D;
    protected SpriteRenderer spriteRenderer;

    protected EnemyAnimation m_enemyAnimation;
    protected PlayerController m_playerController;

    protected Transform playerTransform;
    protected Transform enemyTransform;

    [Header("Attacking")]
    [SerializeField] protected Transform hitPoint;
    [SerializeField] protected LayerMask playerLayer;

    protected Vector3 target;
    protected Vector3 moveDirection;
    protected Vector3 oldTarget;
    protected float distanceToA;
    protected float distanceToB;

    protected SpriteRenderer enemySprite;
    protected Collider2D[] colliders;

    public GameObject bloodAnimation;

    protected virtual void Awake()
    {
        theRB2D = GetComponent<Rigidbody2D>();

        enemyTransform = GetComponent<Transform>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        enemySprite = GetComponent<SpriteRenderer>();

        m_enemyAnimation = GetComponent<EnemyAnimation>();

        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        colliders = GetComponents<Collider2D>();
    }

    private void Start()
    {
        currentHealth = health;
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
        HandleDirection();
        HandleKnockBack();
    }

    private void HandleKnockBack()
    {
        if (m_canKnockBack)
        {
            if (playerTransform.position.x < transform.position.x && !isDead)
            {
                StartCoroutine(KnockbackRoutine(knockbackPower));
            }
            if (playerTransform.position.x > transform.position.x && !isDead)
            {
                StartCoroutine(KnockbackRoutine(-knockbackPower));
            }

            theRB2D.velocity = new Vector2(m_horizontalMove, theRB2D.velocity.y);
        }
    }

    private IEnumerator KnockbackRoutine(float power)
    {
        m_horizontalMove = power;

        yield return new WaitForSeconds(.1f);
        m_horizontalMove = m_horizontalMove / 2;

        yield return new WaitForSeconds(.1f);
        m_horizontalMove = m_horizontalMove / 5;

        yield return new WaitForSeconds(.1f);
        m_horizontalMove = 0;
        theRB2D.velocity = Vector2.zero;

        m_canKnockBack = false;
    }

    protected virtual void Attack()
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
            foreach (var player in hitEnemies)
            {
                player.gameObject.GetComponent<PlayerController>().Damage(enemyDamage);
            }
        }
        canDamage = false;
        

    }

    private void HandleMovement()
    {
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (enemySprite.isVisible && playerTransform.gameObject.activeInHierarchy && !isDead)
        {
            if (isWaypointMovement && !targetIsPlayer && m_enemyAnimation.idleAnimation != null)
            {
                HandleWaypointMovement();
            }

            if (isAggroMovement)
            {
                HandleAggroMovement();
            }

            if (isAttacking || isDead)
            {
                theRB2D.velocity = Vector2.zero;
                moveDirection = Vector3.zero;
                isWalking = false;
            }
            if (targetIsPlayer && isAggroMovement && !isAttacking && !isTakingHit)
            {
                theRB2D.velocity = new Vector2(m_horizontalMove * speed, theRB2D.velocity.y);
            }
            else if (!targetIsPlayer && isWaypointMovement)
            {
                moveDirection.Normalize();
                theRB2D.velocity = moveDirection * speed;
            }
        }
    }

    private void HandleDirection()
    {
        if (isWaypointMovement)
        {
            if (theRB2D.velocity.x < 0)
            {
                enemyTransform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (theRB2D.velocity.x > 0)
            {
                enemyTransform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void HandleAggroMovement()
    {
        if (distanceToPlayer <= rangeToAggro && !m_playerController.isDead)
        {

            if (!isAttacking)
            {
                if (playerTransform.position.x < transform.position.x)
                {
                    enemyTransform.rotation = Quaternion.Euler(0, 180, 0);
                }
                if (playerTransform.position.x > transform.position.x)
                {
                    enemyTransform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }

            if (canMove && !isAttacking && !isTakingHit)
            {
                isWalking = true;
                targetIsPlayer = true;

                if (isWaypointMovement)
                {
                    // saving the last target before player for waypointmovement
                    if (target == pointA.position || target == pointB.position)
                    {
                        oldTarget = target;
                    }

                    target = playerTransform.position;
                }
            }
        }
        else
        {
            if (isAggroMovement && !isWaypointMovement)
            {
                isWalking = false;
            }

            if (isWaypointMovement || m_playerController.isDead)
            {
                targetIsPlayer = false;
            }
        }

        if (distanceToPlayer <= rangeToAttack && !m_playerController.isDead && !isAttacking && !isTakingHit)
        {
            m_horizontalMove = 0;
            Attack();
        }
        // else if (m_playerController.isDead && isAggroMovement && !isWaypointMovement)
        // {
        //     isWalking = false;
        // }

        if (!isAttacking && !m_playerController.isDead && isWalking && !isTakingHit)
        {
            isIdle = false;

            if (playerTransform.position.x < transform.position.x)
            {
                m_horizontalMove = -speed;
            }
            if (playerTransform.position.x > transform.position.x)
            {
                m_horizontalMove = speed;
            }
        }
        else if (!isWaypointMovement || targetIsPlayer)
        {
            isIdle = false;

            if (playerTransform.position.x < transform.position.x)
            {
                m_horizontalMove = -speed;
            }
            if (playerTransform.position.x > transform.position.x)
            {
                m_horizontalMove = speed;
            }
        }
    }

    private void HandleWaypointMovement()
    {

        distanceToA = Vector2.Distance(transform.position, pointA.position);
        distanceToB = Vector2.Distance(transform.position, pointB.position);

        if (distanceToA < 0.5f && !targetIsPlayer)
        {
            target = pointB.position;
            oldTarget = target;

            while (iteration == 0)
            {
                if (canMove)
                {
                    StartCoroutine(m_enemyAnimation.WaypointAnimRoutine());
                    iteration++;
                }
            }
        }
        else if (distanceToB < 0.5f && !targetIsPlayer)
        {
            target = pointA.position;
            oldTarget = target;

            while (iteration == 0)
            {
                if (canMove)
                {
                    StartCoroutine(m_enemyAnimation.WaypointAnimRoutine());
                    iteration++;
                }
            }
        }
        else
        {
            iteration = 0;
        }
        // when player lefts aggro we use last target before player.
        if (Vector2.Distance(target, transform.position) < 1f || distanceToA < distanceToPlayer && distanceToB < distanceToPlayer)
        {
            target = oldTarget;
        }

        if (isWalking && !isTakingHit)
        {
            moveDirection = target - transform.position;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // keeping enemies from going to edges
        if (other.gameObject.CompareTag("enemyWall"))
        {
            canMove = false;
            isWalking = false;
            if (isWaypointMovement)
            {
                isIdle = true;

                if (!targetIsPlayer)
                {
                    target = oldTarget;
                    canMove = true;
                    isIdle = false;
                    isWalking = true;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("enemyWall"))
        {
            canMove = true;
            if (isWaypointMovement)
            {
                isIdle = false;
                isWalking = true;
            }
        }
    }

    private void CanDamage()
    {
        canDamage = true;
    }

    public void Damage(float damageTaken)
    {
        currentHealth -= damageTaken;

        if (bloodAnimation != null)
        {
            StartCoroutine(PlayBloodAnim());
        }

        if (canKnockBack)
        {
            m_canKnockBack = true;
        }

        if (m_enemyAnimation.takeHitAnimation != null)
        {
            m_horizontalMove = 0;
            moveDirection = Vector3.zero;
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
}
