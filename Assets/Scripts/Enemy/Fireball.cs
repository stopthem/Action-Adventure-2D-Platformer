using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float damage = 1;
    private Vector3 direction;

    void Start()
    {
        direction = PlayerController.Instance.transform.position - transform.position;
        direction.Normalize();
    }

    private void Update()
    {
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        transform.position += direction * bulletSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.Damage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}
