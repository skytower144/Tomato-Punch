using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour
{
    [SerializeField] private Transform leader;
    [SerializeField] private Animator anim;
    [SerializeField] private int followDelay;
    [Space(10), SerializeField] private List<SceneName> offLimitAreas;
    
    private Queue<Vector2> record = new Queue<Vector2>();
    private Rigidbody2D rb;
    private BoxCollider2D col;

    private float followSpeed = 8.9f;
    private float stopTime;
    private bool playerStopped;
    private bool isFollowing;

    private Vector2 direction;
    private float distance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        if (leader == null)
            leader = GameManager.gm_instance.player_movement.transform;
    }

    void FixedUpdate()
    {
        if (isFollowing) Follow();
    }

    public void EnableFollow()
    {
        isFollowing = true;
        col.enabled = false;
    }

    public void DisableFollow()
    {
        isFollowing = false;
        col.enabled = true;
        ClearRecord();
    }

    public void Teleport(Vector2 teleportPos)
    {
        transform.position = teleportPos;
        ClearRecord();
    }

    private void ClearRecord()
    {
        playerStopped = true;
        Animate(false);
        record = new Queue<Vector2>();
    }

    private void Follow()
    {
        if (PlayerHasStopped()) {
            if (!playerStopped) {
                playerStopped = true;
                stopTime = Time.time;
            }

            if (Time.time - stopTime > 0.1f) {
                Animate(false);
                return;
            }
        }
        else
            playerStopped = false;
        
        if (!record.Contains(leader.position))
            record.Enqueue(leader.position);

        if (record.Count > followDelay) {
            Move(record.Dequeue());
        }
    }

    private void Move(Vector2 movePos, bool isSpriteAnimator = false)
    {
        direction = movePos - rb.position;
        distance = direction.magnitude;

        if (distance < 0.01f) {
            rb.position = movePos;
            return;
        }
        else
        {
            // Calculate the movement speed based on followSpeed and Time.deltaTime
            float movementSpeed = Mathf.Min(followSpeed * Time.deltaTime, distance);

            // Move the NPC towards the target position using the calculated speed
            Vector2 amount = direction.normalized * movementSpeed;
            rb.position += amount;
        }
        if (isSpriteAnimator) Animate(direction);
        else Animate(true, direction);
    }

    private void Animate(bool isAnimating, Vector2 direction = default)
    {
        if (anim.GetBool("isWalking") && !isAnimating)
            FlattenPos();
        
        anim.SetBool("isWalking", isAnimating);

        if (!isAnimating) {
            anim.SetFloat("moveX", 0f);
            anim.SetFloat("moveY", 0f);
            return;
        }
        direction = direction.normalized;
        anim.SetFloat("moveX", Mathf.Round(direction.x));
        anim.SetFloat("moveY", Mathf.Round(direction.y));
    }

    private void Animate(Vector2 direction)
    {

    }

    private void FlattenPos()
    {
        transform.position = new Vector2(transform.position.x, Mathf.Floor(transform.position.y * 100f) / 100f);
    }

    private bool PlayerHasStopped()
    {
        return !GameManager.gm_instance.player_movement.playerAnim.GetBool("isWalking");
    }
}