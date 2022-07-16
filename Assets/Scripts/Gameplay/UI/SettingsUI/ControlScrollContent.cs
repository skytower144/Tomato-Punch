using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlScrollContent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private ControlScroll controllScroll;
    [SerializeField] private RebindKey rebindKey;
    [SerializeField] private int index;
    public void OnPointerEnter(PointerEventData eventData)
    {
        controllScroll.ControlMouseSelect(index);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!rebindKey.isBinding)
            rebindKey.StartRebinding();
    }
}
