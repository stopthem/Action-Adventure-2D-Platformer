using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [Header("Spider Spesific")]
    public float poisonDuration;
    public float poisonDurationPerTick;

    protected override void Attack()
    {
        StartCoroutine(PoisonRoutine());
    }

    private IEnumerator PoisonRoutine()
    {
        StartCoroutine(m_enemyAnimation.AttackAnimationRoutine());

        // waiting untill a spesific animation frame
        while (canDamage == false)
        {
            yield return null;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, enemyRange, playerLayer);

        if (hitEnemies != null && canDamage)
        {
            foreach (var player in hitEnemies)
            {
                if (!m_playerController.isPoisoned)
                {
                    m_playerController.isPoisoned = true;
                    yield return StartCoroutine(m_playerController.IsPoisonedRoutine(poisonDurationPerTick, poisonDuration));
                }
            }
        }

        canDamage = false;


    }

    private void Poison()
    {

    }
}
