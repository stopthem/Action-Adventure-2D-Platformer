using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_movementAnimator;
    private Animator m_swordAnimator;

    private void Awake()
    {
        m_movementAnimator = GetComponentInChildren<Animator>();
        m_swordAnimator = transform.GetChild(1).GetComponent<Animator>();
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
        m_swordAnimator.SetTrigger("SwingEffect");

    }
}
