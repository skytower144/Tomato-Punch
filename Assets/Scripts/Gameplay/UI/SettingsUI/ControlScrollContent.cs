using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlScrollContent : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private ControlScroll controllScroll;
    [SerializeField] private int index;
    public void OnPointerEnter(PointerEventData eventData)
    {
        controllScroll.ControlMouseSelect(index);
    }
}
