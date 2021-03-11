using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_playerAnimator;
    private PlayerController m_playerController;

    public AnimationClip attackAnimation;
    public AnimationClip dashAttackAnimation;

    private void Awake()
    {
        m_playerAnimator = GetComponent<Animator>();
        m_playerController = GetComponent<PlayerController>();
    }

    public bool GetBool(string s)
    {
        return m_playerAnimator.GetBool(s);
    }

    public float GetFloat(string f)
    {
        return m_playerAnimator.GetFloat(f);
    }

    public void Move(float move)
    {
        if (move > 0)
        {
            m_playerAnimator.SetBool("Moving", true);
        }
        else
        {
            m_playerAnimator.SetBool("Moving", false);
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

    public void TakeHit()
    {
        if (!m_playerController.isDead)
        {
            m_playerAnimator.SetTrigger("TakeHit");
        }
    }

    public void MovingAttack(bool state)
    {
        if (!m_playerController.isDead)
        {
            m_playerAnimator.SetBool("MovingAttack", true);
        }
    }

    public void MovingAttackAnim()
    {
        if (!m_playerController.isDead)
        {
            StartCoroutine(MovingAttackRoutine());
        }
    }

    private IEnumerator MovingAttackRoutine()
    {
        yield return new WaitForSeconds(dashAttackAnimation.length);
        m_playerAnimator.SetBool("MovingAttack", false);
    }

    public void DeathAnimation()
    {
        if (!m_playerController.isDead)
        {
            m_playerAnimator.SetTrigger("Death");
        }
    }

    public void Attack()
    {
        if (m_playerAnimator.GetBool("Attacking") || m_playerAnimator.GetBool("Moving"))
        {
            return;
        }
        else
        {
            StartCoroutine(AttackAnimRoutine());
        }
    }

    private IEnumerator AttackAnimRoutine()
    {
        m_playerAnimator.SetBool("Attacking", true);

        yield return new WaitForSeconds(attackAnimation.length);

        m_playerAnimator.SetBool("Attacking", false);

    }

    public void Falling(bool status)
    {
        if (m_playerAnimator.GetBool("Jump"))
        {
            m_playerAnimator.SetBool("IsFalling", status);
        }
        else
        {
            m_playerAnimator.SetBool("IsFalling", false);
        }
    }
}
