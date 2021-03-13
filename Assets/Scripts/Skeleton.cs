using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    [Header("Skeleton Spesific")]
    public float rebornTime;

    public override void Killed()
    {
        m_enemyAnimation.DeathAnim();

        isDead = true;

        theRB2D.velocity = Vector2.zero;

        if (currentHealth < -1)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(RebornRoutine());
        }

    }

    private IEnumerator RebornRoutine()
    {
        yield return new WaitForSeconds(rebornTime);
        m_enemyAnimation.Reborn(true);
        isDead = false;
        currentHealth = health;
        yield return new WaitForSeconds(.5f);
        m_enemyAnimation.Reborn(false);
    }
}
