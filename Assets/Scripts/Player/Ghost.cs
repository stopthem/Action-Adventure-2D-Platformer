using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private float ghostDelay;
    public float ghostDelayMax;
    public float whenToDestroy;

    public GameObject ghost;

    [HideInInspector] public bool canCreate;

    private void Start()
    {
        ghostDelay = ghostDelayMax;
    }

    private void Update()
    {
        if (canCreate)
        {
            ghostDelay -= Time.deltaTime;

            if (ghostDelay <= 0)
            {
                SpriteRenderer playerSprite = PlayerController.Instance.spriteRenderer;
                GameObject ghostInstance = Instantiate(ghost, transform.position, transform.rotation);
                ghostInstance.GetComponent<SpriteRenderer>().sprite = playerSprite.sprite;
                ghostInstance.transform.rotation = playerSprite.transform.rotation;
                ghostDelay = ghostDelayMax;
                Destroy(ghostInstance, whenToDestroy);
            }
        }
    }

}
