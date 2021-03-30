using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Enemy m_enemy;
    private EnemyAnimation m_enemyAnimation;
    [SerializeField] private float speed;

    [Header("Waypoint Movement")]
    [SerializeField] private bool isWaypointMovement;
    [SerializeField] private Transform pointA, pointB;

    [Header("Aggro Movement")]
    [SerializeField] private bool isAggroMovement;
    [SerializeField] private float rangeToAggro;
    [SerializeField] private float rangeToAttack;

    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isIdle;
    [HideInInspector] public bool targetIsPlayer;
    private bool canMove = true;

    [Header("Knockback")]
    public bool canKnockBack;
    [HideInInspector] public bool m_canKnockBack;
    [SerializeField] private float knockbackPower;

    private float distanceToPlayer;
    [HideInInspector] public float horizontalMove;
    private float distanceToA;
    private float distanceToB;
    private float waypointIteration;


    private Transform playerTransform;
    private Transform enemyTransform;

    private Rigidbody2D theRB2D;

    private SpriteRenderer m_spriteRenderer;

    private Vector3 target;
    [HideInInspector] public Vector3 moveDirection;
    private Vector3 oldTarget;

    private BoxCollider2D m_boxCollider2D;

    private void Awake()
    {
        m_boxCollider2D = gameObject.GetComponent<BoxCollider2D>();

        theRB2D = gameObject.GetComponent<Rigidbody2D>();

        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        enemyTransform = gameObject.GetComponent<Transform>();

        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        m_enemy = gameObject.GetComponent<Enemy>();

        m_enemyAnimation = gameObject.GetComponent<EnemyAnimation>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleDirection();
        HandleKnockBack();
    }

    private void HandleKnockBack()
    {
        if (m_canKnockBack)
        {
            if (playerTransform.position.x < transform.position.x && !m_enemy.isDead)
            {
                StartCoroutine(KnockbackRoutine(knockbackPower));
            }
            if (playerTransform.position.x > transform.position.x && !m_enemy.isDead)
            {
                StartCoroutine(KnockbackRoutine(-knockbackPower));
            }

            theRB2D.velocity = new Vector2(horizontalMove, theRB2D.velocity.y);
        }
    }

    private IEnumerator KnockbackRoutine(float power)
    {
        horizontalMove = power;

        yield return new WaitForSeconds(.1f);
        horizontalMove = horizontalMove / 2;

        yield return new WaitForSeconds(.1f);
        horizontalMove = horizontalMove / 5;

        yield return new WaitForSeconds(.1f);
        horizontalMove = 0;
        theRB2D.velocity = Vector2.zero;

        m_canKnockBack = false;

    }

    private void HandleMovement()
    {
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (m_enemy != null)
        {
            if (isWaypointMovement && !targetIsPlayer && m_enemyAnimation.idleAnimation != null && !m_enemy.isDead)
            {
                HandleWaypointMovement();
            }

            if (isAggroMovement && !m_enemy.isDead)
            {
                HandleAggroMovement();
            }

            if (m_enemy.isAttacking || isIdle)
            {
                theRB2D.velocity = Vector2.zero;
                moveDirection = Vector3.zero;
                horizontalMove = 0;
                isWalking = false;
            }
            if (targetIsPlayer && isAggroMovement && !m_enemy.isAttacking && !m_enemy.isTakingHit && !isIdle)
            {
                theRB2D.velocity = new Vector2(horizontalMove * speed, theRB2D.velocity.y);
            }
            else if (!targetIsPlayer && isWaypointMovement && !isIdle)
            {
                moveDirection.Normalize();
                theRB2D.velocity = moveDirection * speed;
            }
            // if (m_enemy.isDead)
            // {
            //     theRB2D.velocity = Vector2.down;
            // }
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
        if (distanceToPlayer <= rangeToAggro && !PlayerController.Instance.isDead)
        {

            if (!m_enemy.isAttacking)
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

            if (canMove && !m_enemy.isAttacking && !m_enemy.isTakingHit)
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

            if (isWaypointMovement || PlayerController.Instance.isDead)
            {
                targetIsPlayer = false;
                horizontalMove = 0;
                theRB2D.velocity = Vector2.zero;
            }
        }

        if (distanceToPlayer <= rangeToAttack && !PlayerController.Instance.isDead && !m_enemy.isAttacking && !m_enemy.isTakingHit)
        {
            horizontalMove = 0;
            m_enemy.Attack();
        }
        // else if (m_playerController.isDead && isAggroMovement && !isWaypointMovement)
        // {
        //     isWalking = false;
        // }

        if (!m_enemy.isAttacking && !PlayerController.Instance.isDead && isWalking && !m_enemy.isTakingHit)
        {
            isIdle = false;

            if (playerTransform.position.x < transform.position.x)
            {
                horizontalMove = -speed;
            }
            if (playerTransform.position.x > transform.position.x)
            {
                horizontalMove = speed;
            }
        }
        else if (!isWaypointMovement || targetIsPlayer)
        {
            isIdle = false;

            if (playerTransform.position.x < transform.position.x)
            {
                horizontalMove = -speed;
            }
            if (playerTransform.position.x > transform.position.x)
            {
                horizontalMove = speed;
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

            horizontalMove = 0;
            theRB2D.velocity = Vector2.zero;

            while (waypointIteration == 0)
            {
                if (canMove)
                {
                    StartCoroutine(m_enemyAnimation.WaypointAnimRoutine());
                    waypointIteration++;
                }
            }
        }
        else if (distanceToB < 0.5f && !targetIsPlayer)
        {
            target = pointA.position;
            oldTarget = target;

            horizontalMove = 0;
            theRB2D.velocity = Vector2.zero;

            while (waypointIteration == 0)
            {
                if (canMove)
                {
                    StartCoroutine(m_enemyAnimation.WaypointAnimRoutine());
                    waypointIteration++;
                }
            }
        }
        else
        {
            waypointIteration = 0;
        }
        // when player lefts aggro we use last target before player.
        if (Vector2.Distance(target, transform.position) < 1f || distanceToA < distanceToPlayer && distanceToB < distanceToPlayer)
        {
            target = oldTarget;
        }
        else if (PlayerController.Instance.isDead)
        {
            target = oldTarget;
        }

        if (isWalking && !m_enemy.isTakingHit)
        {
            moveDirection = target - transform.position;
        }
    }

    public IEnumerator DeathSequence()
    {
        m_spriteRenderer.sortingOrder = 20;
        yield return new WaitForSeconds(5f);
        m_boxCollider2D.enabled = false;
        theRB2D.velocity = Vector2.down / 5;
        Destroy(gameObject, 3f);
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

}
