using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusNavigation : MonoBehaviour
{
    [SerializeField] tomatoStatus tomatostatus;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] private GameObject pointBase, pointFrame;
    [System.NonSerialized] public bool navigating_status;
    private RectTransform pointBaseTransform;
    private int statusNumber;
    void Start()
    {
        pointBaseTransform = pointBase.GetComponent<RectTransform>();
    }
    void OnEnable()
    {
        navigating_status = false;
        statusNumber = 2;
    }
    void OnDisable()
    {
        normalize_navigation();
    }
    void Update()
    {
        if(!navigating_status && playerMovement.Press_Key("Interact"))
        {
            activate_navigation();
        }

        else if(navigating_status)
        {
            if( playerMovement.Press_Direction("UP") && BoundaryCheck("UP"))
            {
                IncreaseNumber();
                pointBaseTransform.anchoredPosition = new Vector3(pointBaseTransform.anchoredPosition.x, + pointBaseTransform.anchoredPosition.y + 87f);
            }
            else if( playerMovement.Press_Direction("DOWN") && BoundaryCheck("DOWN"))
            {
                DecreaseNumber();
                pointBaseTransform.anchoredPosition = new Vector3(pointBaseTransform.anchoredPosition.x, + pointBaseTransform.anchoredPosition.y - 87f);
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                tomatostatus.IncreaseStat(statusNumber);
            }
            else if(playerMovement.Press_Key("Cancel"))
            {
                normalize_navigation();
            }
        }
    }
    private void activate_navigation()
    {
        navigating_status = true;
        pointBase.SetActive(true);
        pointFrame.SetActive(true);
    }

    public void normalize_navigation()
    {
        navigating_status = false;
        pointBase.SetActive(false);
        pointFrame.SetActive(false);

        pointBaseTransform = pointBase.GetComponent<RectTransform>();
        pointBaseTransform.anchoredPosition = new Vector3(pointBaseTransform.anchoredPosition.x, 116f);
        statusNumber = 2;
    }

    private bool BoundaryCheck(string direction)
    {
        if (direction == "UP")
        {
            if (pointBaseTransform.anchoredPosition.y + 87f > 116f)
                return false;
        }
        else if (direction == "DOWN")
        {
            if (pointBaseTransform.anchoredPosition.y - 87f < -58f)
                return false;
        }

        return true;
    }

    private void IncreaseNumber()
    {
        if (statusNumber + 1 < 3)
            statusNumber += 1;
    }
    private void DecreaseNumber()
    {
        if (statusNumber - 1 > -1)
            statusNumber -= 1;
    }
}