using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParryBar : MonoBehaviour
{
    public GameObject parryWhiteBar;
    [SerializeField] private Image parryWhiteBarFill;
    public Image parryFill;
    public GameObject parry_fullCharge;
    [SerializeField] private GameObject parryBar;
    [SerializeField] private GameObject gaksung;
    [System.NonSerialized] public bool gaksungOn = false;
    private void Update()
    {
        if(parryFill.fillAmount == 1)
        {
            if(parryBar.activeSelf)
            {
                parry_fullCharge.SetActive(true);
            }
            if(!gaksungOn && !tomatoControl.isGuard)
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

    void OnDisable()
    {
        gaksungOn = false;
        gaksung.SetActive(false);
        parry_fullCharge.SetActive(false);
    }
   
}
