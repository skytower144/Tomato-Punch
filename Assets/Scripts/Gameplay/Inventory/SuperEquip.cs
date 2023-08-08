using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SuperEquip", menuName = "Inventory/SuperEquipment")]
public class SuperEquip : Item
{
    [HideInInspector] public Sprite superIcon;
    public string animTag;
    [Space(10)]
    public float skillDamage;
}