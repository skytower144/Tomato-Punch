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
    [SerializeField] private PauseMenu pauseMenu; public PauseMenu pause_menu => pauseMenu;
    [SerializeField] private GameObject playerUI, pauseObj, darkFilter, colliderObj, faderObj;
    [SerializeField] private List <GameObject> playerUIList;
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
        if(!isBattle && !TitleScreen.isTitleScreen)
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
       if(!isBattle && !isInteracting && !TitleScreen.isTitleScreen)
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

        Debug.DrawLine(temp, interactPos, Color.green, 0.5f);
    }

    IEnumerator IsInteracting(float interval)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(interval));
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

        if (checking_direction == "UP")
            return ((x == 0) && (y == 1));
        
        else if (checking_direction == "RU")
            return ((x == 1) && (y == 1));
        
        else if (checking_direction == "RIGHT")
            return ((x == 1) && (y == 0));
        
        else if (checking_direction == "RD")
            return ((x == 1) && (y == -1));
        
        else if (checking_direction == "DOWN")
            return ((x == 0) && (y == -1));

        else if (checking_direction == "LD")
            return ((x == -1) && (y == -1));
        
        else if (checking_direction == "LEFT")
            return ((x == -1) && (y == 0));
        
        else if (checking_direction == "LU")
            return ((x == -1) && (y == 1));
        return false;
    }

    public void SetIsInteracting(bool state)
    {
        isInteracting = state;
        myAnim.SetBool("isWalking", false);
    }
}
