using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    public static WorldCamera instance;
    [SerializeField] private Animator cameraAnim;
    public void Init()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public void PlayCameraEffect(string camera_tag)
    {
        gameObject.SetActive(true);
        PlayerCamera.playerCamera_instance.player_camera.enabled = false;
        cameraAnim.Play(camera_tag, -1, 0f);
    }

    public void ResetCamera()
    {
        cameraAnim.Play("default", -1, 0f);
        PlayerCamera.playerCamera_instance.player_camera.enabled = true;
        gameObject.SetActive(false);
    }
}
