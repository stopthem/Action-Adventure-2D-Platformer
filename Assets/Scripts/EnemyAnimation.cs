using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Enemy m_enemy;
    private Animator m_animator;

    [Header("Animations")]
    public AnimationClip attackAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip takeHitAnimation;

    private void Awake()
    {
        m_enemy = GetComponent<Enemy>();
        m_animator = GetComponent<Animator>();
    }

    public bool GetBool(string s)
    {
        return m_animator.GetBool(s);
    }

    public void Idle(bool status)
    {
        m_animator.SetBool("Idle", status);
    }

    public void Walking(bool status)
    {
        m_animator.SetBool("Walking", status);
    }

    public void Attacking(bool status)
    {
        m_animator.SetBool("Attacking", status);
    }

    public void TakeHit(bool status)
    {
        m_animator.SetBool("TakeHit", status);
    }

    public void DeathAnim()
    {
        if (m_enemy.isDead)
        {
            return;
        }
        else
        {
            m_animator.SetTrigger("Death");
        }
    }

    public IEnumerator AttackAnimationRoutine()
    {
        Walking(false);
        Attacking(true);
        
        yield return new WaitForSeconds(attackAnimation.length);
        
        Attacking(false);
        Walking(true);

    }

    public IEnumerator WaypointRoutine()
    {
        Idle(true);
        Walking(false);

        yield return new WaitForSeconds(idleAnimation.length);

        Idle(false);
        Walking(true);

    }

    public IEnumerator TakeHitRoutine()
    {
        TakeHit(true);

        yield return new WaitForSeconds(takeHitAnimation.length);

        TakeHit(false);
    }
}
