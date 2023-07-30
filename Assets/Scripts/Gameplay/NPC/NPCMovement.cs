using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private bool enableMove;
    [SerializeField] private int followDelay;
    private float followSpeed = 8.9f;
    private Transform leader;
    private Queue<Vector2> record = new Queue<Vector2>();
    private Rigidbody2D rb;

    Vector2 direction;
    float distance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leader = GameManager.gm_instance.player_movement.transform;
    }

    void FixedUpdate()
    {
        if (!enableMove) return;
        Watch();
    }

    private void Watch()
    {
        if (PlayerHasStopped()) return;
        record.Enqueue(leader.position);

        if (record.Count > followDelay) {
            Follow(record.Dequeue());
        }
    }

    private void Follow(Vector2 movePos)
    {
        direction = movePos - rb.position;
        distance = direction.magnitude;

        if (distance < 0.01f)
            rb.position = movePos;
        else
        {
            // Calculate the movement speed based on followSpeed and Time.deltaTime
            float movementSpeed = Mathf.Min(followSpeed * Time.deltaTime, distance);

            // Move the cat towards the target position using the calculated speed
            rb.position += direction.normalized * movementSpeed;
        }       
    }

    private bool PlayerHasStopped()
    {
        return !GameManager.gm_instance.player_movement.playerAnim.GetBool("isWalking");
    }
}