using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable<float>, IKillable
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


    [HideInInspector] public int attackItaration;

    protected float distanceToPlayer;

    protected bool canMove = true;
    protected bool targetIsPlayer = false;
    protected bool isDead = false;

    protected Rigidbody2D theRB2D;
    protected Transform enemyTransform;
    protected SpriteRenderer spriteRenderer;

    private EnemyAnimation m_enemyAnimation;

    protected Transform playerTransform;

    protected Vector3 target;
    protected Vector3 moveDirection;
    protected Vector3 oldTarget;
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

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Start()
    {
        currentHealth = health;
    }

    public abstract void Attack();

    protected virtual void Update()
    {
        HandleMovement();

        HandleDirection();

        if (m_enemyAnimation.GetBool("Attacking"))
        {
            theRB2D.velocity = Vector2.zero;
        }

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

            moveDirection.Normalize();
            theRB2D.velocity = moveDirection * speed;
        }
    }

    private void HandleAggroMovement()
    {
        if (distanceToPlayer <= rangeToAggro)
        {
            if (playerTransform.position.x < transform.position.x)
            {
                enemyTransform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (playerTransform.position.x > transform.position.x)
            {
                enemyTransform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (canMove && !m_enemyAnimation.GetBool("Attacking"))
            {
                m_enemyAnimation.Walking(true);

                if (isWaypointMovement)
                {
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

            if (isWaypointMovement)
            {
                targetIsPlayer = false;
            }
        }

        if (distanceToPlayer <= rangeToAttack)
        {
            attackItaration++;
            if (attackItaration == 1)
            {
                StartCoroutine(m_enemyAnimation.AttackAnimationRoutine());
            }
        }
        if (isAggroMovement && !isWaypointMovement || targetIsPlayer)
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

        if (playerTransform.position.x < transform.position.x)
        {
            enemyTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (playerTransform.position.x > transform.position.x)
        {
            enemyTransform.rotation = Quaternion.Euler(0, 0, 0);
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

    public void Damage(float damageTaken)
    {
        currentHealth -= damageTaken;
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
        isDead = true;
        m_enemyAnimation.DeathAnim();
        Destroy(gameObject, 5f);
    }
}
