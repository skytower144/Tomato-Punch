using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private List<RectTransform> slotTransforms;

    [SerializeField] private int slotNumber;
    private bool isAnimating = false;

    private void OnEnable()
    {
        pauseMenu.is_busy = true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < 3; i++)
        {
            DOTween.Complete($"saveslot_Hover_On_{i}");
            DOTween.Complete($"saveslot_Hover_Off_{i}");
        }
        slotNumber = 0;

        isAnimating = false;
        slotTransforms[0].localScale = new Vector3(1.1f, 1.1f, 1f);
        slotTransforms[1].localScale = new Vector3(1f, 1f, 1f);
        slotTransforms[2].localScale = new Vector3(1f, 1f, 1f);
    }

    void Update()
    {
        if (playerMovement.Press_Key("Move"))
        {
            Navigate();
        }
        else if(playerMovement.Press_Key("Pause"))
        {
            pauseMenu.is_busy = false;
            gameObject.SetActive(false);
            playerMovement.HitMenu();
        }
        else if(playerMovement.Press_Key("Cancel"))
        {
           pauseMenu.is_busy = false;
           gameObject.SetActive(false);
        }
    }

    private void Navigate()
    {
        if (!isAnimating)
        {
            int prevSlotNumber = slotNumber;

            string direction = playerMovement.Press_Direction();

            if (direction == "UP")
            {
                slotNumber -= 1;
                if (slotNumber < 0)
                    slotNumber = 0;
            }
            
            else if (direction == "DOWN")
            {
                slotNumber += 1;
                if (slotNumber > 2)
                    slotNumber = 2;
            }

            if (prevSlotNumber != slotNumber)
            {
                NormalizeSlot(prevSlotNumber);
                HighLightSlot(slotNumber);
            }
        }
    }

    private void NormalizeSlot(int number)
    {
        DOTween.Rewind($"saveslot_Hover_Off_{number}");
        DOTween.Play($"saveslot_Hover_Off_{number}");
    }

    private void HighLightSlot(int number)
    {
        DOTween.Rewind($"saveslot_Hover_On_{number}");
        DOTween.Play($"saveslot_Hover_On_{number}");
    }

    public void DisableNavigation()
    {
        isAnimating = true;
    }

    public void EnableNavigation()
    {
        isAnimating = false;
    }
}
