using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    [SerializeField] private Camera loadingCamera;
    [SerializeField] private GameObject loadingDisplay;
    [SerializeField] private TextMeshProUGUI loadingText;

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
        loadingText.text = UIControl.instance.uiTextDict["LoadingText"];
        UIControl.instance.SetFontData(loadingText, "LoadingText");
        
        PlayerMovement.instance.cameraControl.player_camera.enabled = false;
        loadingDisplay.SetActive(true);
        loadingCamera.enabled = true;
        gameObject.SetActive(true);

        DOTween.Rewind("loading_in");
        DOTween.Play("loading_in");
    }

    private void DisableLoadingScreen()
    {
        loadingCamera.enabled = false;
        gameObject.SetActive(false);
        loadingDisplay.SetActive(false);

        PlayerMovement.instance.cameraControl.SetPlayerCamera(!PlayerMovement.instance.cameraControl.isCameraFixated);
        
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
