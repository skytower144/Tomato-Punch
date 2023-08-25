using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherPoints : MonoBehaviour
{
    [SerializeField] private List<GameObject> featherList;
    private int featherPoint;
    public int feather_point => featherPoint;

    void OnEnable()
    {
        ResetFeather();
    }

    private void SetFeatherUI()
    {
        for (int i = 0; i < 5; i++)
            featherList[i].SetActive(i < featherPoint);
    }

    public void AddFeatherPoint()
    {
        featherPoint++;
        SetFeatherUI();
    }

    public void SubtractFeatherPoint(int amount)
    {
        featherPoint -= amount;
        SetFeatherUI();
    }

    public void ResetFeather()
    {
        foreach (GameObject feather in featherList) {
            feather.SetActive(false);
        }
        featherPoint = 0;
    }
}
