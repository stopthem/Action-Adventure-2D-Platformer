using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable<float>, IKillable
{
    // public InputDetection inputDetection;

    private PlayerAnimation m_playerAnimation;
    private Enemy m_enemy;

    // private int m_direction;

    private BoxCollider2D m_boxCollider2D;

    private Rigidbody2D m_rigidBody;

    private Transform m_playerTransform;

    private Vector2 m_moveInput;
    public GameObject bloodAnimation;

    private int m_buttonUsed = 0;

    public float moveSpeed;
    public float jumpSpeed;

    [Header("Dash")]
    public float dashDuration;
    public float dashLength;
    public float dashSpeed;
    public float dashCooldown;

    [Header("Moving Atack")]
    public float speedAfterMovingAttack;
    public float movingAttackDamage;

    [Header("General")]
    public float attackRange = .5f;
    public float damage;
    public float health;
    public float knocbackPower;
    public float invincibleDuration;
    private float m_currentHealth;
    private float m_originalGravity;
    private float m_direction;
    private float m_horizontalMove;

    private int triggerIteration = 0;

    private bool m_grounded;
    private bool m_candamage;
    private bool canDash = true;
    private bool m_canKnockBack;
    private bool canTakeDamage = true;
    [HideInInspector] public bool invincible;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isMovingAttacking;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isFalling;

    public Transform attackPoint;
    public LayerMask enemyLayer;
    [SerializeField] private LayerMask m_groundedLayer;

    private CapsuleCollider2D capsuleCollider;

    private SpriteRenderer spriteRenderer;

    // private Joystick joystick;

    private bool m_moveLeft = false, m_moveRight = false;
    private void Awake()
    {
        // joystick = FindObjectOfType<Joystick>();

        m_rigidBody = GetComponent<Rigidbody2D>();

        m_playerTransform = GetComponent<Transform>();

        m_playerAnimation = GetComponent<PlayerAnimation>();

        m_boxCollider2D = GetComponent<BoxCollider2D>();

        m_enemy = GetComponent<Enemy>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_currentHealth = health;
        m_originalGravity = m_rigidBody.gravityScale;
    }

    private void Update()
    {
        if (!isDead)
        {
            if (!isAttacking && !isMovingAttacking && !m_canKnockBack)
            {
                MoveCharacter();
            }

            HandleAttack();
            IsGrounded();
            GetPlayerDirection();
        }
    }

    private void FixedUpdate()
    {
        m_rigidBody.velocity = new Vector2(m_horizontalMove, m_rigidBody.velocity.y);

        HandleDash();
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            if (m_direction == 1)
            {
                m_rigidBody.AddForce(new Vector2(-dashLength, 0) * dashSpeed, ForceMode2D.Impulse);
            }
            else if (m_direction == 2)
            {
                m_rigidBody.AddForce(new Vector2(dashLength, 0) * dashSpeed, ForceMode2D.Impulse);
            }
        }
    }

    private void GetPlayerDirection()
    {
        if (transform.rotation == Quaternion.Euler(0, 180, 0))
        {
            m_direction = 1;
        }
        else if (transform.rotation == Quaternion.Euler(0, 0, 0))
        {
            m_direction = 2;
        }
    }

    // handles all moving based on platform and input device
    private void MoveCharacter()
    {
        // if (inputDetection.instance.isGamepadEnabled)
        // {
        //     HandleGamepadMovement();
        // }
        // else if (inputDetection.instance.isMobile)
        // {
        //     HandleMobileMovement();
        // }

        if (Input.GetKeyDown(KeyCode.W))
        {
            JumpButton();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            StartCoroutine(DashRoutine());
        }

        m_moveInput.x = Input.GetAxisRaw("Horizontal");
        m_moveInput.Normalize();

        m_horizontalMove = moveSpeed * m_moveInput.x;
        if (m_horizontalMove > 0 || m_horizontalMove < 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        m_playerAnimation.Move(Mathf.Abs(m_moveInput.x));

        HandleDirection();

    }

    private IEnumerator DashRoutine()
    {
        canDash = false;

        isDashing = true;

        m_rigidBody.gravityScale = 0;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        m_rigidBody.gravityScale = m_originalGravity;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void HandleDirection()
    {
        if (m_horizontalMove < 0)
        {
            m_playerTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (m_horizontalMove > 0)
        {
            m_playerTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void HandleGamepadMovement()
    {
        // if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        // {
        //     JumpButton();
        // }

        //instead of seperate buttons i used getaxisraw for the movement.
        m_moveInput.x = Input.GetAxisRaw("Horizontal");
        m_moveInput.Normalize();

        m_horizontalMove = moveSpeed * m_moveInput.x;
    }

    private void HandleMobileMovement()
    {
        if (m_moveLeft)
        {
            m_horizontalMove = -moveSpeed;
        }
        else if (m_moveRight)
        {
            m_horizontalMove = moveSpeed;
        }
        else
        {
            m_horizontalMove = 0;
        }

        // if (inputDetection.instance.isJoystickControlsForMobileEnabled)
        // {
        //     m_horizontalMove = joystick.Horizontal * moveSpeed;
        // }
    }

    private void IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_boxCollider2D.bounds.center, m_boxCollider2D.bounds.size, 0f, Vector2.down, .3f, m_groundedLayer.value);
        // Debug.DrawRay(m_boxCollider2D.bounds.center, Vector2.down * .5f, Color.green);


        if (hitInfo.collider != null)
        {
            m_grounded = true;
            isJumping = false;
            isFalling = false;
            m_buttonUsed = 0;
        }
        else
        {
            m_grounded = false;
            isJumping = true;
        }

        if (m_rigidBody.velocity.y < 0)
        {
            isFalling = true;
        }
    }

    // Handles jump button on click
    public void JumpButton()
    {
        m_buttonUsed++;
        if (m_grounded && m_buttonUsed == 1)
        {
            m_rigidBody.velocity = Vector2.up * jumpSpeed;
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackRoutine());
        }
        // moving attack
        if (isMoving && Input.GetKeyDown(KeyCode.Space))
        {
            if (m_direction == 1)
            {
                m_horizontalMove = -speedAfterMovingAttack;
            }
            else if (m_direction == 2)
            {
                m_horizontalMove = speedAfterMovingAttack;
            }

            isMovingAttacking = true;

            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {

        if (isMovingAttacking)
        {
            m_playerAnimation.MovingAttackAnim();
            yield return new WaitForSeconds(.5f);

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

        if (hitEnemies != null && m_candamage && isAttacking || isMovingAttacking)
        {
            foreach (var enemy in hitEnemies)
            {
                if (isMovingAttacking)
                {
                    enemy.gameObject.GetComponent<Enemy>().Damage(movingAttackDamage);
                }
                else
                {
                    enemy.gameObject.GetComponent<Enemy>().Damage(damage);
                }
            }
            // waits for second spesific animation frame and damages twice
            if (!isMovingAttacking)
            {
                m_candamage = false;

                while (m_candamage == false)
                {
                    yield return null;
                }

                hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

                if (m_candamage && hitEnemies != null)
                {
                    foreach (var enemy in hitEnemies)
                    {
                        enemy.gameObject.GetComponent<Enemy>().Damage(damage);
                    }
                }
            }
        }
        m_candamage = false;
    }

    //animation event for attack
    public void CanDamage()
    {
        m_candamage = true;
    }

    //animation event for movingAttack
    public void StopPlayer()
    {
        m_horizontalMove = 0;
    }

    public void Damage(float damageTaken)
    {
        if (canTakeDamage)
        {
            m_currentHealth -= damageTaken;
            StartCoroutine(Invincible(invincibleDuration));

            StartCoroutine(PlayBloodAnim());

            m_playerAnimation.TakeHit();

            if (m_currentHealth <= 0)
            {
                Killed();
            }
        }
    }

    private IEnumerator Invincible(float duration)
    {
        Physics2D.IgnoreLayerCollision(11, 10, true);
        invincible = true;
        canTakeDamage = false;
        

        yield return new WaitForSeconds(duration);

        canTakeDamage = true;
        invincible = false;
        capsuleCollider.enabled = true;
        Physics2D.IgnoreLayerCollision(11, 10, false);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!invincible)
        {
            if (other.gameObject.CompareTag("Enemy") || m_canKnockBack)
            {
                isMoving = false;
                while (triggerIteration == 0)
                {
                    m_canKnockBack = true;

                    Damage(1);

                    if (m_direction == 1)
                    {
                        StartCoroutine(KnockbackRoutine(knocbackPower));
                    }
                    else if (m_direction == 2)
                    {
                        StartCoroutine(KnockbackRoutine(-knocbackPower));
                    }
                    triggerIteration++;
                }

            }
        }
    }

    private IEnumerator KnockbackRoutine(float power)
    {
        m_horizontalMove = power;

        yield return new WaitForSeconds(.1f);
        m_horizontalMove = power / 2;

        yield return new WaitForSeconds(.2f);
        m_horizontalMove = power / 5;

        yield return new WaitForSeconds(.2f);
        m_horizontalMove = 0;

        m_canKnockBack = false;
        triggerIteration = 0;
    }
}
