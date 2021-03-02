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

    public void Move(float move)
    {
        m_movementAnimator.SetFloat("Move", Mathf.Abs(move));
    }

    public void Jump(bool state)
    {
        m_movementAnimator.SetBool("Jump", state);
    }
    public void Attack()
    {
        if (m_movementAnimator.GetBool("Attacking") || m_movementAnimator.GetFloat("Move") != 0)
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
        m_playerController.canDamage = false;
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
