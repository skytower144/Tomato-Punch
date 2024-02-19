using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSortingLayer : MonoBehaviour
{
    [SerializeField] Renderer _renderer;

    void ChangeLayerID(string layerName)
    {
        _renderer.sortingLayerID = SortingLayer.NameToID(layerName);
    }
}
