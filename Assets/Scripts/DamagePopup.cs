using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    [SerializeField] private float moveY;
    [SerializeField] private float scaleDissapear;
    [SerializeField] private float timeToDissapear;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        StartCoroutine(MovingRoutine());

        if (transform.localScale == new Vector3(0, 0, 0))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator MovingRoutine()
    {
        transform.position += new Vector3(0, moveY) * Time.deltaTime;
        transform.localScale -= new Vector3(scaleDissapear, scaleDissapear) * Time.deltaTime;

        yield return new WaitForSeconds(timeToDissapear);

        Destroy(gameObject);
    }
}
