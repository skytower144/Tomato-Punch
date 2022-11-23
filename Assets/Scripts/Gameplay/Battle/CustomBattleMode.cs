using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBattleMode : MonoBehaviour
{
    public GameObject cartridge;

    public void ChangeBattleMode()
    {
        GameManager.gm_instance.battle_system.textSpawn.SwitchCartridge(cartridge);
    }
}
