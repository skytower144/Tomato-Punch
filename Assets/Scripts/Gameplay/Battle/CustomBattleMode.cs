using UnityEngine;

[System.Serializable]
public class CustomBattleMode
{
    public GameObject cartridge;
    public bool spawnAtEnemy = false;

    public void ChangeBattleMode()
    {
        GameManager.gm_instance.battle_system.textSpawn.SwitchCartridge(cartridge, spawnAtEnemy);
    }
}
