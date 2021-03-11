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

    protected bool canMove = true;
    protected bool targetIsPlayer;
    [HideInInspector] public bool isDead;
    protected bool canDamage;
    protected bool m_canKnockBack;

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
    protected Vector2 moveDirection;
    protected Vector2 oldTarget;
    protected float distanceToA;
    protected float distanceToB;

    protected SpriteRenderer enemySprite;

    protected virtual void Awake()
    {
        theRB2D = GetComponent<Rigidbody2D>();

        enemyTransform = GetComponent<Transform>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        enemySprite = GetComponent<SpriteRenderer>();

        m_enemyAnimation = GetComponent<EnemyAnimation>();

        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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
                m_canKnockBack = false;
            }
            if (playerTransform.position.x > transform.position.x && !isDead)
            {
                StartCoroutine(KnockbackRoutine(-knockbackPower));
                m_canKnockBack = false;
            }
        }
    }

    private IEnumerator KnockbackRoutine(float power)
    {
        m_horizontalMove = power;

        yield return new WaitForSeconds(.2f);
        m_horizontalMove = power / 2;

        yield return new WaitForSeconds(.2f);
        m_horizontalMove = power / 5;

        yield return new WaitForSeconds(.2f);
        m_horizontalMove = 0;
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

            if (!m_enemyAnimation.GetBool("Walking"))
            {
                moveDirection = Vector2.zero;
            }

            if (targetIsPlayer && isAggroMovement)
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

            if (!m_enemyAnimation.GetBool("Attacking"))
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

            if (canMove && !m_enemyAnimation.GetBool("Attacking"))
            {
                m_enemyAnimation.Walking(true);
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
                m_enemyAnimation.Walking(false);
            }

            if (isWaypointMovement || m_playerController.isDead)
            {
                targetIsPlayer = false;
            }
        }

        if (distanceToPlayer <= rangeToAttack && !m_playerController.isDead)
        {
            if (!m_enemyAnimation.GetBool("Attacking"))
            {
                m_horizontalMove = 0;
                Attack();
            }
        }
        else if (m_playerController.isDead && isAggroMovement && !isWaypointMovement)
        {
            m_enemyAnimation.Walking(false);
        }

        if (!m_enemyAnimation.GetBool("Attacking") && isAggroMovement && !isWaypointMovement || targetIsPlayer && !m_playerController.isDead)
        {
            if (m_enemyAnimation.GetBool("Walking"))
            {
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
    }

    private void HandleWaypointMovement()
    {
        if (m_enemyAnimation.GetBool("Idle") && canMove)
        {
            StartCoroutine(m_enemyAnimation.WaypointRoutine());
        }

        distanceToA = Vector2.Distance(transform.position, pointA.position);
        distanceToB = Vector2.Distance(transform.position, pointB.position);

        if (distanceToA < 0.5f && !targetIsPlayer)
        {

            target = pointB.position;
            oldTarget = target;
            m_enemyAnimation.Idle(true);
        }
        else if (distanceToB < 0.5f && !targetIsPlayer)
        {
            target = pointA.position;
            oldTarget = target;
            m_enemyAnimation.Idle(true);
        }
        // when player lefts aggro we use last target before player.
        if (Vector2.Distance(target, transform.position) < 1f || distanceToA < distanceToPlayer && distanceToB < distanceToPlayer)
        {
            target = oldTarget;
        }

        if (m_enemyAnimation.GetBool("Walking") && !m_enemyAnimation.GetBool("TakeHit"))
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
            m_enemyAnimation.Walking(false);
            if (isWaypointMovement)
            {
                m_enemyAnimation.Idle(true);

                if (!targetIsPlayer)
                {
                    target = oldTarget;
                    canMove = true;
                    m_enemyAnimation.Idle(false);
                    m_enemyAnimation.Walking(true);
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
                m_enemyAnimation.Idle(false);
                m_enemyAnimation.Walking(true);
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

        if (canKnockBack)
        {
            m_canKnockBack = true;
        }

        if (m_enemyAnimation.takeHitAnimation != null)
        {
            StartCoroutine(m_enemyAnimation.TakeHitRoutine());
        }

        if (currentHealth <= 0)
        {
            Killed();
        }
    }

    public void Killed()
    {
        m_enemyAnimation.DeathAnim();

        isDead = true;

        theRB2D.velocity = Vector2.zero;

        Destroy(gameObject, 5f);
    }
}
