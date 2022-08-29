using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera playerCamera_instance;
    public Camera player_camera;
    public Canvas player_uiCanvas;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Transform essential_transform, player_transform;

    private float canvas_x, canvas_y;
    private RectTransform canvas_pos;

    void Awake()
    {
        if (playerCamera_instance != null)
        {
            return;
        }

        playerCamera_instance = this;
    }

    public void TogglePlayerCamera()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        Change_UI_Hierarchy();
    }

    private void Change_UI_Hierarchy()
    {
        if (!gameObject.activeSelf) {
            // Backup canvas position before entering.
            canvas_pos = uiCanvas.GetComponent<RectTransform>();
            canvas_x = canvas_pos.localPosition.x;
            canvas_y = canvas_pos.localPosition.y;

            uiCanvas.transform.SetParent(essential_transform);
        }
        else {
            uiCanvas.transform.SetParent(player_transform);
            
            // Recover canvas position when exiting.
            canvas_pos.localPosition = new Vector2(canvas_x, canvas_y);
        }
    }

    public void StapleUICanvas(Transform camera_pos)
    {
        // Staple canvas position according to the camera position when entering.
        canvas_pos.localPosition = new Vector2(camera_pos.localPosition.x, camera_pos.localPosition.y);
    }

}
