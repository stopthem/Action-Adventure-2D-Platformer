using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_movementAnimator;

    private void Awake()
    {
        m_movementAnimator = GetComponentInChildren<Animator>();
    }
    public void Move(float move)
    {
        m_movementAnimator.SetFloat("Move",Mathf.Abs(move));
    }

    public void Jump(bool state)
    {
        m_movementAnimator.SetBool("Jump",state);
    }
    public void Attack()
    {
        m_movementAnimator.SetTrigger("Attack");
    }

    public void Falling(bool status)
    {
        m_movementAnimator.SetBool("IsFalling",status);
    }
}
