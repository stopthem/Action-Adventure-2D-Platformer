using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_movementAnimator;
    private PlayerController m_playerController;

    public AnimationClip attackAnimation;

    private void Awake()
    {
        m_movementAnimator = GetComponentInChildren<Animator>();
        m_playerController = GetComponent<PlayerController>();
    }

    public bool GetBool(string s)
    {
        return m_movementAnimator.GetBool(s);
    }

    public float GetFloat(string f)
    {
        return m_movementAnimator.GetFloat(f);
    }

    public void Move(float move)
    {
        if (move > 0)
        {
            m_movementAnimator.SetBool("Moving", true);
        }
        else
        {
            m_movementAnimator.SetBool("Moving", false);
        }
        // m_movementAnimator.SetFloat("Move", Mathf.Abs(move));
    }

    public void Jump(bool state)
    {
        m_movementAnimator.SetBool("Jump", state);
    }

    public void TakeHit()
    {
        if (!m_playerController.isDead)
        {
            m_movementAnimator.SetTrigger("TakeHit");
        }

    }

    public void DeathAnimation()
    {
        if (!m_playerController.isDead)
        {
            m_movementAnimator.SetTrigger("Death");
        }
    }

    public void Attack()
    {
        if (m_movementAnimator.GetBool("Attacking") || m_movementAnimator.GetBool("Moving"))
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
        m_movementAnimator.SetBool("Attacking", true);

        yield return new WaitForSeconds(attackAnimation.length);

        m_movementAnimator.SetBool("Attacking", false);

    }

    public void Falling(bool status)
    {
        if (m_movementAnimator.GetBool("Jump"))
        {
            m_movementAnimator.SetBool("IsFalling", status);
        }
        else
        {
            m_movementAnimator.SetBool("IsFalling", false);
        }
    }
}
