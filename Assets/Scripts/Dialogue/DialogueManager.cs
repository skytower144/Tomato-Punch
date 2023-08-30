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
    private PlayerMovement playerMovement;
    private ChoiceType choiceType = ChoiceType.OXChoice;
    
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject portraitBox;

    [SerializeField] private Image portrait; 
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypeEffect dialogue_typeEffect;
    [SerializeField] private RectTransform dialogue_rect;

    [Header("Choice UI")]
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private RectTransform choiceBoxPTransform;
    [SerializeField] private List<TextMeshProUGUI> choiceText;

    string[] splitTag;

    public NPCController current_npc => currentNpc;
    public bool is_continue_talk => isContinueTalk;

    private NPCController currentNpc;
    private Story currentStory; 
    private string currentSentence;

    private Interactable currentTarget;
    private DialogueExit dialogueExit = DialogueExit.Nothing;

    private bool dialogueIsPlaying, isPromptChoice, isContinueTalk, hideDialogue;

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
    private const string MOVECHOICEBOX_TAG = "movechoicebox";
    private const string RESETCHOICEBOX_TAG = "resetchoicebox";
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
    private const string CAMERA_TAG = "camera";
    private const string TELEPORT_TAG = "teleport";
    private const string SETACTIVE_TAG = "setactive";
    private const string UNLOCKPORTAL_TAG = "unlockportal";
    private const string JOINPARTY_TAG = "joinparty";
    private const string LEAVEPARTY_TAG = "leaveparty";

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

    public void EnterDialogue(TextAsset inkJSON, Interactable interactingTarget)
    {
        if (inkJSON == null) return;

        playerMovement.SetIsInteracting(true);

        currentNpc = (interactingTarget is NPCController) ? ((NPCController)interactingTarget) : null;
        currentTarget = interactingTarget;

        GameManager.gm_instance.partyManager.SetMemberFollow(false);
        
        currentStory = new Story(inkJSON.text);
        portrait.sprite = SpriteDB.ReturnPortrait("Tomato_neutral");

        SetDialogueBox(true);
        dialogueIsPlaying = true;
    }

    public void ExitDialogue()
    {
        dialogueIsPlaying = false;
        SetDialogueBox(false);
        SetPortraitBox(false);
        dialogueText.text = "";
        currentSentence = "";

        playerMovement.SetIsInteracting(false);
        GameManager.gm_instance.partyManager.SetMemberFollow(true);

        InvokeEvent();
    }
    private void InvokeEvent()
    {
        switch (dialogueExit)
        {
            case DialogueExit.Battle:
                currentNpc.StartBattle(GameManager.gm_instance.battle_system.enemy_control._base);
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
            
            if (currentSentence == "/cut") {
                HideDialogue();
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
                choiceText[index].text = choice.text;
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
                
                case ADDKEYEVENT_TAG: // #addkeyevent:Find_BabyCat_StartingPoint
                    GameManager.gm_instance.playerKeyEventManager.AddKeyEvent(tag_value);
                    break;
                
                case ROLLBACKDIALOGUE_TAG: // #rollbackdialogue:_
                    currentNpc.RollbackDialogue();
                    break;

                case CONTINUETALK_TAG: // #continuetalk:_
                    isContinueTalk = true;
                    break;
                
                case TURNPLAYER_TAG: //#turnplayer:LEFT@1.7// #turnplayer:LEFT
                    string[] directionInfo = CheckTagValueError(tag_value);
                    float delay = 0f;

                    if (directionInfo.Length == 2)
                        delay = float.Parse(directionInfo[1]);
                    
                    StartCoroutine(PlayerMovement.instance.DelayFaceAdjustment(directionInfo[0], delay));
                    break;

                case ANIMATE_TAG: // #animate:fainted
                    currentNpc.Play(tag_value);
                    break;
                
                case FOCUSANIMATE_TAG: // #focusanimate:StartingPoint_Donut@angry // #focusanimate:this@drop@stop // use with #changeidle
                    bool stopAnimation = false;
                    string[] animInfo = tag_value.Split('@');
                    NPCController npc = ((animInfo[0] == "_") || (animInfo[0] == "this")) ? currentNpc : NPCManager.instance.npc_dict[animInfo[0]];

                    if (animInfo.Length > 2)
                        stopAnimation = true;
                    
                    HideDialogue();
                    npc.Play(animInfo[1], ShowAndContinueDialogue, stopAnimation);
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
                
                case MOVECHOICEBOX_TAG:
                    choiceBoxPTransform.localPosition = new Vector2(725, -103.5f);
                    break;
                
                case RESETCHOICEBOX_TAG:
                    choiceBoxPTransform.localPosition = new Vector2(498, 166);
                    break;
                
                case VIEWSHOP_TAG:
                    choiceType = ChoiceType.ShopChoice;
                    break;

                case CALCULATESHOP_TAG:
                    currentStory.variablesState["totalPrice"] = GameManager.gm_instance.ui_control.ui_shop.total_price;
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

                case CAMERA_TAG:
                    WorldCamera.instance.PlayCameraEffect(tag_value);
                    break;
                
                case TELEPORT_TAG: // #teleport:StartingPoint_Donut@x@y
                    string[] posInfo = CheckTagValueError(tag_value);

                    NPCController npc2 = NPCManager.instance.npc_dict[posInfo[0]];
                    npc2.Teleport(float.Parse(posInfo[1]), float.Parse(posInfo[2]));
                    break;
                
                case HIDENPC_TAG:
                    currentNpc.isDisabled = true;
                    currentNpc.gameObject.SetActive(false);
                    break;
                
                case SETACTIVE_TAG: // #setactive:StartingPoint_Donut@true
                    string[] info = CheckTagValueError(tag_value);

                    NPCController npc3 = NPCManager.instance.npc_dict[info[0]];
                    bool state = (info[1] == "true") ? true : false;
                    npc3.gameObject.SetActive(state);
                    break;
                
                case UNLOCKPORTAL_TAG: // #unlockportal:_
                    dialogueExit = DialogueExit.UnlockDoor;
                    break;

                case JOINPARTY_TAG: // #joinparty:_
                    GameManager.gm_instance.partyManager.JoinParty(current_npc);
                    break;
                
                case LEAVEPARTY_TAG: // #leaveparty:_
                    string leavingMemberName = (tag_value == "_") ? current_npc.ReturnID() : tag_value;
                    GameManager.gm_instance.partyManager.LeaveParty(leavingMemberName);
                    break; 

                default:
                    Debug.Log("Tag detected but not handled." + tag);
                    break;
            }
        }
    }

    private void DisplayDialogue()
    {
        if (String.IsNullOrEmpty(currentSentence) || currentSentence == "/cut")
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
        hideDialogue = false;
        SetDialogueBox(true);
        DisplayDialogue();
    }

    private void HideDialogue()
    {
        hideDialogue = true;
        SetDialogueBox(false);
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

    private void SetDialogueBox(bool state)
    {
        dialogueBox.SetActive(state);
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
}

public enum DialogueExit { Nothing, Battle, UnlockDoor }

[System.Serializable]
public class KeyEventDialogue
{
    public PlayerKeyEvent keyEvent;
    public string inkFileName;

    public KeyEventDialogue()
    {
        this.keyEvent = PlayerKeyEvent.None;
    }
}