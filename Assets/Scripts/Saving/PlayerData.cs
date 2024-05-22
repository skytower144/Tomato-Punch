using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string current_scene = SceneName.TomatoHouse.ToString();
    public Vector3 postion;
    public bool isCameraFixated;
    public int stat_points, money, level;
    public float max_health, current_health, max_guard, attack, total_exp, left_exp, expBar_max, expBar_current;
    public string equip_left, equip_right, equip_super;
    public int slot_index_left = -1;
    public int slot_index_right = -1;
    public int slot_index_super = -1;
    public List<string> carrying_equip_list = new List<string>();
    public List<SerializedItemQuantity> carrying_countable_list = new List<SerializedItemQuantity>();
    public List<Quest> assignedQuests = new List<Quest>();
    public List<Quest> unassignedQuests = new List<Quest>();
    public List<PlayerKeyEvent> keyEventList = new List<PlayerKeyEvent>();
    public List<PartyMember> partyMembers = new List<PartyMember>();
}

[System.Serializable]
public class SerializedItemQuantity
{
    public string item_name;
    public int item_count;

    public SerializedItemQuantity (string item_name, int item_count)
    {
        this.item_name = item_name;
        this.item_count = item_count;
    }
}
