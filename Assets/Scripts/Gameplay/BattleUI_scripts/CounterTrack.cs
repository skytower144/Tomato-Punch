using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CounterTrack : MonoBehaviour
{
    [SerializeField] private List<Image> counterIcon;
    [SerializeField] private tomatoControl tomatocontrol;
    public void CounterTracker()
    {
        for (int i=0; i<5; i++)
        {
            counterIcon[i].enabled = CheckCounterPoint(i);
        }
    }

    private bool CheckCounterPoint(int counterPt)
    {
        return (counterPt < tomatocontrol.tomatoes);
    }

}
