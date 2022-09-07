using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class LocationPortal : MonoBehaviour
{
    [SerializeField] string portal_id;
    [SerializeField] string arrival_scene;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnterDirection enterDirection;

    [Header("Optional")]
    [SerializeField] private bool outdoor_to_indoor;
    [SerializeField] private bool indoor_to_outdoor;
    [SerializeField] private Animator enterAnimator;
    [SerializeField] private CameraSwitch camera_switch;

    private PlayerMovement player_movement;
    private bool canEnter;

    void Start()
    {
        player_movement = PlayerMovement.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canEnter = true;
        if (enterAnimator != null){
            // Make sure the SpawnPoint is close enough to the Portal Collider.
            // When Player is exiting, Player collider should be in contact with the portal collider to trigger the DoorOpen animation.
            enterAnimator.Play("DoorOpen", -1, 0f);
        }
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
                Time.timeScale = 0;

                player_movement.FaceAdjustment(direction);
                
                if (enterAnimator != null)
                    StartCoroutine(DelayEnter(0.5f));

                else
                {
                    FadeAndTeleport();
                }
            }
        }
    }

    private void FadeAndTeleport()
    {
        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");

        StartCoroutine(ManageScenes(0.35f));
    }
    IEnumerator DelayEnter(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        FadeAndTeleport();
    }

    IEnumerator ManageScenes(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        player_movement.current_portalID = portal_id;

        SceneDetails target_scene = null;
        foreach(GameObject trigger in GameObject.FindGameObjectsWithTag("SceneTrigger")) // inactive objects will not be targeted.
        {
            target_scene = trigger.GetComponent<SceneDetails>();
            if (target_scene.scene_name == this.arrival_scene)
                break;
        }

        if (outdoor_to_indoor)
        {
            target_scene.LoadScene(this);
            GameManager.gm_instance.CurrentScene.UnloadChainedScenes();
        }
        else if (indoor_to_outdoor)
        {
            var unloading_scene = GameManager.gm_instance.CurrentScene;
            TeleportPlayer();
            unloading_scene.UnloadScene();
        }
        else
        {
            target_scene.LoadScene(this);
            GameManager.gm_instance.CurrentScene.UnloadScene();
        }
    }

    public void TeleportPlayer()
    {
        LocationPortal destinationPortal = null;
        foreach(GameObject portal in GameObject.FindGameObjectsWithTag("LocationPortal"))
        {
            destinationPortal = portal.GetComponent<LocationPortal>();
            if (destinationPortal != this && destinationPortal.portal_id == this.portal_id)
                break;
        }
        
        player_movement.transform.position = destinationPortal.spawnPoint.position;

        // Toggle camera view
        if (camera_switch != null)
            camera_switch.SwitchCamera();
        
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");

        Time.timeScale = 1;
    }


    public Transform SpawnPoint => spawnPoint;

    public enum EnterDirection { UP, DOWN, LEFT, RIGHT };
}
