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
    [SerializeField] private Transform essential_transform;
    [SerializeField] private GameObject itemTotal, itemTotalPrefab;
    public GameObject item_total => itemTotal;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private string selectedProfileId = "Slot_New";
    public string selected_profile_id => selectedProfileId;

    private FileDataHandler dataHandler;
    public FileDataHandler pm_dataHandler => dataHandler;
    [System.NonSerialized] public SaveData save_data = new SaveData();
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Progress Manager in scene.");
        }
        instance = this;

        CountableItemDB.Initiatlize();
        EquipDB.Initiatlize();
        ItemPrefabDB.Initiatlize();
        
        // Application.persistentDataPath will give the OS standard directory for persisting data in a Unity project.
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
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
                    GameManager.DoDebug("capturing " + assistant.scene.name);
                    return;
                }
            }
            else {
                assistant.GetComponent<ProgressAssistant>().InitiateCapture();
                GameManager.DoDebug("capturing " + assistant.scene.name);
            }
        }
    }
    public void SaveSaveData()
    {
        CaptureScene();
        dataHandler.Save(save_data, selectedProfileId);
    }
    public void RemoveAllSaveData()
    {
        for (int i = 0; i < 4; i++)
        {
            DeleteSaveData($"Slot_{i}");
        }
    }
    private void BackupCleanSlot()
    {
        save_data = new SaveData(); // Make sure save_data is a clean save
        dataHandler.Save(save_data, "Slot_New");
    }

    public void LoadSaveData(bool startNewGame = false)
    {
        if (!this.dataHandler.CheckFileExists("Slot_New")) // If this is the first ever load
        {
            BackupCleanSlot();
        }

        if (startNewGame)
        {
            RemoveAllSaveData();
            save_data = dataHandler.Load("Slot_New");
        }
        else
        {
            if (dataHandler.CheckFileExists(selected_profile_id))
                save_data = dataHandler.Load(selectedProfileId);
            else
                save_data = dataHandler.Load("Slot_New");
        }
        Destroy(itemTotal);
        itemTotal = Instantiate(itemTotalPrefab, essential_transform);
        GameManager.gm_instance.itemManager = itemTotal.GetComponent<ItemManager>();
        GameManager.gm_instance.itemManager.RecoverItemState();

        playerInventory.GatherEquipSlots();
        LoadPlayerData(); // apply data to gameplay
    }

    public void DeleteSaveData(string profileId)
    {
        dataHandler.Delete(profileId);
    }

    public void SavePlayerData()
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

        if (SceneControl.instance.CurrentScene)
            tomatoData.current_scene = SceneControl.instance.CurrentScene.GetSceneName();
        tomatoData.postion = playerInventory.gameObject.transform.position;

        tomatoData.isCameraOff = PlayerCamera.playerCamera_instance.isCameraOff;
        tomatoData.uiCanvas_position = PlayerCamera.playerCamera_instance.uiCanvas_transform.localPosition;
        tomatoData.backup_canvas_x = PlayerCamera.playerCamera_instance.canvas_x;
        tomatoData.backup_canvas_y = PlayerCamera.playerCamera_instance.canvas_y;

        foreach (ItemQuantity consumable in playerInventory.consumableItems)
        {
            tomatoData.carrying_countable_list.Add(new SerializedItemQuantity(consumable.item.ItemName, consumable.count));
        }
        foreach (ItemQuantity other in playerInventory.otherItems)
        {
            tomatoData.carrying_countable_list.Add(new SerializedItemQuantity(other.item.ItemName, other.count));
        }
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

        tomatoData.assignedQuests = QuestManager.instance.ReturnQuestState(true);
        tomatoData.unassignedQuests = QuestManager.instance.ReturnQuestState(false);

        tomatoData.keyEventList = GameManager.gm_instance.playerKeyEventManager.ReturnPlayerKeyEvents();
        tomatoData.partyMembers = GameManager.gm_instance.partyManager.ReturnPartyMembers();

        save_data.player_data = tomatoData;
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

        SceneControl.instance.SetCurrentScene(SceneControl.instance.sceneDict[tomatoData.current_scene], true);
        playerInventory.gameObject.transform.position = tomatoData.postion;

        PlayerCamera.playerCamera_instance.isCameraOff = tomatoData.isCameraOff;
        PlayerCamera.playerCamera_instance.uiCanvas_vector = tomatoData.uiCanvas_position;
        PlayerCamera.playerCamera_instance.canvas_x = tomatoData.backup_canvas_x;
        PlayerCamera.playerCamera_instance.canvas_y = tomatoData.backup_canvas_y;
        
        playerInventory.consumableItems.Clear();
        playerInventory.otherItems.Clear();
        playerInventory.normalEquip.Clear();
        playerInventory.superEquip.Clear();
        inventoryUI.ClearAllEquipSlots();

        foreach (var consumable in tomatoData.carrying_countable_list)
        {
            playerInventory.AddItem(CountableItemDB.ReturnItemOfName(consumable.item_name), consumable.item_count);
        }
        foreach (string equip_name in tomatoData.carrying_equip_list)
        {
            playerInventory.AddItem(EquipDB.ReturnItemOfName(equip_name));
        }
        inventoryUI.UpdateConsumableSlots();
        inventoryUI.UpdateOtherItemSlots();
        inventoryUI.RecoverSlotIndex(tomatoData.slot_index_left, tomatoData.slot_index_right, tomatoData.slot_index_super);
        inventoryUI.UpdateEquipSlots(tomatoData.slot_index_left, tomatoData.slot_index_right, tomatoData.slot_index_super);

        QuestManager.instance.UpdateQuestState(tomatoData.assignedQuests, tomatoData.unassignedQuests);
        GameManager.gm_instance.playerKeyEventManager.RestorePlayerKeyEvents(tomatoData.keyEventList);
        GameManager.gm_instance.partyManager.RestorePartyMembers(tomatoData.partyMembers);
    }

    public Dictionary<string, SaveData> GetAllProfilesSaveData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public void ChangeSelectedProfileId(string inputProfileId)
    {
        selectedProfileId = inputProfileId;
    }

    public bool CheckAnySaveExists()
    {
        for (int i = 0; i <= 3; i++)
        {
            if (dataHandler.CheckFileExists($"Slot_{i}"))
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class SaveData
{
    public PlayerData player_data = new PlayerData();
    public StringProgressData progress_dict = new StringProgressData();
    public StringItemLocation CreatedItem_dict = new StringItemLocation();
    public StringItemLocation RemovedItem_dict = new StringItemLocation();
}
