using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private bool enableMove;
    [SerializeField] private int followDelay;
    private Transform leader;
    private Queue<Vector2> record = new Queue<Vector2>();
    private Rigidbody2D rb;

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
        record.Enqueue(leader.position);

        if (record.Count > followDelay)
            Follow();
    }

    private void Follow()
    {
        //rb.position = followPos;
        rb.position = Vector2.MoveTowards(rb.position, record.Dequeue(), speed * Time.deltaTime);
    }

    private bool PlayerHasStopped()
    {
        return !GameManager.gm_instance.player_movement.playerAnim.GetBool("isWalking");
    }

}
