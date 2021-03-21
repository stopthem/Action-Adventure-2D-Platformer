using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Enemy m_enemy;
    private EnemyMovement m_enemyMovement;
    private Animator m_enemyAnimator;

    [Header("Animations")]
    public AnimationClip attackAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip takeHitAnimation;

    private void Awake()
    {
        m_enemy = GetComponent<Enemy>();
        m_enemyMovement = GetComponent<EnemyMovement>();
        m_enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (HasParameter("TakeHit", m_enemyAnimator))
        {
            TakeHit(m_enemy.isTakingHit);
        }
        if (HasParameter("Walking", m_enemyAnimator))
        {
            Walking(m_enemyMovement.isWalking);
        }
        if (HasParameter("Attacking", m_enemyAnimator))
        {
            Attacking(m_enemy.isAttacking);
        }
        if (HasParameter("Idle", m_enemyAnimator))
        {
            Idle(m_enemyMovement.isIdle);
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

    private void Idle(bool status)
    {
        m_enemyAnimator.SetBool("Idle", status);
    }

    private void Walking(bool status)
    {
        m_enemyAnimator.SetBool("Walking", status);
    }

    private void Attacking(bool status)
    {
        m_enemyAnimator.SetBool("Attacking", status);
    }

    public IEnumerator AttackAnimationRoutine()
    {
        m_enemyMovement.isWalking = false;
        m_enemy.isAttacking = true;

        yield return new WaitForSeconds(attackAnimation.length);

        m_enemy.isAttacking = false;
        m_enemyMovement.isWalking = true;

    }

    private void TakeHit(bool status)
    {
        m_enemyAnimator.SetBool("TakeHit", status);
    }

    public void Reborn(bool status)
    {
        m_enemyAnimator.SetBool("Reborn", status);
    }

    public IEnumerator TakeHitAnimRoutine()
    {
        m_enemy.isTakingHit = true;
        m_enemyMovement.isWalking = false;

        yield return new WaitForSeconds(takeHitAnimation.length);

        m_enemy.isTakingHit = false;
        m_enemyMovement.isWalking = true;
    }

    public void DeathAnim()
    {
        if (m_enemy.isDead)
        {
            return;
        }
        else
        {
            m_enemyAnimator.SetTrigger("Death");
        }
    }

    public IEnumerator WaypointAnimRoutine()
    {
        m_enemyMovement.isIdle = true;
        m_enemyMovement.isWalking = false;

        yield return new WaitForSeconds(idleAnimation.length);

        m_enemyMovement.isIdle = false;
        m_enemyMovement.isWalking = true;

    }
}
