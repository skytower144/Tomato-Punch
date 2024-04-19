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
    void Update()
    {
        CheckGaksung();
    }
    public void SetWhiteBar()
    {
        parryWhiteBarFill.fillAmount = parryFill.fillAmount;
    }
    public void CheckGaksung()
    {
        if (parryFill.fillAmount == 1)
        {
            if (!parry_fullCharge.activeSelf)
                parry_fullCharge.SetActive(true);
            
            if (!gaksungOn)
            {
                gaksungOn = true;
                
                if (gameObject.activeSelf)
                    gaksung.SetActive(true);
            }
        }
    }
}
