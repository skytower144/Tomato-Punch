using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OptionScript : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private List <GameObject> optionList;
    [System.NonSerialized] public bool is_busy_option;
    private int optionNumber;
    void Update()
    {
        if(is_busy_option)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                playerMovement.HitMenu();
                CloseOptions();
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                CloseOptions();
            }
        }
    }
    public void OpenOptions()
    {
        is_busy_option = true;

        optionNumber = 0;
        optionList[0].SetActive(true);

        DOTween.Rewind("open_option");
        DOTween.Play("open_option");
    }
    private void CloseOptions()
    {
        is_busy_option = false;
        DOTween.Rewind("close_option");
        DOTween.Play("close_option");
    }

    public void TurnoffOption()
    {
        optionList[optionNumber].SetActive(false);
    }
}
