using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSpawn : MonoBehaviour
{
    [SerializeField] private GameObject missEffect;
    [System.NonSerialized] static public bool isMiss = false;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private GameObject GetReadyText;
    private Vector3 randomPosition;

    private void OnEnable()
    {
        Instantiate(GetReadyText, transform);
    }
    void Update()
    {
        if (isMiss)
        {
            randomPosition = Random.insideUnitSphere * 1.5f + new Vector3(-30,0,0);
            GameObject miss = Instantiate(missEffect, transform);
            miss.transform.position = randomPosition;

            decreaseStamina();
            isMiss = false;
        }
    }

    void decreaseStamina()
    {
        tomatocontrol.currentStamina -= 1;
        if (tomatocontrol.currentStamina < 0)
            tomatocontrol.currentStamina = 0;

        staminaIcon.SetStamina(tomatocontrol.currentStamina);
    }
}
