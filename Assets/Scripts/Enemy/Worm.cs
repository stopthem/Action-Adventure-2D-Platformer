using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    [SerializeField] private GameObject fireBall;

    [SerializeField] private Transform shootPoint;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }


    private void Update()
    {
        // if (!enemy.isDead)
        // {
        //     if (PlayerController.Instance.transform.position.x < transform.position.x)
        //     {
        //         transform.rotation = Quaternion.Euler(0, 180, 0);
        //     }
        //     else
        //     {
        //         transform.rotation = Quaternion.Euler(0, 0, 0);
        //     }
        // }
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(fireBall, shootPoint.position, transform.rotation, transform);
    }
}
