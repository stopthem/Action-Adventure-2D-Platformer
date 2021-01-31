using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // public InputDetection inputDetection;

    private int m_direction;

    private Rigidbody2D m_rigidBody;

    private Transform m_playerTransform;

    private Vector2 m_moveInput;

    private int m_buttonUsed = 0;

    private float m_horizontalMove;
    public float moveSpeed;
    public float jumpSpeed;
    
    private bool m_grounded;

    [SerializeField]private LayerMask m_groundedLayer;

    // private Joystick joystick;

    private bool m_moveLeft = false, m_moveRight = false;
    private void Awake()
    {
        // joystick = FindObjectOfType<Joystick>();

        m_rigidBody = GetComponent<Rigidbody2D>();

        m_playerTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        MoveCharacter();
        GetPlayerDirection();
    }

    // handles physics
    public void FixedUpdate()
    {
        m_rigidBody.velocity = new Vector2(m_horizontalMove, m_rigidBody.velocity.y);
    }

    // gets player direction for Alice's dash and further use.
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

    // Moving buttons hold process
    // public void MoveLeft()
    // {
    //     m_moveLeft = true;
    // }
    // public void StopMoveLeft()
    // {
    //     m_moveLeft = false;
    // }
    // public void MoveRight()
    // {
    //     m_moveRight = true;

    // }
    // public void StopMoveRight()
    // {
    //     m_moveRight = false;
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
        // else //test code for editor
        // {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
        m_moveInput.x = Input.GetAxisRaw("Horizontal");
        m_moveInput.Normalize();
        // }
        m_horizontalMove = moveSpeed * m_moveInput.x;
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

            // m_playerTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (m_moveRight)
        {
            m_horizontalMove = moveSpeed;

            // m_playerTransform.rotation = Quaternion.Euler(0, 0, 0);
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

    // Handles jump button on click
    public void JumpButton()
    {
        m_buttonUsed++;

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1f, m_groundedLayer.value);
        if (hitInfo.collider != null)
        {
            m_grounded = true;
            m_buttonUsed = 0;
        }
        if (m_grounded && m_buttonUsed == 0)
        {
            m_rigidBody.velocity = Vector2.up * jumpSpeed;
            m_grounded = false;
            m_buttonUsed++;
        }
    }
}
