using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_playerAnimator;
    private PlayerController m_playerController;
    private PlayerMovement m_playerMovement;

    [Header("Animations")]
    public AnimationClip attackAnimation;
    public AnimationClip dashAttackAnimation;
    public AnimationClip takehitAnimation;

    private void Awake()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerAnimator = GetComponent<Animator>();
        m_playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (HasParameter("Moving", m_playerAnimator))
        {
            Moving(m_playerMovement.isMoving);
        }
        if (HasParameter("Attacking", m_playerAnimator))
        {
            Attacking(m_playerController.isAttacking);
        }
        if (HasParameter("MovingAttack", m_playerAnimator))
        {
            DashAttack(m_playerController.isDashAttacking);
        }
        if (HasParameter("IsDashing", m_playerAnimator))
        {
            Dash(m_playerMovement.isDashing);
        }
        if (HasParameter("IsFalling", m_playerAnimator))
        {
            Falling(m_playerMovement.isFalling);
        }
        if (HasParameter("Jump", m_playerAnimator))
        {
            Jump(m_playerMovement.isJumping);
        }
        if (HasParameter("IsInvincible", m_playerAnimator))
        {
            IsInvincible(m_playerController.invincible);
        }
        if (HasParameter("IsPoisoned", m_playerAnimator))
        {
            isPoisoned(m_playerController.isPoisoned);
        }
        if (HasParameter("CriticalHealth", m_playerAnimator))
        {
            CriticalHealth(m_playerController.isCriticalHealth);
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
            m_playerMovement.isMoving = true;
        }
        else
        {
            m_playerMovement.isMoving = false;
        }
    }

    public void Moving(bool move)
    {
        if (!m_playerController.isDead)
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
        if (!m_playerController.isDead)
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
        if (!m_playerController.isDead)
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
        if (!m_playerController.isDead)
        {
            m_playerAnimator.SetBool("Attacking", state);
        }
    }

    private IEnumerator AttackAnimRoutine()
    {
        m_playerController.isAttacking = true;

        yield return new WaitForSeconds(attackAnimation.length);

        m_playerController.isAttacking = false;
    }

    private void DashAttack(bool state)
    {
        if (!m_playerController.isDead)
        {
            m_playerAnimator.SetBool("MovingAttack", state);
        }
    }

    private void CriticalHealth(bool status)
    {
        m_playerAnimator.SetBool("CriticalHealth", status);
    }

    public void MovingAttackAnim()
    {
        if (!m_playerController.isDead)
        {
            StartCoroutine(MovingAttackRoutine());
        }
    }

    public IEnumerator MovingAttackRoutine()
    {
        yield return new WaitForSeconds(dashAttackAnimation.length);
        m_playerController.isDashAttacking = false;
    }

    public void DeathAnimation()
    {
        if (!m_playerController.isDead)
        {
            m_playerAnimator.SetTrigger("Death");
        }
    }

    public void AttackAnim()
    {
        if (m_playerController.isAttacking || m_playerMovement.isMoving)
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
        if (m_playerMovement.isJumping)
        {
            m_playerAnimator.SetBool("IsFalling", status);
        }
        else
        {
            m_playerAnimator.SetBool("IsFalling", false);
        }
    }
}
