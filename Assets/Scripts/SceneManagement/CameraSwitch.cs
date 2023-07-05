using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Camera customCamera;
    [SerializeField] private float customPlayerSpeed;
    private float cacheSpeed;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetPlayerSpeed();

        PlayerCamera.playerCamera_instance.DisablePlayerCamera();
        PlayerCamera.playerCamera_instance.StapleUICanvas(gameObject.transform);
        customCamera.enabled = true;
        SetCustomView();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        NormalizePlayerSpeed();
        
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

    private void SetPlayerSpeed()
    {
        cacheSpeed = PlayerMovement.instance.player_speed;
        PlayerMovement.instance.SetPlayerSpeed(customPlayerSpeed);
    }

    private void NormalizePlayerSpeed()
    {
        PlayerMovement.instance.SetPlayerSpeed(cacheSpeed);
    }
}
