using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LocationPortal : MonoBehaviour, Interactable
{
    [SerializeField] string portal_id;
    [SerializeField] SceneName arrival_scene;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnterDirection enterDirection;

    [SerializeField] private bool outdoor_to_indoor;
    [SerializeField] private bool indoor_to_outdoor;

    [Header("Optional")]
    [SerializeField] private Animator enterAnimator;
    [SerializeField] private CameraSwitch camera_switch;
    [SerializeField] private string quest_id;

    private static string outdoorSceneName = "";

    private PlayerMovement player_movement;
    private bool canEnter = false;

    void Start()
    {
        player_movement = PlayerMovement.instance;
    }
    public void Interact()
    {
        if (IsLocked()) {
            TextAsset inkJsonData = InkDB.ReturnTextAsset(UIControl.currentLangMode, gameObject.scene.name, quest_id);
            DialogueManager.instance.EnterDialogue(inkJsonData, this);
        }
    }

    private bool IsLocked()
    {
        if (string.IsNullOrEmpty(quest_id)) return false;

        // If Quest is assigned and completed => should be unlocked
        if ((QuestManager.instance.FindQuest(quest_id, QuestList.Unassigned) == null) && (QuestManager.instance.FindQuest(quest_id, QuestList.Assigned).is_completed)) {
            return false;
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLocked()) return;
        EnableDoor();
    }

    public void EnableDoor()
    {
        if (canEnter) return;
        
        canEnter = true;
        if (enterAnimator != null){
            // Make sure the SpawnPoint is close enough to the Portal Collider.
            // When Player is exiting, Player collider should be in contact with the portal collider to trigger the DoorOpen animation.
            enterAnimator.Play("DoorOpen", -1, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsLocked()) return;
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

        foreach (SceneDetails target_scene in SceneControl.instance.sceneDict.Values)
        {
            if (target_scene.scene_name == arrival_scene) {
                target_scene.LoadScene(this);
                break;
            }
        }
    }

    public void TeleportPlayer()
    {
        if (outdoor_to_indoor)
        {
            outdoorSceneName = SceneControl.instance.CurrentScene.GetSceneName();
            if (ProgressManager.instance.assistants.ContainsKey(outdoorSceneName))
                ProgressManager.instance.assistants[outdoorSceneName].levelHolder.SetActive(false);

            SceneControl.instance.CurrentScene.UnloadChainedScenes();
        }
        else if (indoor_to_outdoor)
        {
            if (ProgressManager.instance.assistants.ContainsKey(outdoorSceneName))
                ProgressManager.instance.assistants[outdoorSceneName].levelHolder.SetActive(true);
            
            // var unloading_scene = SceneControl.instance.CurrentScene;
            // TeleportPlayer();
            // unloading_scene.UnloadScene();
        }
        else
        {
            SceneControl.instance.CurrentScene.UnloadScene();
        }

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
            camera_switch.ReturnToPlayerCamera();
        
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");

        Time.timeScale = 1;
    }

    public Transform SpawnPoint => spawnPoint;

    public enum EnterDirection { UP, DOWN, LEFT, RIGHT };
}
