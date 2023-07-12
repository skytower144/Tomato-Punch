using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherPoints : MonoBehaviour
{
    [SerializeField] private List<GameObject> featherList;
    private int featherPoint;

    void OnEnable()
    {
        ResetFeather();
    }

    public void AddFeatherPoint()
    {
        featherPoint ++;
        for (int i = 0; i < 5; i++)
            featherList[i].SetActive(CheckFeatherPoint(i));
    }

    private bool CheckFeatherPoint(int comparePoint)
    {
        return (comparePoint < featherPoint);
    }

    public void ResetFeather()
    {
        foreach (GameObject feather in featherList) {
            feather.SetActive(false);
        }
        featherPoint = 0;
    }
}
