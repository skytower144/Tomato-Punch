using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringProgressData : SerializableDictionary<string, ProgressData>{}

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance { get; private set; }
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private tomatoStatus tomatostatus;
    [SerializeField] private TomatoLevel tomatolevel;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemManager itemManager;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private string selectedProfileId = "test";
    public string selected_profile_id => selectedProfileId;

    private FileDataHandler dataHandler;
    public FileDataHandler pm_dataHandler => dataHandler;
    public SaveData save_data = new SaveData();
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Progress Manager in scene.");
        }
        instance = this;
    }

    private void Start()
    {
        // Application.persistentDataPath will give the OS standard directory for persisting data in a Unity project.
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        
        LoadSaveData(); // every time when game starts - repeat

        EquipDB.Initiatlize();
        ItemPrefabDB.Initiatlize();
        
        if (save_data.player_data.max_health == 0){ // only once since new game : initalizing dictionary keys and values.
            SavePlayerData(save_data);
        }

        playerInventory.GatherSlots(); // repeat
        LoadPlayerData(); // repeat
        itemManager.RecoverItemState(); // repeat

        if (!this.dataHandler.CheckFileExists("Slot_New")) // Backup for clean slot
        {
            pm_dataHandler.Save(save_data, "Slot_New");
        }
    }

    public void CaptureScene(bool saveSingleScene = false, string targetSceneName = null)
    {
        // Capture Objects' progress before unloading.
        foreach (GameObject assistant in GameObject.FindGameObjectsWithTag("ProgressAssistant"))
        {
            if (saveSingleScene)
            {
                if (assistant.scene.name == targetSceneName) {
                    assistant.GetComponent<ProgressAssistant>().InitiateCapture();
                    Debug.Log("capturing " + assistant.scene.name);
                    return;
                }
            }
            else {
                assistant.GetComponent<ProgressAssistant>().InitiateCapture();
                Debug.Log("capturing " + assistant.scene.name);
            }
        }
    }

    private void LoadSaveData()
    {
        save_data = dataHandler.Load(selectedProfileId); 
    }

    public void SaveSaveData()
    {
        pm_dataHandler.Save(save_data, selectedProfileId);
    }

    public void SavePlayerData(SaveData target_save)
    {
        PlayerData tomatoData = new PlayerData();

        tomatoData.max_health = tomatocontrol.maxHealth;
        tomatoData.current_health = tomatocontrol.currentHealth;
        tomatoData.max_guard = tomatocontrol.maxGuard;
        tomatoData.attack = tomatocontrol.tomatoAtk;
        tomatoData.total_exp = tomatostatus.player_totalExp;
        tomatoData.left_exp = tomatostatus.player_leftExp;
        tomatoData.expBar_max = tomatolevel.expFill.maxValue;
        tomatoData.expBar_current = tomatolevel.expFill.value;

        tomatoData.stat_points = tomatostatus.player_statPt;
        tomatoData.money = tomatostatus.playerMoney;
        tomatoData.level = tomatolevel.playerLevel;

        tomatoData.postion = playerInventory.gameObject.transform.position;

        foreach (Item equip in playerInventory.normalEquip)
        {
            tomatoData.carrying_equip_list.Add(equip.ItemName);
        }
        foreach (Item equip in playerInventory.superEquip)
        {
            tomatoData.carrying_equip_list.Add(equip.ItemName);
        }

        (tomatoData.slot_index_left, tomatoData.slot_index_right, tomatoData.slot_index_super) = inventoryUI.ReturnSlotIndex();

        if (tomatocontrol.tomatoEquip[0])
            tomatoData.equip_left = tomatocontrol.tomatoEquip[0].ItemName;
        if (tomatocontrol.tomatoEquip[1])
            tomatoData.equip_right = tomatocontrol.tomatoEquip[1].ItemName;
        if (tomatocontrol.tomatoSuperEquip)
            tomatoData.equip_super = tomatocontrol.tomatoSuperEquip.ItemName;

        target_save.player_data = tomatoData;
    }

    public void LoadPlayerData()
    {
        PlayerData tomatoData = save_data.player_data;

        tomatocontrol.maxHealth = tomatoData.max_health;
        tomatocontrol.currentHealth = tomatoData.current_health;
        tomatocontrol.maxGuard = tomatoData.max_guard;
        tomatocontrol.tomatoAtk = tomatoData.attack;
        tomatostatus.player_totalExp = tomatoData.total_exp;
        tomatostatus.player_leftExp = tomatoData.left_exp;
        tomatolevel.expFill.maxValue = tomatoData.expBar_max;
        tomatolevel.expFill.value = tomatoData.expBar_current;

        tomatostatus.player_statPt = tomatoData.stat_points;
        tomatostatus.playerMoney = tomatoData.money;
        tomatolevel.playerLevel = tomatoData.level;

        playerInventory.gameObject.transform.position = tomatoData.postion;

        foreach (string equip_name in tomatoData.carrying_equip_list)
        {
            playerInventory.AddItem(EquipDB.ReturnItemOfName(equip_name));
        }

        inventoryUI.RecoverSlotIndex(tomatoData.slot_index_left, tomatoData.slot_index_right, tomatoData.slot_index_super);
        inventoryUI.AddColor_Left(tomatoData.slot_index_left);
        inventoryUI.AddColor_Right(tomatoData.slot_index_right);
        inventoryUI.AddColor_S(tomatoData.slot_index_super);

        if (!string.IsNullOrEmpty(tomatoData.equip_left))
            tomatocontrol.tomatoEquip[0] = (Equip) EquipDB.ReturnItemOfName(tomatoData.equip_left);
        if (!string.IsNullOrEmpty(tomatoData.equip_right))
            tomatocontrol.tomatoEquip[1] = (Equip) EquipDB.ReturnItemOfName(tomatoData.equip_right);
        if (!string.IsNullOrEmpty(tomatoData.equip_super))
            tomatocontrol.tomatoSuperEquip = (SuperEquip) EquipDB.ReturnItemOfName(tomatoData.equip_super);
    }

    public Dictionary<string, SaveData> GetAllProfilesSaveData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public void ChangeSelectedProfileId(string inputProfileId)
    {
        selectedProfileId = inputProfileId;
    }
}

[System.Serializable]
public class SaveData
{
    public PlayerData player_data;
    public StringProgressData progress_dict = new StringProgressData();
    public StringItemLocation CreatedItem_dict = new StringItemLocation();
    public StringItemLocation RemovedItem_dict = new StringItemLocation();
}
