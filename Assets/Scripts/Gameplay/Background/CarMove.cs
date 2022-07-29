using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float speed;
    private Rigidbody2D carRb;
    private Vector3 movementVector = Vector3.zero;
    private bool startingLeft;
    private bool startGoing = false;

    void OnDisable()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        carRb = GetComponent<Rigidbody2D>();
        movementVector = (target.position - transform.position).normalized * speed;

        startingLeft = transform.position.x < target.position.x;
        Invoke("StartGoing", 0.5f);
    }

    void FixedUpdate()
    {
        if (startGoing)
        {
            if ((startingLeft && transform.position.x <= target.position.x) || (!startingLeft && target.position.x <= transform.position.x))
            {
                float nextPosition_x = transform.position.x + movementVector.x * Time.deltaTime;
                transform.position = new Vector3(nextPosition_x, transform.position.y);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void StartGoing()
    {
        startGoing = true;
    }
}
