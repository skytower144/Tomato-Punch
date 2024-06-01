using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour, Character
{
    public static PlayerMovement instance { get; private set; }
    public LayerMask interactableLayer;
    public Animator myAnim;
    public BoxCollider2D myCol;
    public PlayerInput playerInput;
    public PlayerCamera cameraControl;
    public GameManager gameManager;
    public PauseMenu pauseMenu;

    public GameObject playerSprite, playerUI, pauseObj, darkFilter, faderObj;
    public GameObject newspaper;
    private Rigidbody2D myRb;

    [SerializeField] private float speed;
    public float player_speed => speed;
    private Vector2 movement;

    [System.NonSerialized] public string current_portalID;
    
    public static bool isBattle = false;
    public bool is_interacting => isInteracting;
    private bool isInteracting = false;
    private bool isAnimating = false;

    private float stepTimer = 0f;
    private float stepInterval;
    private int stepCounter = 0;

    // public event Action BeginBattle;

    private void Awake()
    {
        if (instance != null) return;
        instance = this;
    }
    void Start()
    {
       myRb = GetComponent<Rigidbody2D>();
       stepInterval = Time.deltaTime;
    }
    
    void OnEnable()
    {
        playerInput.actions.Enable();
    }
    void OnDisable()
    {
        playerInput.actions.Disable();
    }
    public void HandleUpdate()
    {
        if (PlayerCanMove())
        {
            if (Press_Key("Interact"))
            {
                PlayerInteract();
            }
            else if (Press_Key("Status"))
            {
                HitStatus();
            }
            else if (Press_Key("Pause"))
            {
                HitMenu();
            }
        }
    }

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
                
                // CountSteps();
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

    // Step Tracker QuickSave
    private void CountSteps()
    {
        if (stepCounter > gameManager.save_load_menu.AutoSaveStep && PlayerCanMove()) {
            ResetStepCounter();
            gameManager.save_load_menu.ProceedSave(3);
        }
        stepTimer += stepInterval;
        stepCounter = (int) stepTimer;
        // GameManager.DoDebug($"{stepCounter}");
    }

    public void ResetStepCounter()
    {
        stepTimer = 0f;
        stepCounter = 0;
    }

    public void Animate(bool isAnimating, Vector2 direction = default, bool flattenPos = true)
    {
        myAnim.SetBool("isWalking", isAnimating);

        if (!isAnimating) return;
        
        direction = direction.normalized;
        myAnim.SetFloat("moveX", Mathf.Round(direction.x));
        myAnim.SetFloat("moveY", Mathf.Round(direction.y));
    }

    public bool InputDetection(Vector2 move)
    {
        return move.x >= gameManager.stickSensitivity || move.x <= -gameManager.stickSensitivity|| move.y >= gameManager.stickSensitivity || move.y <= -gameManager.stickSensitivity;
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

        if (AppSettings.IsUnityEditor) Debug.DrawLine(temp, interactPos, Color.green, 0.5f);
    }

    IEnumerator IsInteracting(float interval)
    {
        yield return WaitForCache.GetWaitForSecondReal(interval);
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

        if (movement.y >= gameManager.stickSensitivity)
            return "UP";

        else if (movement.y <= -gameManager.stickSensitivity)
            return "DOWN";
        
        else if (movement.x >= gameManager.stickSensitivity)
            return "RIGHT";
        
        else if (movement.x <= -gameManager.stickSensitivity)
            return "LEFT";
        
        return "";
    }

    public IEnumerator DelayFaceAdjustment(string direction, float delay)
    {
        yield return WaitForCache.GetWaitForSecondReal(delay);
        DialogueManager.instance.cutsceneHandler.FaceAdjustment(myAnim, direction);
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

    public void SetPlayerSpeed(float input_speed)
    {
        speed = input_speed;
    }

    public IEnumerator PlayMoveActions(string[] posStrings, float moveSpeed, bool isAnimate)
    {
        string[] posString;
        float originalSpeed = speed;
        myCol.enabled = false;

        if (moveSpeed != -1f)
            speed = moveSpeed;

        foreach (string xy in posStrings) {
            posString = xy.Split('~');
            float x = posString[0] == "_" ? transform.position.x : float.Parse(posString[0]);
            float y = posString[1] == "_" ? transform.position.y : float.Parse(posString[1]);
            Vector2 targetPos = new Vector2(x, y);

            while ((targetPos - myRb.position).magnitude >= 0.01f) {
                yield return Walk(targetPos, isAnimate);
            }
            transform.position = new Vector3(x, y, transform.position.z);
        }
        speed = originalSpeed;
        myRb.velocity = Vector2.zero;

        Animate(false, default, false);
        myCol.enabled = true;
    }

    IEnumerator Walk(Vector2 movePos, bool isAnimate)
    {
        Vector2 direction = movePos - myRb.position;
        float distance = direction.magnitude;

        float movementSpeed = Mathf.Min(speed * Time.deltaTime, distance);
        Vector2 amount = direction.normalized * movementSpeed;

        myRb.position += amount;
        
        if (isAnimate) Animate(true, direction);
        yield break;
    }

    public void SetIsAnimating(bool state)
    {
        isAnimating = state;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public Animator UsesDefaultAnimator()
    {
        return myAnim;
    }

    public void Play(string clipName, Action dialogueAction = null, bool stopAfterAnimation = false)
    {
        myAnim.Play(clipName, -1, 0f);
    }

    public void Turn(string direction)
    {
        DialogueManager.instance.cutsceneHandler.FaceAdjustment(myAnim, direction);
    }
    
    public void Teleport(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }
    public Vector3 GetPlayerPos()
    {
        return transform.position;
    }
    public bool PlayerCanMove()
    {
        return (
            !isBattle &&
            !isInteracting &&
            !TitleScreen.isTitleScreen &&
            !gameManager.save_load_menu.isLoading &&
            playerSprite.activeSelf &&
            !isAnimating &&
            Time.timeScale == 1
        );
    }
}

public static class InputDir
{
    public const string UP = "UP";
    public const string DOWN = "DOWN";
    public const string RIGHT = "RIGHT";
    public const string LEFT = "LEFT";
}
