using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JolaController : MonoBehaviour, Interactable
{
    private Animator playerAnimator;
    public GameObject playerObject;
    private float player_x, player_y;
    [SerializeField] private GameObject battleCircle, exclamation;
   
    public void Interact()
    {
        PlayerMovement.isBattle = true;

        playerAnimator = playerObject.GetComponent<Animator>();
        playerAnimator.SetBool("isWalking",false);

        Instantiate (exclamation, new Vector2 (transform.position.x - 0.05f, transform.position.y + 3.1f), Quaternion.identity);
        Invoke("battleStart_ef", 0.4f);

    }
    public void battleStart_ef()
    {
        player_x = GameObject.Find("Player").transform.position.x;
        player_y = GameObject.Find("Player").transform.position.y;
        Destroy(Instantiate (battleCircle, new Vector2 (player_x-2.6f, player_y), Quaternion.identity),2f);
    }
}

