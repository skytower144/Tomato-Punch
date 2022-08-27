using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

// Teleports the player  to a different position without loading/switching scenes.
public class LocationPortal : MonoBehaviour
{
    [SerializeField] string portal_id;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnterDirection enterDirection;

    [Header("Optional")]
    [SerializeField] private Animator enterAnimator;
    [SerializeField] private CameraSwitch camera_switch;

    private PlayerMovement player_movement;
    private bool canEnter;

    void Start()
    {
        player_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
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

        StartCoroutine(TeleportPlayer(0.35f));
    }
    IEnumerator DelayEnter(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        FadeAndTeleport();
    }

    IEnumerator TeleportPlayer(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        player_movement.current_portalID = portal_id;

        var destinationPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.portal_id == this.portal_id);
        
        player_movement.transform.position = destinationPortal.spawnPoint.position;

        if (camera_switch != null)
            camera_switch.SwitchCamera();
        
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");

        Time.timeScale = 1;
    }

    public Transform SpawnPoint => spawnPoint;

    public enum EnterDirection { UP, DOWN, LEFT, RIGHT };
}
