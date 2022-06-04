using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardDetail
{
    public Item RewardItem;

    [Range(0, 100)]
    public float DropChance;
    
}
