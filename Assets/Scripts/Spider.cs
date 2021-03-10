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
                theRB2D.AddForce(new Vector2(.8f, 0) * 30, ForceMode2D.Impulse);
                canKnockBack = false;
            }
            if (playerTransform.position.x > transform.position.x && !isDead)
            {
                theRB2D.AddForce(new Vector2(-.8f, 0) * 30, ForceMode2D.Impulse);
                canKnockBack = false;
            }
        }
    }
}
