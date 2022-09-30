using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId;
    public string profile_id => profileId;

    [Header("Content")]
    [SerializeField] private GameObject hasData, noData;
    [SerializeField] private TextMeshProUGUI levelText;
    public Image slotFilter;

    public void SetData(SaveData data)
    {
        SaveLoadMenu saveLoadMenu = gameObject.transform.parent.gameObject.GetComponent<SaveLoadMenu>();

        // There's no data for this profileId
        if (data == null)
        {   
            hasData.SetActive(false);
            noData.SetActive(true);
        }
        else
        {
            levelText.text = "Lv. " + data.player_data.level;

            hasData.SetActive(true);
            noData.SetActive(false);
        }
    }
}
