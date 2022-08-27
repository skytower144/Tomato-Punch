using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera playerCamera_instance;
    public Camera player_camera;
    public Canvas player_uiCanvas;
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
    }
}
