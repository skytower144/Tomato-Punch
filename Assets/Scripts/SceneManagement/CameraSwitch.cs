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

        PlayerMovement.instance.cameraControl.SetPlayerCamera(false);
        PlayerMovement.instance.cameraControl.FixateUICanvas(gameObject.transform);
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
        PlayerMovement.instance.cameraControl.SetPlayerCamera(true);
        customCamera.enabled = false;
        // PlayerMovement.instance.cameraControl.SetPlayerView(); --> Change_UI_Hierarchy
    }

    private void SetCustomView()
    {
        PlayerMovement.instance.cameraControl.player_uiCanvas.worldCamera = customCamera;
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
