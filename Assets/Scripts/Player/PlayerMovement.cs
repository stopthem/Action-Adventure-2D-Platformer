using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    private Ghost m_ghost;
    private Joystick joystick;

    private Rigidbody2D m_rigidBody;
    private Transform m_playerTransform;

    private BoxCollider2D m_boxCollider2D;

    private int m_triggerIteration = 0;

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
    [HideInInspector] public float dashButtonUsed;
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
        Instance = this;

        joystick = FindObjectOfType<DynamicJoystick>();

        m_ghost = GetComponent<Ghost>();

        m_rigidBody = GetComponent<Rigidbody2D>();

        m_playerTransform = GetComponent<Transform>();

        m_boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        m_originalGravity = m_rigidBody.gravityScale;
    }

    private void Update()
    {
        if (!PlayerController.Instance.isDead)
        {
            if (!PlayerController.Instance.isAttacking && !PlayerController.Instance.isDashAttacking && !canKnockBack && !isDashing && !GameManager.Instance.isPaused)
            {
                MoveCharacter();
            }

            if (isDashing)
            {
                m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, 0);
            }

            if (m_canDash && InputDetection.instance.isJoystickControlsForMobileEnabled)
            {
                UIHandler.Instance.DashButton(false, true);
            }

            IsGrounded();


            if (!isDashing && InputDetection.instance.isJoystickControlsForMobileEnabled)
            {
                dashButtonUsed = 0;
            }
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

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        m_canDash = false;


        m_ghost.canCreate = true;

        if (!isGrounded)
        {
            m_rigidBody.gravityScale = 0;
        }

        yield return new WaitForSeconds(dashDuration);

        m_ghost.canCreate = false;

        isDashing = false;

        if (InputDetection.instance.isJoystickControlsForMobileEnabled && !PlayerController.Instance.isDashAttacking)
        {
            UIHandler.Instance.DashButton(false, false);
        }

        m_rigidBody.gravityScale = m_originalGravity;

        yield return new WaitForSeconds(dashCooldown);

        m_canDash = true;

        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            UIHandler.Instance.DashButton(false, true);
        }

    }

    private void HandleDirection()
    {
        if (horizontalMove < 0)
        {
            m_playerTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (horizontalMove > 0)
        {
            m_playerTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (m_playerTransform.rotation == Quaternion.Euler(0, 180, 0))
        {
            direction = 1;
        }
        else if (m_playerTransform.rotation == Quaternion.Euler(0, 0, 0))
        {
            direction = 2;
        }
    }

    private void MoveCharacter()
    {
        // if (inputDetection.instance.isGamepadEnabled)
        // {
        //     HandleGamepadMovement();
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

        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            horizontalMove = joystick.Horizontal * moveSpeed;
        }
        else
        {
            horizontalMove = moveSpeed * m_moveInput.x;
        }

        if (horizontalMove > 0 || horizontalMove < 0)
        {
            if (!PlayerController.Instance.isDead)
            {
                isMoving = true;
            }
        }
        else
        {
            isMoving = false;
        }

        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            PlayerAnimation.Instance.Move(Mathf.Abs(joystick.Horizontal));
        }
        else
        {
            PlayerAnimation.Instance.Move(Mathf.Abs(m_moveInput.x));
        }


        HandleDirection();

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


    private void IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(m_boxCollider2D.bounds.center, m_boxCollider2D.bounds.size, 0f, Vector2.down, .1f, groundedLayer.value);
        // Debug.DrawRay(m_boxCollider2D.bounds.center, Vector2.down * .5f, Color.green);


        if (hitInfo.collider != null)
        {
            isGrounded = true;
            isJumping = false;
            isFalling = false;
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
        if (isGrounded)
        {
            m_rigidBody.velocity = Vector2.up * jumpSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!PlayerController.Instance.capsuleCollider.isActiveAndEnabled)
        {
            return;
        }
        if (!PlayerController.Instance.invincible)
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

                    PlayerController.Instance.Damage(1);

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
    private void StopPlayer()
    {
        horizontalMove = 0;
    }
    //ONCLICK
    public void DashButton()
    {
        if (m_canDash == true)
        {
            dashButtonUsed++;
            UIHandler.Instance.DashButton(true, true);
            StartCoroutine(DashRoutine());
        }
    }
}
