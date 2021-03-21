using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [Header("Spider Spesific")]
    public float poisonDuration;
    public float poisonDurationPerTick;

    public override void Attack()
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

            for (int i = 0; i < hitEnemies.Length; i++)
            {
                if (!PlayerController.Instance.isPoisoned)
                {
                    PlayerController.Instance.isPoisoned = true;
                    StartCoroutine(PlayerController.Instance.IsPoisonedRoutine(poisonDurationPerTick, poisonDuration,transform.position, enemyDamage));
                }
                else
                {
                    hitEnemies[i].GetComponent<PlayerController>().Damage(enemyDamage,transform.position);
                }
            }
        }

        canDamage = false;


    }
}
