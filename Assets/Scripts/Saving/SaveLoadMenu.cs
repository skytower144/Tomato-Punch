using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private List<RectTransform> slotTransforms;
    public List<Sprite> slotSprites;
    private SaveSlot[] saveSlots;

    private int slotNumber = 0;
    private bool isAnimating = false;

    [System.NonSerialized] public bool isLoadMode = false;
    [System.NonSerialized] public bool isLoading = false;
    private bool isSaving = false;

    // Prompt Elements
    [SerializeField] private GameObject promptBox;
    [SerializeField] private List <TextMeshProUGUI> choiceTextList;
    [SerializeField] private List <Image> choiceFrameList;
    [SerializeField] private Color32 choiceHighlight_frame, choiceHighlight_text, choiceDefault;
    private int choiceNumber = 0;
    private bool isPrompt = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    void Start()
    {
        PrepareMenu();
    }
    void Update()
    {
        if (!isAnimating)
        {
            if (playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                GameManager.gm_instance.DetectHolding(Navigate);
            }
            else if (GameManager.gm_instance.WasHolding)
            {
                GameManager.gm_instance.holdStartTime = float.MaxValue;
            }
            else if (playerMovement.Press_Key("Cancel"))
            {
                if (!isPrompt)
                    StartCoroutine(ExitSaveLoadMenu(0.4f));
                else
                    ClosePrompt();
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                if (isLoadMode) {
                    if (!isPrompt)
                    {
                        if (TitleScreen.isTitleScreen)
                            StartCoroutine(PrepareLoad());
                        else
                            PromptConfirm();
                    }
                    else if (isPrompt)
                    {
                        if (choiceNumber == 0)
                            StartCoroutine(PrepareLoad());
                        ClosePrompt();
                    }
                }
                else if (!isLoading && !isSaving) {
                    isSaving = true;
                    ProceedSave();
                }
            }
            else if (!isPrompt && !TitleScreen.isTitleScreen)
            {
                if (playerMovement.Press_Key("Pause"))
                    SimulateEscape();   
            }
        }
    }

    private void PromptConfirm()
    {
        choiceNumber = 0;
        choiceTextList[0].color = choiceHighlight_text;
        choiceFrameList[0].color = choiceHighlight_frame;
        choiceTextList[1].color = choiceDefault;
        choiceFrameList[1].color = choiceDefault;
        
        isPrompt = true;
        promptBox.SetActive(true);

        DOTween.Rewind("prompt_Load");
        DOTween.Play("prompt_Load");
    }
    private void ClosePrompt()
    {
        promptBox.SetActive(false);
        isPrompt = false;
    }

    public void PrepareMenu() // After saving or After loading complete.
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
        slotNumber = 0;

        OnFocusSlot(0);
        OffFocusSlot(1);
        OffFocusSlot(2);

        slotTransforms[0].localScale = new Vector3(1.1f, 1.1f, 1.1f);
        slotTransforms[1].localScale = new Vector3(1f, 1f, 1f);
        slotTransforms[2].localScale = new Vector3(1f, 1f, 1f);

        gameObject.GetComponent<CanvasGroup>().alpha = 1;
    
        gameObject.SetActive(false);
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
        string direction = playerMovement.Press_Direction();

        if (!isPrompt)
        {
            int prevSlotNumber = slotNumber;

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
        else if (isPrompt)
        {
            choiceTextList[choiceNumber].color = choiceDefault;
            choiceFrameList[choiceNumber].color = choiceDefault;
            if (direction == "LEFT")
            {
                choiceNumber = 0;
            }
            else if (direction == "RIGHT")
            {
                choiceNumber = 1;
            }
            choiceTextList[choiceNumber].color = choiceHighlight_text;
            choiceFrameList[choiceNumber].color = choiceHighlight_frame;
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
        
        isSaving = false;
    }

    public IEnumerator PrepareLoad(bool startNewGame = false)
    {
        isLoadMode = false;
        isLoading = true;

        Debug.Log("Covering Gamescreen.");
        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");

        ProgressManager.instance.item_total.SetActive(false);

        yield return new WaitForSecondsRealtime(0.5f);

        if (TitleScreen.isTitleScreen)
            EssentialLoader.instance.RestorePortablePosition(ProceedLoad_1, startNewGame); // Normalize hierarchy
        else
            ProceedLoad_1(startNewGame);
    }

    public void ProceedLoad_1(bool startNewGame = false)
    {
        TitleScreen.instance.ResetTitle(); // Reset PauseMenu and Filter to its normal state.
        LoadingScreen.instance.EnableLoadingScreen();

        string targetFileName = "Slot_New";
        if (!startNewGame)
            targetFileName = $"Slot_{slotNumber}";
        ProgressManager.instance.ChangeSelectedProfileId(targetFileName);
        
        PlayerMovement.instance.collider_obj.SetActive(false);

        SceneControl.instance.UnloadExceptGameplay(SceneControl.instance.ScenesExceptGameplay(), ProceedLoad_2, startNewGame);
        SimulateEscape();
    }
    public void ProceedLoad_2(bool startNewGame = false)
    {
        ProgressManager.instance.LoadSaveData(startNewGame);
       
        SceneControl.instance.CurrentScene.TriggerScene();
        
        SceneControl.instance.InvokeRepeating("CheckLoadComplete", 0.1f, 1f);

        // PlayerMovement.instance.collider_obj.SetActive(true); --> SceneControl - CheckLoadComplete
        // PrepareMenu(); --> ""
    }
}
