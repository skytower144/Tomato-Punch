using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;

public enum ChoiceType { OXChoice, ShopChoice }

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    public CutsceneHandler cutsceneHandler;
    public Canvas uiCanvas;

    private PlayerMovement playerMovement;
    private ChoiceType choiceType = ChoiceType.OXChoice;
    
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject portraitBox;

    [SerializeField] private Image portrait, dialogueBoxImage, cursorImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypeEffect dialogue_typeEffect;
    [SerializeField] private RectTransform dialogue_rect, dialogueBox_rect, cursor_rect;

    [Header("Choice UI")]
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private RectTransform choiceBoxPTransform;
    [SerializeField] private List<TextMeshProUGUI> choiceText;

    [System.NonSerialized] public NPCController ContinueTalkTarget;
    public NPCController current_npc => currentNpc;
    public bool is_continue_talk => isContinueTalk;

    private NPCController currentNpc;
    private Story currentStory; 
    private string currentSentence;
    private string[] splitTag;
    private string[] animInfo;

    private Interactable currentTarget;
    private DialogueExit dialogueExit = DialogueExit.Nothing;

    private bool dialogueIsPlaying, isPromptChoice, isContinueTalk, hideDialogue;

    private string uiCanvas_layerName;
    private int uiCanvas_order;
    private float dialogueBox_x, dialogueBox_y, cursor_x, cursor_y;
    private Color32 dialogueText_color, cursor_color;

    private const string PORTRAIT_TAG = "portrait";
    private const string HIDEPORTRAIT_TAG = "hideportrait";
    private const string DIALOGUE_TAG = "nextdialogue";
    private const string JUDGEKEYEVENT_TAG = "judgekeyevent";
    private const string ADDKEYEVENT_TAG = "addkeyevent";
    private const string ROLLBACKDIALOGUE_TAG = "rollbackdialogue";
    private const string CONTINUETALK_TAG = "continuetalk";
    private const string TURNPLAYER_TAG = "turnplayer";
    private const string ANIMATE_TAG = "animate";
    private const string FOCUSANIMATE_TAG = "focusanimate";
    private const string CHANGEIDLE_TAG = "changeidle";
    private const string BATTLE_TAG = "battle";
    private const string BATTLETARGET_TAG = "battletarget";
    private const string PURCHASE_TAG = "purchase";
    private const string CHECKPLAYERMONEY_TAG = "checkplayermoney";
    private const string EARNMONEY_TAG = "earnmoney";
    private const string SETPLAYERHEALTH_TAG = "setplayerhp";
    private const string MOVECHOICEBOX_TAG = "movechoicebox";
    private const string RESETCHOICEBOX_TAG = "resetchoicebox";
    private const string MOVEDIALOGUEBOX_TAG = "movedialoguebox";
    private const string MOVECURSOR_TAG = "movecursor";
    private const string DIALOGUEBOXALPHA_TAG = "setboxalpha";
    private const string DIALOGUEBOXSIZE_TAG = "setboxsize";
    private const string RESETDIALOGUEBOX_TAG = "resetdialoguebox";
    private const string UICANVASLAYER_TAG = "uicanvaslayer";
    private const string TEXTCOLOR_TAG = "textcolor";
    private const string VIEWSHOP_TAG = "viewshop";
    private const string CALCULATESHOP_TAG = "calculateshop";
    private const string PAYSHOP_TAG = "payshop";
    private const string CONTINUESHOPPING_TAG = "continueshopping";
    private const string HIDENPC_TAG = "hidenpc";
    private const string HASQUEST_TAG = "hasquest";
    private const string GIVEQUEST_TAG = "givequest";
    private const string CHECKQUEST_TAG = "checkquest";
    private const string COMPLETEQUEST_TAG = "completequest";
    private const string CHECKPARTY_TAG = "checkparty";
    private const string REMOVEITEM_TAG = "removeitem";
    private const string TELEPORT_TAG = "teleport";
    private const string FLIP_TAG = "flip";
    private const string SETACTIVE_TAG = "setactive";
    private const string UNLOCKPORTAL_TAG = "unlockportal";
    private const string JOINPARTY_TAG = "joinparty";
    private const string LEAVEPARTY_TAG = "leaveparty";
    public string BattleTag => BATTLE_TAG;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene.");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        isPromptChoice = false;

        dialogueBox.SetActive(false);
        SaveDialogueBoxState();

        playerMovement = PlayerMovement.instance;
    }

    void Update()
    {
        if (!dialogueIsPlaying || hideDialogue)
        {
            return;
        }
        else if (playerMovement.Press_Key("Interact") && !isPromptChoice)
        {
            if (dialogue_typeEffect.isPrinting)
                dialogue_typeEffect.SetMessage(currentSentence);
            else
                StartCoroutine(ContinueStory());
        }
        else if (dialogueIsPlaying && (currentSentence == ""))
            StartCoroutine(ContinueStory());
    }

    public void EnterDialogue(TextAsset inkJSON, Interactable interactingTarget = null, bool autoContinue = false)
    {
        if (inkJSON == null) return;

        playerMovement.SetIsInteracting(true);

        currentNpc = (interactingTarget is NPCController) ? ((NPCController)interactingTarget) : null;
        currentTarget = interactingTarget;

        GameManager.gm_instance.partyManager.SetMemberFollow(false);
        GameManager.gm_instance.save_load_menu.DetermineAutoSave(inkJSON.text);
        
        currentStory = new Story(inkJSON.text);
        portrait.sprite = SpriteDB.ReturnPortrait("Tomato_neutral");

        SetBoldness();
        SetDialogueBox(true);
        dialogueIsPlaying = true;

        if (autoContinue)
            StartCoroutine(ContinueStory());
    }

    public void ExitDialogue()
    {
        dialogueIsPlaying = false;
        SetDialogueBox(false);
        SetPortraitBox(false);
        dialogueText.text = "";
        currentSentence = "";

        ResetDialogueBoxState();
        playerMovement.SetIsInteracting(false);
        GameManager.gm_instance.partyManager.SetMemberFollow(true);
        GameManager.gm_instance.save_load_menu.DetermineAutoSave();

        InvokeEvent();
    }
    private void InvokeEvent()
    {
        switch (dialogueExit)
        {
            case DialogueExit.Battle:
                GameManager.gm_instance.battle_system.StartBattle(
                    GameManager.gm_instance.battle_system.enemy_control._base, 
                    currentNpc.gameObject.GetComponent<CustomBattleMode>()
                );
                break;
            
            case DialogueExit.UnlockDoor:
                ((LocationPortal)currentTarget).EnableDoor();
                break;
            
            default:
                break;
        }
        currentTarget = null;
        dialogueExit = DialogueExit.Nothing;
    }

    private IEnumerator ContinueStory()
    {
        if (currentStory.canContinue){
            currentSentence = currentStory.Continue().Trim();
            currentSentence = TextDB.Translate(currentSentence, TranslationType.DIALOGUE);
            
            if (currentSentence == "/cut") {
                SetDialogue(false);
                yield return cutsceneHandler.HandleCutsceneTags(currentStory.currentTags);
                ShowAndContinueDialogue();
                yield break;
            }
            else
                HandleTags(currentStory.currentTags);
            
            if (!hideDialogue)
                DisplayDialogue();
        }
        else
            ExitDialogue();
        yield break;
    }

    private bool CheckChoiceSentence()
    {
        if (currentStory.currentChoices.Count != 2)
            return false;
        return true;
        
    }
    private void DisplayChoices()
    {
        if (!CheckChoiceSentence())
            return;
        
        List<Choice> currentChoices = currentStory.currentChoices;
        isPromptChoice = true;
        
        if (choiceType == ChoiceType.OXChoice)
        {
            int index = 0;
            foreach (Choice choice in currentChoices)
            {
                string tag = choice.text.ToUpper();
                string fontTag;

                if (tag == "YES") {
                    tag = "ConfirmPrompt_Yes";
                    fontTag = "Choicebox_Yes";
                }
                else {
                    tag = "ConfirmPrompt_No";
                    fontTag = "Choicebox_No";
                }
                choiceText[index].text = TextDB.Translate(tag, TranslationType.UI);
                UIControl.instance.SetFontData(choiceText[index], fontTag);
                index++; 
            }
            choiceBox.GetComponent<ChoiceSelect>().proceedAction = MakeChoice;
            choiceBox.SetActive(true);
        }
        else if (choiceType == ChoiceType.ShopChoice)
        {
            // Repeated visit.
            if (UIControl.instance.ui_shop.shopInteraction == ShopInteraction.ContinueShopping)
                UIControl.instance.ui_shop.ContinueShopping();
            
            else { // If it's the first time viewing the shop.
                try {
                    currentNpc.gameObject.GetComponent<ShopInventory>().ViewShopUI(MakeChoice);
                }
                catch (Exception e) {
                    Debug.LogError($"Could not get ShopInventory component from {currentNpc.name}.\n{e}");
                }
            }
        }
    }

    private void MakeChoice()
    {
        int choiceNumber = 0;

        if (choiceType == ChoiceType.OXChoice) {
            choiceNumber = choiceBox.GetComponent<ChoiceSelect>().choice_number;
            choiceBox.SetActive(false);
        }
        else if (choiceType == ChoiceType.ShopChoice) {
            ShopInteraction player_selection = ShopSystem.instance.shopInteraction;
            choiceNumber = (int) player_selection;
            
            NormalizeChoiceType();

            if (player_selection == ShopInteraction.ExitShop)
                ShopSystem.instance.gameObject.SetActive(false);
        }

        currentStory.ChooseChoiceIndex(choiceNumber);
        StartCoroutine(ContinueStory());
        isPromptChoice = false;
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            splitTag = tag.Split(':');
            if (splitTag.Length != 2)
                continue;
            
            string tag_key = splitTag[0].Trim();
            string tag_value = splitTag[1].Trim();

            switch (tag_key)
            {
                case PORTRAIT_TAG: // #portrait:Tomato_neutral
                    SetPortraitBox(true);
                    portrait.sprite = SpriteDB.ReturnPortrait(tag_value);
                    break;
                
                case HIDEPORTRAIT_TAG: // #hideportrait:_
                    SetPortraitBox(false);
                    break;

                case DIALOGUE_TAG: // #nextdialogue:policeman_fainted
                    currentNpc.LoadNextDialogue(tag_value);
                    break;
                
                case JUDGEKEYEVENT_TAG: // #judgekeyevent:Win_Rupple_StartingPoint@Lose_Rupple_StartingPoint // #judgekeyevent:Win_Rupple_StartingPoint
                    GameManager.gm_instance.playerKeyEventManager.CacheKeyEvents(tag_value);
                    break;
                
                case ADDKEYEVENT_TAG: // #addkeyevent:Find_BabyCat_StartingPoint // #addkeyevent:Find_BabyCat_StartingPoint@true // #addkeyevent:eventname@applynow
                    string[] eventInfo = tag_value.Split('@');
                    GameManager.gm_instance.playerKeyEventManager.AddKeyEvent(eventInfo[0]);

                    if (eventInfo.Length == 2 && eventInfo[1] == "true")
                        GameManager.gm_instance.playerKeyEventManager.ApplyGlobalKeyEvent(eventInfo[0]);
                    break;
                
                case ROLLBACKDIALOGUE_TAG: // #rollbackdialogue:_
                    currentNpc.RollbackDialogue();
                    break;

                case CONTINUETALK_TAG: // #continuetalk:_
                    ContinueTalkTarget = (tag_value[0] != '_') ? NPCManager.instance.npc_dict[tag_value] : null;
                    isContinueTalk = true;
                    break;
                
                case TURNPLAYER_TAG: //#turnplayer:LEFT@1.7// #turnplayer:LEFT
                    string[] directionInfo = CheckTagValueError(tag_value);
                    float delay = 0f;

                    if (directionInfo.Length == 2)
                        delay = float.Parse(directionInfo[1]);
                    
                    StartCoroutine(PlayerMovement.instance.DelayFaceAdjustment(directionInfo[0], delay));
                    break;

                case ANIMATE_TAG: // #animate:fainted // #animate:fainted-fainted // #animate:clipname-newidlename
                    animInfo = tag_value.Split('-');
                    if (animInfo.Length == 2) currentNpc.ChangeIdleAnimation(animInfo[1]);
                    
                    currentNpc.Play(animInfo[0]);
                    break;
                
                case FOCUSANIMATE_TAG: // #focusanimate:StartingPoint_Donut@angry // #focusanimate:StartingPoint_Donut@angry-isangry // #focusanimate:this@drop@stop
                    bool stopAnimation = false;
                    animInfo = tag_value.Split('@');
                    NPCController npc = ((animInfo[0] == "_") || (animInfo[0] == "this")) ? currentNpc : NPCManager.instance.npc_dict[animInfo[0]];

                    if (animInfo.Length > 2)
                        stopAnimation = true;
                    
                    animInfo = animInfo[1].Split('-');
                    if (animInfo.Length == 2) npc.ChangeIdleAnimation(animInfo[1]);

                    SetDialogue(false);
                    npc.Play(animInfo[0], ShowAndContinueDialogue, stopAnimation);
                    break;
                
                case CHANGEIDLE_TAG: // #changeidle:StartingPoint_Donut@isangry
                    string[] info0 = CheckTagValueError(tag_value);

                    NPCController npc0 = NPCManager.instance.npc_dict[info0[0]];
                    npc0.ChangeIdleAnimation(info0[1]);
                    break;

                case BATTLE_TAG:
                    dialogueExit = DialogueExit.Battle;
                    GameManager.gm_instance.battle_system.enemy_control.LoadEnemyBaseData(currentNpc.enemyData);
                    break;

                case BATTLETARGET_TAG: // #battletarget:StartingPoint_Donut
                    dialogueExit = DialogueExit.Battle;
                    GameManager.gm_instance.battle_system.enemy_control.LoadEnemyBaseData(NPCManager.instance.npc_dict[tag_value].enemyData);
                    break;

                case PURCHASE_TAG: // #purchase:Milk Bottle@0 // #purchase:Donut@0@3 // ItemName@price@_amount
                    GameManager.gm_instance.ui_control.ui_shop.PurchaseItemByTag(tag_value);
                    break;

                case CHECKPLAYERMONEY_TAG:
                    int amount = int.Parse(tag_value);
                    if (!GameManager.gm_instance.battle_system.tomatostatus.CheckEnoughMoney(amount))
                        currentStory.variablesState["enoughMoney"] = false;
                    break;
                
                case EARNMONEY_TAG: //#earnmoney:2
                    int earnedMoney = int.Parse(tag_value);
                    GameManager.gm_instance.battle_system.tomatostatus.UpdatePlayerMoney(earnedMoney);
                    break;
                
                case SETPLAYERHEALTH_TAG: // #setplayerhp:10 // #setplayerhp:10@percent
                    string[] healthInfo = tag_value.Split('@');

                    if (healthInfo.Length >= 1)
                        GameManager.gm_instance.TomatoStatus.SetPlayerHealth(float.Parse(healthInfo[0]));

                    else if (healthInfo.Length >= 2 && healthInfo[1] == "percent")
                        GameManager.gm_instance.TomatoStatus.SetPlayerHealthPercentage(int.Parse(healthInfo[0]));
                    break;
                
                case MOVECHOICEBOX_TAG:
                    choiceBoxPTransform.localPosition = new Vector2(725, -103.5f);
                    break;
                
                case RESETCHOICEBOX_TAG:
                    choiceBoxPTransform.localPosition = new Vector2(498, 166);
                    break;
                
                case MOVEDIALOGUEBOX_TAG: // #movedialoguebox:x@y
                    string[] boxPosInfo = CheckTagValueError(tag_value);
                    float boxX = float.Parse(boxPosInfo[0]);
                    float boxY = float.Parse(boxPosInfo[1]);
                    dialogueBox_rect.localPosition = new Vector2(boxX, boxY);
                    break;

                case MOVECURSOR_TAG: // #movecursor@x@y
                    string[] cursorInfo = CheckTagValueError(tag_value);
                    float cursorX = float.Parse(cursorInfo[0]);
                    float cursorY = float.Parse(cursorInfo[1]);
                    cursor_rect.localPosition = new Vector2(cursorX, cursorY);
                    break;

                case DIALOGUEBOXALPHA_TAG: // #setboxalpha:0.5
                    float boxAlpha = float.Parse(tag_value);
                    Color boxColor = dialogueBoxImage.color;
                    boxColor.a = boxAlpha;
                    dialogueBoxImage.color = boxColor;
                    break;

                case DIALOGUEBOXSIZE_TAG: // #setboxsize:1.2
                    float boxSize = float.Parse(tag_value);
                    dialogueBox_rect.localScale = new Vector3(boxSize, boxSize, 1f);
                    break;

                case RESETDIALOGUEBOX_TAG: // #resetdialoguebox:_
                    ResetDialogueBoxState();
                    break;

                case UICANVASLAYER_TAG: // #uicanvaslayer:name@layerOrder
                    string[] layerInfo = CheckTagValueError(tag_value);
                    uiCanvas.sortingLayerName = layerInfo[0];
                    uiCanvas.sortingOrder = int.Parse(layerInfo[1]);
                    break;

                case TEXTCOLOR_TAG: // #textcolor:white
                    Color32 changedColor = new Color32();
                    if (tag_value == "white")
                        changedColor = new Color32(255, 255, 255, 255);

                    dialogueText.color = cursorImage.color = changedColor;
                    break;

                case VIEWSHOP_TAG:
                    choiceType = ChoiceType.ShopChoice;
                    break;

                case CALCULATESHOP_TAG:
                    currentSentence = currentSentence.Replace("[?]", $"{GameManager.gm_instance.ui_control.ui_shop.total_price}");
                    break;

                case PAYSHOP_TAG:
                    ShopSystem.instance.ProceedPayment();
                    break;
                
                case CONTINUESHOPPING_TAG:
                    ShopSystem.instance.shopInteraction = ShopInteraction.ContinueShopping;
                    break;
                
                case HASQUEST_TAG:
                    var tempQuest0 = QuestManager.instance.FindQuest(tag_value, QuestList.Assigned);
                    currentStory.variablesState["isQuestActive"] = (tempQuest0 != null);
                    break;

                case GIVEQUEST_TAG:
                    QuestManager.instance.AddQuest(tag_value);
                    break;
                
                case CHECKQUEST_TAG:
                    var tempQuest2 = QuestManager.instance.FindQuest(tag_value, QuestList.Assigned);
                    currentStory.variablesState["isQuestCompleted"] = tempQuest2?.CheckQuestComplete(); 
                    break;

                case COMPLETEQUEST_TAG:
                    var tempQuest3 = QuestManager.instance.FindQuest(tag_value, QuestList.Assigned);
                    tempQuest3?.GiveReward();
                    break;
                
                case CHECKPARTY_TAG: // #checkparty:npcid
                    currentStory.variablesState["hasMember"] = GameManager.gm_instance.partyManager.HasMember(tag_value);
                    break;

                case REMOVEITEM_TAG: // #removeitem:Donut
                    Item targetItem = Item.ReturnMatchingItem(tag_value);
                    Inventory.instance.RemoveItem(targetItem);
                    break;
                
                case TELEPORT_TAG: // #teleport:StartingPoint_Donut@x@y #teleport:player@x@y
                    string[] posInfo = CheckTagValueError(tag_value);
                    float x = float.Parse(posInfo[1]);
                    float y = float.Parse(posInfo[2]);

                    if (posInfo[0].ToLower() == "player")
                        playerMovement.Teleport(x, y);
                    else {
                        NPCController npc2 = NPCManager.instance.npc_dict[posInfo[0]];
                        npc2.Teleport(x, y);
                    }
                    break;
                
                case FLIP_TAG: // #flip:Company_SecondFloor_worker_A
                    NPCController npc3 = NPCManager.instance.npc_dict[tag_value];
                    Vector3 npcScale = npc3.transform.localScale;
                    npc3.transform.localScale = new Vector3(npcScale.x * -1, npcScale.y, npcScale.z);
                    break;
                
                case HIDENPC_TAG: // #hidenpc:_
                    currentNpc.gameObject.SetActive(false);
                    break;
                
                case SETACTIVE_TAG: // #setactive:StartingPoint_Donut@true
                    string[] info = CheckTagValueError(tag_value);

                    NPCController npc4 = NPCManager.instance.npc_dict[info[0]];
                    npc4.gameObject.SetActive(info[1] == "true");
                    break;
                
                case UNLOCKPORTAL_TAG: // #unlockportal:_
                    dialogueExit = DialogueExit.UnlockDoor;
                    break;

                case JOINPARTY_TAG: // #joinparty:_
                    GameManager.gm_instance.partyManager.JoinParty(currentNpc);
                    break;
                
                case LEAVEPARTY_TAG: // #leaveparty:_
                    string leavingMemberName = (tag_value == "_") ? currentNpc.ReturnID() : tag_value;
                    GameManager.gm_instance.partyManager.LeaveParty(leavingMemberName);
                    break;
                
                default:
                    GameManager.DoDebug("Tag detected but not handled." + tag);
                    break;
            }
        }
    }

    private void DisplayDialogue()
    {
        if (string.IsNullOrEmpty(currentSentence) || currentSentence == "/cut" || currentSentence == "/ignore")
            StartCoroutine(ContinueStory());
        
        else {
            if (CheckChoiceSentence()) {
                dialogue_typeEffect.proceed_action = DisplayChoices;
            }
            dialogue_typeEffect.SetMessage(currentSentence);
        }
    }

    public void ShowAndContinueDialogue()
    {
        SetDialogue(true);
        DisplayDialogue();
    }

    public IEnumerator DialogueAction(Animator anim, Action dialogueAction, float delay, bool stopAfterAnimation)
    {
        yield return new WaitForSecondsRealtime(delay);
        dialogueAction.Invoke();
        if (!stopAfterAnimation)
            anim.Play(CutsceneHandler.GetBaseLayerEntryAnimationTag(anim));
    }

    private void SetDialogue(bool state)
    {
        hideDialogue = !state;
        SetDialogueBox(state);
    }

    public void SetPortraitBox(bool hasPortrait)
    {
        if (hasPortrait)
        {
            dialogue_rect.localPosition = new Vector3(130.77f, -98.91992f);
            dialogue_rect.sizeDelta = new Vector2(981.85f, 221.45f);
            portraitBox.SetActive(true);
        }
        else
        {
            dialogue_rect.localPosition = new Vector3(4.17f, -98.91992f);
            dialogue_rect.sizeDelta = new Vector2(1235.06f, 221.45f);
            portraitBox.SetActive(false);
        }
    }

    public void SetDialogueBox(bool state)
    {
        AlignUiWithCamera(state);
        dialogueBox.SetActive(state);
    }
    private void AlignUiWithCamera(bool isBoxVisible)
    {
        if (isBoxVisible && playerMovement.cameraControl.isCameraDetached) {
            float x = playerMovement.cameraControl.transform.localPosition.x;
            float y = playerMovement.cameraControl.transform.localPosition.y;
            float z = uiCanvas.transform.localPosition.z;
            uiCanvas.transform.position = new Vector3(x, y, z);
        }
    }

    private void SaveDialogueBoxState()
    {
        dialogueBox_x = dialogueBox_rect.localPosition.x;
        dialogueBox_y = dialogueBox_rect.localPosition.y;
        cursor_x = cursor_rect.localPosition.x;
        cursor_y = cursor_rect.localPosition.y;
        
        dialogueText_color = dialogueText.color;
        cursor_color = cursorImage.color;

        uiCanvas_layerName = uiCanvas.sortingLayerName;
        uiCanvas_order = uiCanvas.sortingOrder;
    }

    private void ResetDialogueBoxState()
    {
        dialogueBox_rect.localScale = new Vector3(1f, 1f, 1f);
        dialogueBox_rect.localPosition = new Vector2(dialogueBox_x, dialogueBox_y);
        cursor_rect.localPosition = new Vector2(cursor_x, cursor_y);

        dialogueText.color = dialogueText_color;
        cursorImage.color = cursor_color;

        Color boxColor = dialogueBoxImage.color;
        boxColor.a = 1;
        dialogueBoxImage.color = boxColor;

        uiCanvas.sortingLayerName = uiCanvas_layerName;
        uiCanvas.sortingOrder = uiCanvas_order;
        playerMovement.cameraControl.ResetPlayerCamera();
    }

    public void SetIsContinueTalkBool(bool state)
    {
        isContinueTalk = state;
    }

    private void NormalizeChoiceType()
    {
        choiceType = ChoiceType.OXChoice;
    }

    private string[] CheckTagValueError(string tag_value)
    {
        string[] tag_bundle = tag_value.Split('@');

        if (tag_bundle.Length < 2) {
            Debug.LogError($"Incorrect info : {tag_bundle}");
            return null;
        }
        return tag_bundle;
    }
    private void SetBoldness()
    {
        bool isBold = TextDB.Translate("BoldType", TranslationType.FONT) == "bold";
        if (isBold) {
            dialogueText.fontStyle |= FontStyles.Bold;
            choiceText[0].fontStyle |= FontStyles.Bold;
            choiceText[1].fontStyle |= FontStyles.Bold;
        }
        else {
            dialogueText.fontStyle &= ~FontStyles.Bold;
            choiceText[0].fontStyle &= ~FontStyles.Bold;
            choiceText[1].fontStyle &= ~FontStyles.Bold;
        }
    }
}

public enum DialogueExit { Nothing, Battle, UnlockDoor }