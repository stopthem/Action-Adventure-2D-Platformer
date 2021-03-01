using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int speed;
    [SerializeField] protected int health;
    [Header("Waypoint Movement")]
    [SerializeField] protected bool isWaypointMovement;
    [SerializeField] protected Transform pointA, pointB;
    [Header("Aggro Movement")]
    [SerializeField] protected bool isAggroMovement;
    [SerializeField] protected float rangeToAggro;
    [SerializeField] protected float rangeToAttack;

    protected int checkpoint;

    protected float distance;

    protected bool canMove = true;
    protected bool targetIsPlayer = false;

    protected Rigidbody2D theRB2D;
    protected Transform enemyTransform;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    [SerializeField] protected AnimationClip attackAnimation;

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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Attack()
    {

    }

    protected virtual void Update()
    {
        HandleMovement();

        HandleDirection();
        print(canMove);
        if (animator.GetBool("Attacking"))
        {
            theRB2D.velocity = Vector2.zero;
        }

    }

    protected void HandleDirection()
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

    protected void HandleMovement()
    {
        distance = Vector2.Distance(transform.position, playerTransform.position);

        if (enemySprite.isVisible && playerTransform.gameObject.activeInHierarchy)
        {
            moveDirection = Vector2.zero;

            if (isWaypointMovement && !targetIsPlayer)
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

    protected void HandleAggroMovement()
    {
        if (distance <= rangeToAggro)
        {
            if (playerTransform.position.x < transform.position.x)
            {
                enemyTransform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (playerTransform.position.x > transform.position.x)
            {
                enemyTransform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (canMove && !animator.GetBool("Attacking"))
            {
                animator.SetBool("Walking", true);

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
                animator.SetBool("Walking", false);
            }

            if (isWaypointMovement)
            {
                targetIsPlayer = false;
            }
        }

        if (distance <= rangeToAttack)
        {
            checkpoint++;
            if (checkpoint == 1)
            {
                StartCoroutine(AttackAnimationRoutine());
            }
        }

        if (isAggroMovement && !isWaypointMovement || targetIsPlayer)
        {
            if (animator.GetBool("Walking"))
            {
                moveDirection = playerTransform.position - transform.position;
            }
        }

        moveDirection.Normalize();

    }

    private IEnumerator AttackAnimationRoutine()
    {
        moveDirection = Vector2.zero;
        animator.SetBool("Walking", false);
        animator.SetBool("Attacking", true);
        Attack();

        yield return new WaitForSeconds(attackAnimation.length);

        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", true);
        checkpoint = 0;

    }

    private void HandleWaypointMovement()
    {
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
            animator.SetBool("Idle", true);



        }
        else if (distanceToB < 0.5f && !targetIsPlayer)
        {
            target = pointA.position;
            oldTarget = target;
            animator.SetBool("Idle", true);

        }

        if (Vector2.Distance(target, transform.position) < 1f)
        {
            target = oldTarget;
        }

        if (animator.GetBool("Walking"))
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
            animator.SetBool("Walking", false);
            if (isWaypointMovement)
            {
                animator.SetBool("Idle", true);
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
                animator.SetBool("Idle", false);
                animator.SetBool("Walking", true);
            }
        }
    }
}
