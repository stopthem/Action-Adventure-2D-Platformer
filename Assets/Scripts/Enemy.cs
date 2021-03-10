using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable<float>, IKillable
{
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
    [SerializeField] protected float enemyDamage;
    [SerializeField] protected float enemyRange;

    protected bool canMove = true;
    protected bool targetIsPlayer;
    [HideInInspector] public bool isDead;
    protected bool canDamage;
    protected bool canKnockBack;
    protected Rigidbody2D theRB2D;
    protected SpriteRenderer spriteRenderer;

    protected EnemyAnimation m_enemyAnimation;
    protected PlayerController m_playerController;

    protected Transform playerTransform;
    protected Transform enemyTransform;
    [SerializeField] protected Transform hitPoint;

    protected Vector3 target;
    protected Vector3 moveDirection;
    protected Vector3 oldTarget;
    protected float distanceToA;
    protected float distanceToB;

    protected SpriteRenderer enemySprite;

    [SerializeField] protected LayerMask playerLayer;

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
            moveDirection = Vector2.zero;

            if (isWaypointMovement && !targetIsPlayer && m_enemyAnimation.idleAnimation != null)
            {
                HandleWaypointMovement();
            }

            if (isAggroMovement)
            {
                HandleAggroMovement();
            }
        }
        moveDirection.Normalize();
        theRB2D.velocity = moveDirection * speed;
    }

    private void HandleDirection()
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

                if (isWaypointMovement)
                {
                    // saving the last target before player for waypointmovement
                    if (target == pointA.position || target == pointB.position)
                    {
                        oldTarget = target;
                    }

                    target = playerTransform.position;
                    targetIsPlayer = true;
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
                moveDirection = playerTransform.position - transform.position;
            }
        }

        moveDirection.Normalize();

    }

    private void HandleWaypointMovement()
    {
        if (m_enemyAnimation.GetBool("Idle") && canMove)
        {
            StartCoroutine(m_enemyAnimation.WaypointRoutine());
        }

        if (isDead)
        {
            target = Vector3.zero;
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

        if (m_enemyAnimation.GetBool("Walking"))
        {
            moveDirection = target - transform.position;
            moveDirection.Normalize();
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
        canKnockBack = true;

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
        Destroy(gameObject, 5f);
    }
}
