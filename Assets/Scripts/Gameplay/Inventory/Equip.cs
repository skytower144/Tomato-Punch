using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equip", menuName = "Inventory/Equipment")]
public class Equip : Item
{
    public string SkillAnimation;
    public List <GameObject> HitEffects;
    public int skillDamage;
}
