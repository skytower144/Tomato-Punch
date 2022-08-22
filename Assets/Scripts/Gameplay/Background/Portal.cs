using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string portal_id;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnterDirection enterDirection;
    private PlayerMovement player_movement;
    private bool canEnter;

    void Start()
    {
        player_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        if ((player_movement.current_portalID != "") && (player_movement.current_portalID == portal_id))
            player_movement.transform.position = spawnPoint.position;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canEnter = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canEnter = false;
    }

    void Update()
    {
        if (canEnter)
        {
            string direction = player_movement.Press_Direction();
            if (direction == this.enterDirection.ToString()) {
                canEnter = false;
                StartCoroutine(SwitchScene());
            }
        }
    }


    IEnumerator SwitchScene()
    {
        player_movement.current_portalID = portal_id;

        // Wait until scene is completely loaded.
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    public Transform SpawnPoint => spawnPoint;

    public enum EnterDirection { UP, DOWN, LEFT, RIGHT };

}
