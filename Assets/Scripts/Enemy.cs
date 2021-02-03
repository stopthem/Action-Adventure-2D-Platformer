using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int speed;
    [SerializeField] protected int health;

    protected int gems;

    protected Rigidbody2D theRB2D;
    protected Transform enemyTransform;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        theRB2D = GetComponent<Rigidbody2D>();
        enemyTransform = GetComponent<Transform>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Attack()
    {

    }

    protected virtual void Update()
    {

    }
}
