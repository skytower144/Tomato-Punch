using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    [SerializeField] private Camera loadingCamera;
    [SerializeField] private GameObject loadingDisplay;
    [SerializeField] private List<TextAndFont> textDataList = new List<TextAndFont>();

    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public void EnableLoadingScreen()
    {
        UIControl.instance.SwitchLanguage(textDataList, UIControl.currentLangMode);
        
        PlayerCamera.playerCamera_instance.player_camera.enabled = false;
        loadingDisplay.SetActive(true);
        loadingCamera.enabled = true;

        DOTween.Rewind("loading_in");
        DOTween.Play("loading_in");
    }

    private void DisableLoadingScreen()
    {
        loadingCamera.enabled = false;
        loadingDisplay.SetActive(false);
                
        PlayerCamera.playerCamera_instance.RecoverCameraState(PlayerCamera.playerCamera_instance.isCameraOff);
        PlayerCamera.playerCamera_instance.Change_UI_Hierarchy(true);
        PlayerCamera.playerCamera_instance.RecoverCanvasCurrentPosition(PlayerCamera.playerCamera_instance.uiCanvas_vector);
        PlayerCamera.playerCamera_instance.RecoverCanvasBackupPosition(PlayerCamera.playerCamera_instance.canvas_x, PlayerCamera.playerCamera_instance.canvas_y);
        
        GameManager.gm_instance.save_load_menu.PrepareMenu();
        PlayerMovement.instance.collider_obj.SetActive(true);

        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");
    }

    public IEnumerator UncoverLoadingScreen()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        DOTween.Rewind("loading_out");
        DOTween.Play("loading_out");
        yield return new WaitForSecondsRealtime(0.8f);

        DisableLoadingScreen();
    }
}
