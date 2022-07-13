using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlToggle : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ControlScroll controlScroll;
    public void OnPointerDown(PointerEventData eventData)
    {
        controlScroll.ControlInteractMenu();
    }    
}
