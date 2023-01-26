using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBehaviour : MonoBehaviour
{
    [SerializeField] private WanderBehaviour wanderBehaviour;
    [SerializeField] private GameObject fleePoint;
    [SerializeField] private float speed;
    private bool isFlee = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.x < fleePoint.transform.position.x)
            transform.localScale = new Vector2(-1, transform.localScale.y);
        else
            transform.localScale = new Vector2(1, transform.localScale.y);
        
        wanderBehaviour.ExitDefaultBehaviour();
        wanderBehaviour.PlayAnimation("birds_flee");
        spriteRenderer.sortingOrder = 2;
        
        isFlee = true;
    }

    void Update()
    {
        if (isFlee)
        {
            transform.position = Vector2.MoveTowards(transform.position, fleePoint.transform.position, speed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, fleePoint.transform.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
