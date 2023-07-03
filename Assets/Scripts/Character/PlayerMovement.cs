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
    [SerializeField] private PauseMenu pauseMenu; public PauseMenu pause_menu => pauseMenu;
    [SerializeField] private GameObject playerUI, pauseObj, darkFilter, colliderObj, faderObj;
    public GameObject dark_filter => darkFilter;
    public GameObject collider_obj => colliderObj;
    public GameObject fader_obj => faderObj;

    [SerializeField] private float speed;
    private Vector2 movement;

    [System.NonSerialized] public string current_portalID;
    public LayerMask interactableLayer;
    
    public static bool isBattle = false;
    private bool isInteracting = false;
    public bool is_interacting => isInteracting;

    // public event Action BeginBattle;
    public static PlayerMovement instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
       myRb = GetComponent<Rigidbody2D>();
       playerInput = GetComponent<PlayerInput>();
    }
    public void HandleUpdate()
    {
        if (PlayerCanMove())
        {
            if (Press_Key("Interact"))
            {
                PlayerInteract();
            }
            else if (Press_Key("Status")) // ENTER
            {
                HitStatus();
            }
            else if (Press_Key("Pause"))
            {
                HitMenu();
            }
        }
    }

    // Update is called once per frame
    public void FixedUpdate() //For Executing Physics
    {
       if (PlayerCanMove())
       {
            movement = playerInput.actions["Move"].ReadValue<Vector2>();

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
        else {
            myRb.velocity = Vector3.zero;
        }
    }

    public bool InputDetection(Vector2 move)
    {
        return (move.x >= gameManager.stickSensitivity || move.x <= -gameManager.stickSensitivity|| move.y >= gameManager.stickSensitivity || move.y <= -gameManager.stickSensitivity);
    }
    public Vector2 ReturnMoveVector()
    {
        return playerInput.actions["Move"].ReadValue<Vector2>();
    }
    public void PlayerInteract()
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

        Debug.DrawLine(temp, interactPos, Color.green, 0.5f);
    }

    IEnumerator IsInteracting(float interval)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(interval));
        isInteracting = !isInteracting;
        myAnim.SetBool("isWalking",false);
    }
    public void HitStatus()
    {
        playerUI.SetActive(!playerUI.activeSelf);
        StartCoroutine(IsInteracting(0.1f));
    }
    public void HitMenu()
    {
        darkFilter.SetActive(!darkFilter.activeSelf);
        pauseMenu.SpawnPauseMenu(!pauseObj.activeSelf);
        StartCoroutine(IsInteracting(0.2f));
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

    public void FaceAdjustment(string facing_direction)
    {
        float face_x = 0f;
        float face_y = 0f;

        if (facing_direction == "UP")
            face_y = 1f;
        else if (facing_direction == "DOWN")
            face_y = -1f;
        else if (facing_direction == "LEFT")
            face_x = -1f;
        else if (facing_direction == "RIGHT")
            face_x = 1f;

        myAnim.SetFloat("moveX", face_x);
        myAnim.SetFloat("moveY", face_y);
    }

    public bool CheckFacingDirection(string checking_direction)
    {
        int x = Mathf.RoundToInt(myAnim.GetFloat("moveX"));
        int y = Mathf.RoundToInt(myAnim.GetFloat("moveY"));

        switch (checking_direction) {
            case "UP":
                return ((x == 0) && (y == 1));
            
            case "RU":
                return ((x == 1) && (y == 1));

            case "RIGHT":
                return ((x == 1) && (y == 0));
            
            case "RD":
                return ((x == 1) && (y == -1));
            
            case "DOWN":
                return ((x == 0) && (y == -1));
            
            case "LD":
                return ((x == -1) && (y == -1));
            
            case "LEFT":
                return ((x == -1) && (y == 0));

            case "LU":
                return ((x == -1) && (y == 1));
            
            default:
                return false;
        }
    }

    public void SetIsInteracting(bool state)
    {
        isInteracting = state;
        myAnim.SetBool("isWalking", false);
    }

    private bool PlayerCanMove()
    {
        return (!isBattle && !isInteracting && !TitleScreen.isTitleScreen && !gameManager.save_load_menu.isLoading && collider_obj.activeSelf);
    }
}

public static class InputDir
{
    public const string UP = "UP";
    public const string DOWN = "DOWN";
    public const string RIGHT = "RIGHT";
    public const string LEFT = "LEFT";
}
