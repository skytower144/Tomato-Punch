using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Camera customCamera;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCamera.playerCamera_instance.DisablePlayerCamera();
        PlayerCamera.playerCamera_instance.StapleUICanvas(gameObject.transform);
        customCamera.enabled = true;
        SetCustomView();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        customCamera.enabled = false;
    }

    public void ReturnToPlayerCamera() // OnExit
    {
        PlayerCamera.playerCamera_instance.EnablePlayerCamera();
        customCamera.enabled = false;
        // PlayerCamera.playerCamera_instance.SetPlayerView(); --> Change_UI_Hierarchy
    }

    private void SetCustomView()
    {
        PlayerCamera.playerCamera_instance.player_uiCanvas.worldCamera = customCamera;
    }
}
