using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;

public enum ChoiceType { OXChoice, ShopChoice }

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
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
    
    private UnityEngine.Object tempObject = null;
    private NPCController currentNpc;
    private Story currentStory;
    private string currentSentence;
    private bool dialogueIsPlaying, isPromptChoice;
    private bool isContinueTalk = false; public bool is_continue_talk => isContinueTalk;

    private const string PORTRAIT_TAG = "portrait";
    private const string HIDEPORTRAIT_TAG = "hideportrait";
    private const string DIALOGUE_TAG = "nextdialogue";
    private const string CONTINUETALK_TAG = "continuetalk";
    private const string ANIM_TAG = "animate";
    private const string BATTLE_TAG = "battle";
    private const string PURCHASEONE_TAG = "purchaseone";
    private const string CHECKPLAYERMONEY_TAG = "checkplayermoney";
    private const string MOVECHOICEBOX_TAG = "movechoicebox";
    private const string RESETCHOICEBOX_TAG = "resetchoicebox";
    private const string VIEWSHOP_TAG = "viewshop";
    private const string CALCULATESHOP_TAG = "calculateshop";
    private const string PAYSHOP_TAG = "payshop";
    private const string CONTINUESHOPPING_TAG = "continueshopping";

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
        if (!dialogueIsPlaying)
        {
            return;
        }
        else if (playerMovement.Press_Key("Interact") && !isPromptChoice)
        {
            if (dialogue_typeEffect.isPrinting)
                dialogue_typeEffect.SetMessage(currentSentence);
            else
                ContinueStory();
        }
        else if (dialogueIsPlaying && (currentSentence == ""))
            ContinueStory();
    }

    public void EnterDialogue(TextAsset inkJSON, NPCController current_npc)
    {
        playerMovement.SetIsInteracting(true);

        currentNpc = current_npc;
        currentStory = new Story(inkJSON.text);
        portrait.sprite = Resources.Load<Sprite>("Portraits/Tomato_neutral");

        SetDialogueBox(true);

        dialogueIsPlaying = true;
    }

    public void ExitDialogue()
    {
        dialogueIsPlaying = false;
        SetDialogueBox(false);
        dialogueText.text = "";
        currentSentence = "";

        playerMovement.SetIsInteracting(false);
        InvokeEvent();
    }

    private void InvokeEvent()
    {
        if (tempObject is EnemyBase)
        {
            currentNpc.StartBattle((EnemyBase)tempObject);
        }

        tempObject = null;
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue){
            currentSentence = currentStory.Continue();
            HandleTags(currentStory.currentTags);

            if (String.IsNullOrEmpty(currentSentence))
                ContinueStory();
            
            else {
                if (CheckChoiceSentence()) {
                    dialogue_typeEffect.proceed_action = DisplayChoices;
                }
                dialogue_typeEffect.SetMessage(currentSentence);
            }
        }
        else
            ExitDialogue();
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
        ContinueStory();
        isPromptChoice = false;
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
                continue;
            
            string tag_key = splitTag[0].Trim();
            string tag_value = splitTag[1].Trim();

            switch (tag_key)
            {
                case PORTRAIT_TAG:
                    SetDialogueBox(true, true);
                    portrait.sprite = Resources.Load<Sprite>($"Portraits/{tag_value}");
                    break;
                
                case HIDEPORTRAIT_TAG:
                    SetDialogueBox(true, false);
                    break;

                case DIALOGUE_TAG:
                    currentNpc.LoadNextDialogue(tag_value);
                    break;

                case CONTINUETALK_TAG:
                    isContinueTalk = true;
                    break;

                case ANIM_TAG:
                    currentNpc.Play(tag_value);
                    break;

                case BATTLE_TAG:
                    tempObject = currentNpc.enemyData;
                    break;

                case PURCHASEONE_TAG:
                    GameManager.gm_instance.ui_control.ui_shop.PurchaseOneItem(tag_value);
                    break;

                case CHECKPLAYERMONEY_TAG:
                    int amount = int.Parse(tag_value);
                    if (!GameManager.gm_instance.battle_system.tomatostatus.CheckEnoughMoney(amount))
                        currentStory.variablesState["enoughMoney"] = false;
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

                default:
                    Debug.Log("Tag detected but not handled." + tag);
                    break;
            }
        }
    }

    private void SetDialogueBox(bool state, bool hasPortrait = false)
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
}
