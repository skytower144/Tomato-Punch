using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Camera customCamera;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCamera.playerCamera_instance.TogglePlayerCamera();
        customCamera.enabled = !customCamera.enabled;
        SetCustomView();
    }

    public void SwitchCamera() // OnExit
    {
        PlayerCamera.playerCamera_instance.TogglePlayerCamera();
        customCamera.enabled = !customCamera.enabled;
        SetPlayerView();
    }

    private void SetCustomView()
    {
        PlayerCamera.playerCamera_instance.player_uiCanvas.worldCamera = customCamera;
    }

    private void SetPlayerView()
    {
        PlayerCamera.playerCamera_instance.player_uiCanvas.worldCamera = PlayerCamera.playerCamera_instance.player_camera;
    }
}
