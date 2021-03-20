using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController m_playerController;
    private PlayerAnimation m_playerAnimation;

    private Rigidbody2D m_rigidBody;
    private Transform m_playerTransform;

    private BoxCollider2D m_boxCollider2D;

    private int m_triggerIteration = 0;
    private int m_buttonUsed = 0;

    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isFalling;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool canKnockBack;
    private bool m_canDash = true;

    [Header("General")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [HideInInspector] public float horizontalMove;
    [HideInInspector] public float direction;
    private float m_originalGravity;
    [Header("Dash")]
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashLength;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;

    [SerializeField] private float knockBackPowerCollide;

    private Vector2 m_moveInput;

    [SerializeField] private LayerMask groundedLayer;

    private void Awake()
    {
        // joystick = FindObjectOfType<Joystick>();

        m_rigidBody = GetComponent<Rigidbody2D>();

        m_playerTransform = GetComponent<Transform>();

        m_playerAnimation = GetComponent<PlayerAnimation>();
        m_playerController = GetComponent<PlayerController>();

        m_boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        m_originalGravity = m_rigidBody.gravityScale;
    }

    private void Update()
    {
        if (!m_playerController.isDead)
        {
            if (!m_playerController.isAttacking && !m_playerController.isDashAttacking && !canKnockBack)
            {
                MoveCharacter();
            }

            IsGrounded();
        }
        else
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        m_boxCollider2D.enabled = false;

        m_rigidBody.isKinematic = true;

        isMoving = false;

        horizontalMove = 0;

        m_rigidBody.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        m_rigidBody.velocity = new Vector2(horizontalMove, m_rigidBody.velocity.y);

        HandleDash();
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            if (direction == 1)
            {
                m_rigidBody.AddForce(new Vector2(-dashLength, 0) * dashSpeed, ForceMode2D.Impulse);
            }
            else if (direction == 2)
            {
                m_rigidBody.AddForce(new Vector2(dashLength, 0) * dashSpeed, ForceMode2D.Impulse);
            }
        }
    }

    private void HandleDirection()
    {
        if (horizontalMove < 0)
        {
            m_playerTransform.rotation = Quaternion.Euler(0, 180, 0);

            direction = 1;
        }
        if (horizontalMove > 0)
        {
            m_playerTransform.rotation = Quaternion.Euler(0, 0, 0);

            direction = 2;
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

        if (Input.GetKeyDown(KeyCode.LeftControl) && m_canDash)
        {
            StartCoroutine(DashRoutine());
        }

        m_moveInput.x = Input.GetAxisRaw("Horizontal");
        m_moveInput.Normalize();

        horizontalMove = moveSpeed * m_moveInput.x;
        if (horizontalMove > 0 || horizontalMove < 0)
        {
            if (!m_playerController.isDead)
            {
                isMoving = true;
            }
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
        m_canDash = false;

        isDashing = true;

        if (!isGrounded)
        {
            m_rigidBody.gravityScale = 0;
        }

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        m_rigidBody.gravityScale = m_originalGravity;

        yield return new WaitForSeconds(dashCooldown);

        m_canDash = true;
    }

    private void HandleGamepadMovement()
    {
        // if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        // {
        //     JumpButton();
        // }

        //instead of seperate buttons i used getaxisraw for the movement.
        // m_moveInput.x = Input.GetAxisRaw("Horizontal");
        // m_moveInput.Normalize();

        // m_horizontalMove = moveSpeed * m_moveInput.x;
    }

    private void HandleMobileMovement()
    {
        // if (m_moveLeft)
        // {
        //     m_horizontalMove = -moveSpeed;
        // }
        // else if (m_moveRight)
        // {
        //     m_horizontalMove = moveSpeed;
        // }
        // else
        // {
        //     m_horizontalMove = 0;
        // }

        // if (inputDetection.instance.isJoystickControlsForMobileEnabled)
        // {
        //     m_horizontalMove = joystick.Horizontal * moveSpeed;
        // }
    }

    private void IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_boxCollider2D.bounds.center, m_boxCollider2D.bounds.size, 0f, Vector2.down, .1f, groundedLayer.value);
        // Debug.DrawRay(m_boxCollider2D.bounds.center, Vector2.down * .5f, Color.green);


        if (hitInfo.collider != null)
        {
            isGrounded = true;
            isJumping = false;
            isFalling = false;
            m_buttonUsed = 0;
        }
        else
        {
            isGrounded = false;
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
        if (isGrounded && m_buttonUsed == 1)
        {
            m_rigidBody.velocity = Vector2.up * jumpSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_playerController.capsuleCollider.isActiveAndEnabled)
        {
            return;
        }
        if (!m_playerController.invincible)
        {
            if (other.gameObject.CompareTag("Enemy") || canKnockBack)
            {
                isMoving = false;
                while (m_triggerIteration == 0)
                {
                    if (canKnockBack == false)
                    {
                        canKnockBack = true;
                    }

                    m_playerController.Damage(1);

                    if (direction == 1)
                    {
                        StartCoroutine(KnockbackRoutine(knockBackPowerCollide));
                    }
                    else if (direction == 2)
                    {
                        StartCoroutine(KnockbackRoutine(-knockBackPowerCollide));
                    }
                    m_triggerIteration++;
                }

            }
        }
    }

    public IEnumerator KnockbackRoutine(float power)
    {
        horizontalMove = power;

        yield return new WaitForSeconds(.1f);
        horizontalMove = power / 2;

        yield return new WaitForSeconds(.2f);
        horizontalMove = power / 5;

        yield return new WaitForSeconds(.2f);

        canKnockBack = false;
        m_triggerIteration = 0;
    }

    public void HandleHitKnockback(float direction, bool isPoisonedDamage, float power)
    {
        if (canKnockBack == false && !isPoisonedDamage)
        {
            canKnockBack = true;
        }

        if (direction == 2 && !isPoisonedDamage && canKnockBack)
        {
            StartCoroutine(KnockbackRoutine(-power));
        }
        else if (direction == 1 && !isPoisonedDamage && canKnockBack)
        {
            StartCoroutine(KnockbackRoutine(power));
        }
    }

    //animation event for movingAttack
    public void StopPlayer()
    {
        horizontalMove = 0;
    }
}
