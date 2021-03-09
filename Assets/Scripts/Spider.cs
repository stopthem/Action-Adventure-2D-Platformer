using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (canKnockBack)
        {
            if (playerTransform.position.x < transform.position.x && !isDead)
            {
                theRB2D.velocity = Vector2.right * 25;
                canKnockBack = false;
            }
            if (playerTransform.position.x > transform.position.x && !isDead)
            {
                theRB2D.velocity = Vector2.left * 25;
                canKnockBack = false;
            }
            
        }
    }
}
