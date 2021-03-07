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

    private int m_buttonUsed = 0;

    private float m_horizontalMove;
    public float moveSpeed;
    public float jumpSpeed;
    public float attackRange = .5f;
    public float damage;
    public float health;
    private float currentHealth;

    private bool m_grounded;
    public bool canDamage;
    [SerializeField] public bool isDead;

    public Transform attackPoint;
    public LayerMask enemyLayer;

    [SerializeField] private LayerMask m_groundedLayer;

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
    }

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        if (!isDead)
        {
            MoveCharacter();
            HandleAttack();
            IsGrounded();
        }

        // GetPlayerDirection();
    }

    // handles physics
    private void FixedUpdate()
    {
        m_rigidBody.velocity = new Vector2(m_horizontalMove, m_rigidBody.velocity.y);
    }

    // private void GetPlayerDirection()
    // {
    //     if (transform.rotation == Quaternion.Euler(0, 180, 0))
    //     {
    //         m_direction = 1;
    //     }
    //     else if (transform.rotation == Quaternion.Euler(0, 0, 0))
    //     {
    //         m_direction = 2;
    //     }
    // }

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
        m_moveInput.x = Input.GetAxisRaw("Horizontal");
        m_moveInput.Normalize();

        m_horizontalMove = moveSpeed * m_moveInput.x;

        m_playerAnimation.Move(m_moveInput.x);

        HandleDirection();

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
            m_playerAnimation.Jump(false);
            m_playerAnimation.Falling(false);
            m_buttonUsed = 0;
        }
        else
        {
            m_grounded = false;
            m_playerAnimation.Jump(true);
        }

        if (m_rigidBody.velocity.y < 0)
        {
            m_playerAnimation.Falling(true);
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
    }

    private IEnumerator AttackRoutine()
    {
        m_playerAnimation.Attack();
        // waits untill specific attack animation frame
        while (canDamage == false)
        {
            yield return null;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies != null && canDamage)
        {
            foreach (var enemy in hitEnemies)
            {
                enemy.gameObject.GetComponent<Enemy>().Damage(damage);
            }
        }

        canDamage = false;
    }

    public void CanDamage()
    {
        canDamage = true;
    }

    public void Damage(float damageTaken)
    {
        currentHealth -= damageTaken;
        m_playerAnimation.TakeHit();

        if (currentHealth <= 0)
        {
            Killed();
        }
    }

    public void Killed()
    {
        m_playerAnimation.DeathAnimation();
        isDead = true;
    }
}
