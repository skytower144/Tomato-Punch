using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D myRb;
    private Animator myAnim;
    [SerializeField] private GameObject inventory;
    [SerializeField] private float speed;
    private Vector2 movement;
    public LayerMask interactableLayer;
    public static bool isBattle = false;
    private bool isInteracting = false;
    public event Action BeginBattle;

    // Start is called before the first frame update
    void Start()
    {
       myRb = GetComponent<Rigidbody2D>();
       myAnim = GetComponent<Animator>();
       //Cursor.visible = false;
    }

    public void HandleUpdate()
    {
        if(!isBattle)
        {
            if(Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown("joystick button 0")))
            {
                PlayerInteract();
            }
            if(Input.GetKeyDown(KeyCode.Return))
            {
                inventory.SetActive(!inventory.activeSelf);
                IsInteracting();
                //Cursor.visible = !Cursor.visible;
            }
        }
        else if(isBattle)
        {
            BeginBattle();
        }
    }
    // Update is called once per frame
    public void FixedUpdate() //For Executing Physics
    {
       if(!isBattle && !isInteracting)
       {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement = movement.normalized;

            if(movement != Vector2.zero)
            {
                myAnim.SetBool("isWalking", true);
                myAnim.SetFloat("moveX", movement.x);
                myAnim.SetFloat("moveY", movement.y);

                myRb.MovePosition(myRb.position + movement * speed * Time.fixedDeltaTime);

            }
            else if(movement == Vector2.zero)
            {
            myAnim.SetBool("isWalking", false);
            }
       }
    }
    void PlayerInteract()
    {
        var facingDir = new Vector3(myAnim.GetFloat("moveX"), myAnim.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapPoint(interactPos, interactableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    void IsInteracting()
    {
        isInteracting = !isInteracting;
        myAnim.SetBool("isWalking",false);
    }
}
