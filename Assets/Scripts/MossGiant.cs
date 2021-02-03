using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossGiant : Enemy
{
    [SerializeField] protected Transform pointA, pointB;
    private Vector3 m_target;

    private Animator animator;
    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }
    protected override void Update()
    {
        base.Update();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Moss_Giant_Idle"))
        {
            return;
        }

        if (m_target == pointA.position)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (Vector2.Distance(transform.position, pointA.position) < 0.10f)
        {

            m_target = pointB.position;
            animator.SetTrigger("Idle");

        }
        else if (Vector2.Distance(transform.position, pointB.position) < 0.10f)
        {

            m_target = pointA.position;
            animator.SetTrigger("Idle");
        }

        transform.localPosition = Vector2.MoveTowards(transform.position, m_target, speed * Time.deltaTime);

    }

}
