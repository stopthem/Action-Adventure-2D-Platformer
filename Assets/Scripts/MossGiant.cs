using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossGiant : Enemy
{
    [SerializeField] private AnimationClip idleAnimation;

    protected override void Update()
    {
        base.Update();
        if (animator.GetBool("Idle") && canMove)
        {
            StartCoroutine(WaypointRoutine());
        }
    }

    private IEnumerator WaypointRoutine()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Walking", false);

        yield return new WaitForSeconds(idleAnimation.length);

        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);

    }

    
}
