using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Camera player_camera;
    public Canvas player_uiCanvas;
    public RectTransform uicanvas_pos;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Transform essential_transform;
    [System.NonSerialized] public bool isCameraFixated = false;
    
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
        DialogueManager.instance.cutsceneHandler.SetCutscenePosition();
    }

    public void RestoreCameraState(bool isCameraFixated)
    {
        this.isCameraFixated = isCameraFixated;
    }
}
