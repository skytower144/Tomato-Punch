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


    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private FileDataHandler dataHandler;
    public FileDataHandler pm_dataHandler => dataHandler;

    public StringProgressData progress_dict = new StringProgressData();
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than on Progress Manager in scene.");
        }
        instance = this;

        // Application.persistentDataPath will give the OS standard directory for persisting data in a Unity project.
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        
        progress_dict = dataHandler.Load();
        if (!progress_dict.ContainsKey("Gameplay_PlayerData"))
            SavePlayerData();
        LoadPlayerData();
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
                    return;
                }
            }
            else {
                Debug.Log("capturing " + assistant.scene.name);
                assistant.GetComponent<ProgressAssistant>().InitiateCapture();
            }
        }
    }

    public void SavePlayerData()
    {
        string tomatoID = "Gameplay_PlayerData";
        ProgressData tomatoData = new ProgressData();

        tomatoData.float_list.Add(tomatocontrol.maxHealth);
        tomatoData.float_list.Add(tomatocontrol.currentHealth);
        tomatoData.float_list.Add(tomatocontrol.maxGuard);
        tomatoData.float_list.Add(tomatocontrol.tomatoAtk);
        tomatoData.float_list.Add(tomatostatus.player_totalExp);
        tomatoData.float_list.Add(tomatostatus.player_leftExp);
        tomatoData.float_list.Add(tomatolevel.expFill.maxValue);
        tomatoData.float_list.Add(tomatolevel.expFill.value);

        tomatoData.int_list.Add(tomatostatus.player_statPt);
        tomatoData.int_list.Add(tomatostatus.playerMoney);
        tomatoData.int_list.Add(tomatolevel.playerLevel);

        if (tomatocontrol.tomatoEquip[0])
            tomatoData.string_0 = tomatocontrol.tomatoEquip[0].ItemName;
        if (tomatocontrol.tomatoEquip[1])
            tomatoData.string_1 = tomatocontrol.tomatoEquip[1].ItemName;
        if (tomatocontrol.tomatoSuperEquip)
            tomatoData.string_2 = tomatocontrol.tomatoSuperEquip.ItemName;

        tomatoData.postion = playerInventory.gameObject.transform.position;

        progress_dict[tomatoID] = tomatoData;
    }

    public void LoadPlayerData()
    {
        string tomatoID = "Gameplay_PlayerData";
        ProgressData tomatoData = progress_dict[tomatoID];

        tomatocontrol.maxHealth = tomatoData.float_list[0];
        tomatocontrol.currentHealth = tomatoData.float_list[1];
        tomatocontrol.maxGuard = tomatoData.float_list[2];
        tomatocontrol.tomatoAtk = tomatoData.float_list[3];
        tomatostatus.player_totalExp = tomatoData.float_list[4];
        tomatostatus.player_leftExp = tomatoData.float_list[5];
        tomatolevel.expFill.maxValue = tomatoData.float_list[6];
        tomatolevel.expFill.value = tomatoData.float_list[7];

        tomatostatus.player_statPt = tomatoData.int_list[0];
        tomatostatus.playerMoney = tomatoData.int_list[1];
        tomatolevel.playerLevel = tomatoData.int_list[2];

        //tomatocontrol.tomatoEquip[0].ItemName = tomatoData.string_0;
        //tomatocontrol.tomatoEquip[1].ItemName = tomatoData.string_1;
        //tomatocontrol.tomatoSuperEquip.ItemName = tomatoData.string_2;

        playerInventory.gameObject.transform.position = tomatoData.postion;
    }
}
