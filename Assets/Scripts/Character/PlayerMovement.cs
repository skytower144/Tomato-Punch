using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D myRb;
    private Animator myAnim;
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
    public event Action BeginBattle;

    // Start is called before the first frame update
    void Start()
    {
       myRb = GetComponent<Rigidbody2D>();
       myAnim = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if(!isBattle)
        {
            if(!isInteracting && (Input.GetKeyDown(KeyCode.O) || (Input.GetKeyDown("joystick button 0"))))
            {
                PlayerInteract();
            }
            else if(!isInteracting && Input.GetKeyDown(KeyCode.Return))
            {
                HitStatus();
            }
            else if(!statusNavigation.navigating_status && !SlotNavigation.isBusy && playerUI.activeSelf && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown("joystick button 1")))
            {
                HitStatus();
            }
            else if(!isInteracting && Input.GetKeyDown(KeyCode.Escape))
            {
                HitMenu();
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
        var temp = new Vector3(transform.position.x, transform.position.y - 0.7f);
        var interactPos = temp + facingDir*0.5f;
        // var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapPoint(interactPos, interactableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
            // ? --> prevents crashing when GetComponent Fails.
        }

        //Debug.DrawLine(temp, interactPos, Color.green, 0.5f);
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
}
