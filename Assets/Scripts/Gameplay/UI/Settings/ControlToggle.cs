using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlToggle : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private RebindKey rebindKey;
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!rebindKey.isBinding)
            controlScroll.ControlInteractMenu();
    }    
}
