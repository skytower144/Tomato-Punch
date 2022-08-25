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
    [SerializeField] private Animator enterAnimator;
    private PlayerMovement player_movement;
    private bool canEnter;

    void Start()
    {
        player_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
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
                Time.timeScale = 0;

                player_movement.FaceAdjustment(direction);
                
                if (enterAnimator != null)
                    StartCoroutine(PlayEnterAnimation(0.5f));

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
    IEnumerator PlayEnterAnimation(float waitTime)
    {
        enterAnimator.Play("DoorOpen", -1, 0f);

        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));

        FadeAndTeleport();
    }

    IEnumerator TeleportPlayer(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        player_movement.current_portalID = portal_id;

        var destinationPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.portal_id == this.portal_id);
        
        player_movement.transform.position = destinationPortal.spawnPoint.position;
        
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");
        Time.timeScale = 1;
    }

    public Transform SpawnPoint => spawnPoint;

    public enum EnterDirection { UP, DOWN, LEFT, RIGHT };
}
