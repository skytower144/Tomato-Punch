using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator myAnim;
    private Rigidbody2D myRb;
    private PlayerInput playerInput;
    public PlayerInput PlayerInput => playerInput;
    
    [SerializeField] GameManager gameManager;
    [SerializeField] iconNavigation iconnavigation;
    [SerializeField] StatusNavigation statusNavigation;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] private GameObject playerUI, canvas;
    [SerializeField] private List <GameObject> playerUIList;

    [SerializeField] private float speed;
    private Vector2 movement;

    public LayerMask interactableLayer;

    public static bool isBattle = false;
    private bool isInteracting = false;

    // public event Action BeginBattle;

    void Start()
    {
       myRb = GetComponent<Rigidbody2D>();
       playerInput = GetComponent<PlayerInput>();
    }
    public void HandleUpdate()
    {
        if(!isBattle)
        {
            if(!isInteracting && Press_Key("Interact"))
            {
                PlayerInteract();
            }
            else if(!isInteracting && Press_Key("Status"))
            {
                HitStatus();
            }
            else if(!statusNavigation.navigating_status && !SlotNavigation.isBusy && playerUI.activeSelf && Press_Key("Cancel"))
            {
                HitStatus(); // BREAK OUT OF STATUS
            }
            else if(!isInteracting && Press_Key("Pause"))
            {
                HitMenu();
            }
        }
    }
    // Update is called once per frame
    public void FixedUpdate() //For Executing Physics
    {
       if(!isBattle && !isInteracting)
       {
            // movement.x = Input.GetAxisRaw("Horizontal");
            // movement.y = Input.GetAxisRaw("Vertical");
            //movement = movement.normalized;

            movement = playerInput.actions["Move"].ReadValue<Vector2>();

            //if(movement != Vector2.zero)
            if(InputDetection(movement))
            {
                myAnim.SetBool("isWalking", true);
                myAnim.SetFloat("moveX", movement.x);
                myAnim.SetFloat("moveY", movement.y);

                myRb.MovePosition(myRb.position + movement * speed * Time.fixedDeltaTime);
            }
            else if(!InputDetection(movement))
            {
                myAnim.SetBool("isWalking", false);
                myRb.velocity = Vector3.zero;
            }
       }
       else
        myRb.velocity = Vector3.zero;
    }
    public bool InputDetection(Vector2 move)
    {
        return (move.x >= gameManager.stickSensitivity || move.x <= -gameManager.stickSensitivity|| move.y >= gameManager.stickSensitivity || move.y <= -gameManager.stickSensitivity);
    }
    public Vector2 ReturnMoveVector()
    {
        return playerInput.actions["Move"].ReadValue<Vector2>();
    }
    void PlayerInteract()
    {
        var facingDir = new Vector3(myAnim.GetFloat("moveX"), myAnim.GetFloat("moveY"));
        var temp = new Vector3(transform.position.x, transform.position.y + 0.25f);
        
        var interactPos = temp + facingDir * 0.5f;
        // var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapPoint(interactPos, interactableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
            // ? --> prevents crashing when GetComponent Fails.
        }

        // Debug.DrawLine(temp, interactPos, Color.green, 0.5f);
    }

    void IsInteracting()
    {
        isInteracting = !isInteracting;
        myAnim.SetBool("isWalking",false);
    }

    void inventorySetUp()
    {
        playerUI.SetActive(!playerUI.activeSelf);
        playerUIList[0].SetActive(true);
        playerUIList[1].SetActive(false);
        playerUIList[2].SetActive(false);
        playerUIList[3].SetActive(false);

        statusNavigation.normalize_navigation();
    }
    public void HitStatus()
    {
        inventorySetUp();
        iconnavigation.status_enableStart();
        IsInteracting();
    }
    public void HitMenu()
    {
        canvas.SetActive(!canvas.activeSelf);
        pauseMenu.SpawnPauseMenu(canvas.activeSelf);
        IsInteracting();
    }

    public bool Press_Key(string input_tag)
    {
        return playerInput.actions[input_tag].triggered;
    }
    public bool CheckKeyboardControl()
    {
        return playerInput.actions["Interact"].controls[0].ToString().Contains("Keyboard");
    }

    public string Press_Direction()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();

        if((movement.y >= gameManager.stickSensitivity))
            return "UP";

        else if(movement.y <= -gameManager.stickSensitivity)
            return "DOWN";
        
        else if(movement.x >= gameManager.stickSensitivity)
            return "RIGHT";
        
        else if( (movement.x <= -gameManager.stickSensitivity))
            return "LEFT";
        
        return "";
    }
}
