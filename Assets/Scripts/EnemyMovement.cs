using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Enemy m_enemy;
    private EnemyAnimation m_enemyAnimation;
    private PlayerController m_playerController;
    [SerializeField] protected float speed;

    [Header("Waypoint Movement")]
    [SerializeField] protected bool isWaypointMovement;
    [SerializeField] protected Transform pointA, pointB;

    [Header("Aggro Movement")]
    [SerializeField] protected bool isAggroMovement;
    [SerializeField] protected float rangeToAggro;
    [SerializeField] protected float rangeToAttack;

    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isIdle;
    [HideInInspector] public bool targetIsPlayer;
    protected bool canMove = true;

    [Header("Knockback")]
    public bool canKnockBack;
    [HideInInspector] public bool m_canKnockBack;
    [SerializeField] protected float knockbackPower;

    protected float distanceToPlayer;
    [HideInInspector] public float horizontalMove;
    protected float distanceToA;
    protected float distanceToB;
    protected float waypointIteration;


    protected Transform playerTransform;
    protected Transform enemyTransform;

    private Rigidbody2D theRB2D;

    protected SpriteRenderer enemySprite;

    protected Vector3 target;
    [HideInInspector] public Vector3 moveDirection;
    protected Vector3 oldTarget;

    private void Awake()
    {
        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        theRB2D = gameObject.GetComponent<Rigidbody2D>();

        enemySprite = gameObject.GetComponent<SpriteRenderer>();

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

        if (enemySprite.isVisible && !m_enemy.isDead)
        {
            if (isWaypointMovement && !targetIsPlayer && m_enemyAnimation.idleAnimation != null)
            {
                HandleWaypointMovement();
            }

            if (isAggroMovement)
            {
                HandleAggroMovement();
            }

            if (m_enemy.isAttacking || m_enemy.isDead || isIdle)
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

            if (isWaypointMovement || m_playerController.isDead)
            {
                targetIsPlayer = false;
                horizontalMove = 0;
                theRB2D.velocity = Vector2.zero;
            }
        }

        if (distanceToPlayer <= rangeToAttack && !m_playerController.isDead && !m_enemy.isAttacking && !m_enemy.isTakingHit)
        {
            horizontalMove = 0;
            m_enemy.Attack();
        }
        // else if (m_playerController.isDead && isAggroMovement && !isWaypointMovement)
        // {
        //     isWalking = false;
        // }

        if (!m_enemy.isAttacking && !m_playerController.isDead && isWalking && !m_enemy.isTakingHit)
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
        else if (m_playerController.isDead)
        {
            target = oldTarget;
        }

        if (isWalking && !m_enemy.isTakingHit)
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

}
