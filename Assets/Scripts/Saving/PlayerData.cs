using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public Vector3 postion;
    public int stat_points, money, level;
    public float max_health, current_health, max_guard, attack, total_exp, left_exp, expBar_max, expBar_current;
    public string equip_left, equip_right, equip_super;
    public int slot_index_left, slot_index_right, slot_index_super;
    public List<string> carrying_equip_list = new List<string>();
}
