using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public static PlayerAnimation Instance { get; private set; }

    private Animator m_playerAnimator;

    [Header("Animations")]
    public AnimationClip attackAnimation;
    public AnimationClip dashAttackAnimation;
    public AnimationClip takehitAnimation;

    private void Awake()
    {
        Instance = this;

        m_playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (HasParameter("Moving", m_playerAnimator))
        {
            Moving(PlayerMovement.Instance.isMoving);
        }
        if (HasParameter("Attacking", m_playerAnimator))
        {
            Attacking(PlayerController.Instance.isAttacking);
        }
        if (HasParameter("IsDashAttacking", m_playerAnimator))
        {
            DashAttack(PlayerController.Instance.isDashAttacking);
        }
        if (HasParameter("IsDashing", m_playerAnimator))
        {
            Dash(PlayerMovement.Instance.isDashing);
        }
        if (HasParameter("IsFalling", m_playerAnimator))
        {
            Falling(PlayerMovement.Instance.isFalling);
        }
        if (HasParameter("Jump", m_playerAnimator))
        {
            Jump(PlayerMovement.Instance.isJumping);
        }
        if (HasParameter("IsInvincible", m_playerAnimator))
        {
            IsInvincible(PlayerController.Instance.invincible);
        }
        if (HasParameter("IsPoisoned", m_playerAnimator))
        {
            isPoisoned(PlayerController.Instance.isPoisoned);
        }
        if (HasParameter("CriticalHealth", m_playerAnimator))
        {
            CriticalHealth(PlayerController.Instance.isCriticalHealth);
        }
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    public void Move(float move)
    {
        if (move > 0)
        {
            PlayerMovement.Instance.isMoving = true;
        }
        else
        {
            PlayerMovement.Instance.isMoving = false;
        }
    }

    public void Moving(bool move)
    {
        if (!PlayerController.Instance.isDead)
        {
            m_playerAnimator.SetBool("Moving", move);
        }
    }

    public void Jump(bool state)
    {
        m_playerAnimator.SetBool("Jump", state);
    }

    public void Dash(bool state)
    {
        m_playerAnimator.SetBool("IsDashing", state);
    }
    private void IsInvincible(bool state)
    {
        if (!PlayerController.Instance.isDead)
        {
            m_playerAnimator.SetBool("IsInvincible", state);
        }
    }
    private void isPoisoned(bool state)
    {
        m_playerAnimator.SetBool("IsPoisoned", state);
    }

    public void TakeHit()
    {
        if (!PlayerController.Instance.isDead)
        {
            StartCoroutine(TakeHitAnimRoutine());
        }
    }

    private IEnumerator TakeHitAnimRoutine()
    {
        m_playerAnimator.SetTrigger("TakeHit");
        yield return new WaitForSeconds(takehitAnimation.length);
    }

    private void Attacking(bool state)
    {
        if (!PlayerController.Instance.isDead)
        {
            m_playerAnimator.SetBool("Attacking", state);
        }
    }

    private IEnumerator AttackAnimRoutine()
    {
        PlayerController.Instance.isAttacking = true;

        yield return new WaitForSeconds(attackAnimation.length);

        PlayerController.Instance.isAttacking = false;
    }

    private void DashAttack(bool state)
    {
        if (!PlayerController.Instance.isDead)
        {
            m_playerAnimator.SetBool("IsDashAttacking", state);
        }
    }

    private void CriticalHealth(bool status)
    {
        m_playerAnimator.SetBool("CriticalHealth", status);
    }

    public void MovingAttackAnim()
    {
        if (!PlayerController.Instance.isDead)
        {
            StartCoroutine(MovingAttackRoutine());
        }
    }

    public IEnumerator MovingAttackRoutine()
    {
        yield return new WaitForSeconds(dashAttackAnimation.length);
    }

    public void DeathAnimation()
    {
        if (!PlayerController.Instance.isDead)
        {
            m_playerAnimator.SetTrigger("Death");
        }
    }

    public void AttackAnim()
    {
        if (PlayerController.Instance.isAttacking || PlayerMovement.Instance.isMoving)
        {
            return;
        }
        else
        {
            StartCoroutine(AttackAnimRoutine());
        }
    }

    public void Falling(bool status)
    {
        if (PlayerMovement.Instance.isJumping)
        {
            m_playerAnimator.SetBool("IsFalling", status);
        }
        else
        {
            m_playerAnimator.SetBool("IsFalling", false);
        }
    }
}
