using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParryBar : MonoBehaviour
{
    public GameObject parryWhiteBar;
    [SerializeField] private Image parryWhiteBarFill;
    public Image parryFill;
    public GameObject parryFillUp;
    [SerializeField] private GameObject gaksung;
    [System.NonSerialized] public bool gaksungOn = false;
    private void Update()
    {
        if(parryFill.fillAmount == 1)
        {
            parryFillUp.SetActive(true);
            if(!gaksungOn)
            {
                gaksungOn = true;
                gaksung.SetActive(true);
            }
        }
    }
    public void SetParryBar()
    {
        parryFill.fillAmount = 0;
    }

    public void SetWhiteBar()
    {
        parryWhiteBarFill.fillAmount = parryFill.fillAmount;
    }

   
}
