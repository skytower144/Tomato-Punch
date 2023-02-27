using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera playerCamera_instance;
    public Camera player_camera;
    public Canvas player_uiCanvas;
    
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Transform essential_transform, player_transform;
    private RectTransform canvas_pos;

    // SAVING DATA
    [System.NonSerialized] public bool isCameraOff = false;
    public RectTransform uiCanvas_transform;
    [System.NonSerialized] public Vector3 uiCanvas_vector;
    [System.NonSerialized] public float canvas_x, canvas_y;
    
    void Awake()
    {
        if (playerCamera_instance != null)
        {
            return;
        }

        playerCamera_instance = this;
    }
    public void DisablePlayerCamera()
    {
        isCameraOff = true;
        player_camera.enabled = false;
        Change_UI_Hierarchy();
    }

    public void EnablePlayerCamera()
    {
        isCameraOff = false;
        player_camera.enabled = true;
        Change_UI_Hierarchy();
    }

    public void Change_UI_Hierarchy(bool set_only_transform = false)
    {
        bool detach_ui = isCameraOff;

        if (detach_ui) {
            if (!set_only_transform)
            {
                // Backup canvas position before entering.
                canvas_pos = uiCanvas.GetComponent<RectTransform>();
                if ((canvas_x == 0) && (canvas_y == 0))
                {
                    canvas_x = canvas_pos.localPosition.x;
                    canvas_y = canvas_pos.localPosition.y;
                }
            }
            uiCanvas.transform.SetParent(essential_transform);
        }
        else {
            uiCanvas.transform.SetParent(player_transform); // Do not change code sequence
            SetPlayerView();

            if (!set_only_transform)
            {
                // Recover canvas position when exiting.
                canvas_pos.localPosition = new Vector2(canvas_x, canvas_y);

                canvas_x = canvas_y = 0;
            }
            
        }
    }

    public void SetPlayerView()
    {
        player_uiCanvas.worldCamera = player_camera;
    }

    public void RecoverCameraState(bool is_camera_off)
    {
        isCameraOff = is_camera_off;
        player_camera.enabled = !is_camera_off;
    }

    public void StapleUICanvas(Transform camera_pos)
    {
        // Staple canvas position according to the camera position when entering.
        canvas_pos.localPosition = new Vector2(camera_pos.localPosition.x, camera_pos.localPosition.y);
    }

    public void RecoverCanvasBackupPosition(float saved_x, float saved_y)
    {
        canvas_x = saved_x;
        canvas_y = saved_y;
    }

    public void RecoverCanvasCurrentPosition(Vector3 saved_uiCanvas_position)
    {
        uiCanvas_transform.localPosition = saved_uiCanvas_position;
    }
}
