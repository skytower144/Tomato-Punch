using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private List<RectTransform> slotTransforms;
    public List<Sprite> slotSprites;

    private SaveSlot[] saveSlots;

    private int slotNumber;
    private bool isAnimating = false;

    [System.NonSerialized] public bool isLoadMode = false;
    

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    void Start()
    {
        PrepareMenu();
    }

    public void PrepareMenu()
    {
        // Load the dictionary that contains all of the existing profiles
        Dictionary<string, SaveData> profilesSaveDict = ProgressManager.instance.GetAllProfilesSaveData();

        // Loop through each save slot in the UI and set the content appropriately
        
        foreach (SaveSlot slot in saveSlots)
        {
            SaveData profileData = null;
            profilesSaveDict.TryGetValue(slot.profile_id, out profileData);
            slot.SetData(profileData);
        }

        for (int i = 0; i < 3; i++)
        {
            if (slotNumber == i)
                OnFocusSlot(i);
            else
                OffFocusSlot(i);
        }
    }

    private void ResetMenuState()
    {
        isLoadMode = false;

        slotNumber = 0;

        OnFocusSlot(0);
        OffFocusSlot(1);
        OffFocusSlot(2);

        slotTransforms[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
        slotTransforms[1].localScale = new Vector3(1f, 1f, 1f);
        slotTransforms[2].localScale = new Vector3(1f, 1f, 1f);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isAnimating)
        {
            if (playerMovement.Press_Key("Move"))
            {
                Navigate();
            }
            else if (playerMovement.Press_Key("Pause"))
            {
                if (!TitleScreen.isTitleScreen)
                    SimulateEscape();
            }
            else if (playerMovement.Press_Key("Cancel"))
            {
                StartCoroutine(ExitSaveLoadMenu(0.4f));
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                if (isLoadMode) {
                    PrepareLoad();
                }
                else
                    ProceedSave();
            }
        }
    }

    public void SimulateEscape()
    {
        pauseMenu.is_busy = false;
        isAnimating = false;
        
        ResetMenuState();
        playerMovement.HitMenu();
    }

    private void Navigate()
    {
        int prevSlotNumber = slotNumber;

        string direction = playerMovement.Press_Direction();

        if (direction == "UP")
        {
            slotNumber -= 1;
            if (slotNumber < 0)
                slotNumber = 0;
        }
        
        else if (direction == "DOWN")
        {
            slotNumber += 1;
            if (slotNumber > 2)
                slotNumber = 2;
        }

        if (prevSlotNumber != slotNumber)
        {
            NormalizeSlot(prevSlotNumber);
            HighLightSlot(slotNumber);
        }
    }

    private void NormalizeSlot(int number)
    {
        OffFocusSlot(number);
        DOTween.Rewind($"saveslot_Hover_Off_{number}");
        DOTween.Play($"saveslot_Hover_Off_{number}");
    }

    private void HighLightSlot(int number)
    {
        OnFocusSlot(number);
        DOTween.Rewind($"saveslot_Hover_On_{number}");
        DOTween.Play($"saveslot_Hover_On_{number}");
    }

    public void DisableNavigation()
    {
        isAnimating = true;
    }

    public void EnableNavigation()
    {
        isAnimating = false; 
    }

    public void OffFocusSlot(int number)
    {
        if (!ProgressManager.instance.pm_dataHandler.CheckFileExists($"Slot_{number}")) {
            saveSlots[number].GetComponent<Image>().sprite = slotSprites[3];
            saveSlots[number].slotFilter.sprite = slotSprites[5];
        }
        
        else {
            saveSlots[number].GetComponent<Image>().sprite = slotSprites[1];
            saveSlots[number].slotFilter.sprite = slotSprites[4];
        }
        
        saveSlots[number].slotFilter.enabled = true;
    }

    public void OnFocusSlot(int number)
    {
        if (!ProgressManager.instance.pm_dataHandler.CheckFileExists($"Slot_{number}"))
            saveSlots[number].GetComponent<Image>().sprite = slotSprites[2];
        
        else
            saveSlots[number].GetComponent<Image>().sprite = slotSprites[0];
    
        saveSlots[number].slotFilter.enabled = false;
    }

    public IEnumerator ShowSaveLoadMenu(float waitTime)
    {
        pauseMenu.is_busy = true;
        isAnimating = true;

        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));

        gameObject.GetComponent<CanvasGroup>().alpha = 1;

        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");
        
        isAnimating = false;
    }

    public IEnumerator ExitSaveLoadMenu(float waitTime)
    {
        isAnimating = true;

        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));

        ResetMenuState();
        
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");

        isAnimating = false;
        pauseMenu.is_busy = false;

        if (TitleScreen.isTitleScreen) TitleScreen.busy_with_menu = false;
    }

    public void ProceedSave()
    {
        ProgressManager.instance.ChangeSelectedProfileId($"Slot_{slotNumber}");
        ProgressManager.instance.SaveSaveData();

        PrepareMenu();
    }

    private void PrepareLoad()
    {
        if (TitleScreen.isTitleScreen)
            TitleScreen.instance.ResetTitle();
        
        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");
        StartCoroutine(ShowLoadingScreen(0.5f));
    }

    IEnumerator ShowLoadingScreen(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));

        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");
        loadingScreen.SetActive(true);
        pauseMenu.save_load_menu.SimulateEscape();
    }

    private void ProceedLoad(bool startNewGame = false)
    {
        string targetFileName = "_";
        if (!startNewGame)
            targetFileName = $"Slot_{slotNumber}";
        
        ProgressManager.instance.ChangeSelectedProfileId(targetFileName);

        Debug.Log("Covering Gamescreen..."); // Cover GameScreen
        // saveLoadMenu.SimulateEscape();
        
        PlayerMovement.instance.gameObject.SetActive(false);

        if (startNewGame)
            SceneControl.instance.UnloadExceptGameplay();
        ProgressManager.instance.LoadSaveData();
       
        SceneControl.instance.CurrentScene.TriggerScene();
        SceneControl.instance.InvokeRepeating("CheckLoadComplete", 0.1f, 1f);

        PlayerMovement.instance.gameObject.SetActive(true);

        PrepareMenu();
    }
}
