using System;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    public Camera player_camera;
    public Canvas player_uiCanvas;
    public RectTransform uicanvas_pos;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Transform essential_transform, portable_transform;
    [System.NonSerialized] public bool isCameraFixated = false;
    [System.NonSerialized] public bool isCameraDetached = false; // used for cutscenes
    private float cameraZ;
    void Start()
    {
        cameraZ = player_camera.transform.localPosition.z;
    }

    public void SetPlayerCamera(bool state)
    {
        player_camera.enabled = state;
        Change_UI_Hierarchy();
    }

    private void Change_UI_Hierarchy()
    {
        if (!player_camera.enabled) {
            uiCanvas.transform.SetParent(essential_transform);
            isCameraFixated = true;
        }
        else {
            // Do not change code sequence
            uiCanvas.transform.SetParent(PlayerMovement.instance.transform);
            player_uiCanvas.worldCamera = player_camera;
            uicanvas_pos.localPosition = new Vector2(0, 0);

            isCameraFixated = false;
        }
    }

    public void FixateUICanvas(Transform camera_pos)
    {
        // Fixate canvas position according to the camera position when entering.
        isCameraFixated = true;
        uicanvas_pos.localPosition = new Vector2(camera_pos.localPosition.x, camera_pos.localPosition.y);
        DialogueManager.instance.cutsceneHandler.FadeControl.SetPosition();
    }

    public void RestoreCameraState(bool isCameraFixated)
    {
        this.isCameraFixated = isCameraFixated;
    }

    public void DetachPlayerCamera()
    {
        player_camera.transform.SetParent(portable_transform);
        isCameraDetached = true;
    }
    public void ResetPlayerCamera()
    {
        if (!isCameraDetached)
            return;

        isCameraDetached = false;
        player_camera.transform.SetParent(PlayerMovement.instance.transform);
        player_camera.transform.localPosition = new Vector3(0, 0, cameraZ);
        uiCanvas.transform.localPosition = new Vector3(0, 0, 0);
    }
    public void SetCameraPosition(float x, float y)
    {
        player_camera.transform.position = new Vector3(x, y, cameraZ);
    }
    public void MoveCamera(float x, float y, float duration, string easeTag)
    {
        if (!isCameraDetached)
            DetachPlayerCamera();
        
        Ease defaultEase = Ease.Linear;
        if (easeTag != "_")
            Enum.TryParse(easeTag, true, out defaultEase);

        player_camera.transform.DOMove(new Vector3(x, y, cameraZ), duration).SetEase(defaultEase);
    }
}
