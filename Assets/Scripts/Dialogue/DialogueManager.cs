using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    private PlayerMovement playerMovement;
    
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject portraitBox;
    

    [SerializeField] private Image portrait; 
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypeEffect dialogue_typeEffect;
    [SerializeField] private RectTransform dialogue_rect;

    [Header("Choice UI")]
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private List<TextMeshProUGUI> choiceText;
    
    private UnityEngine.Object tempObject = null;
    private NPCController currentNpc;
    private Story currentStory;
    private string currentSentence;
    private bool dialogueIsPlaying, isPromptChoice;
    private bool isContinueTalk = false; public bool is_continue_talk => isContinueTalk;

    private const string PORTRAIT_TAG = "portrait";
    private const string DIALOGUE_TAG = "nextdialogue";
    private const string CONTINUETALK_TAG = "continuetalk";
    private const string ANIM_TAG = "animate";
    private const string BATTLE_TAG = "battle"; 

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

        SetDialogueBox(true, current_npc.hasPortrait);

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
        int index = 0;

        foreach (Choice choice in currentChoices)
        {
            choiceText[index].text = choice.text;
            index++; 
        }

        choiceBox.GetComponent<ChoiceSelect>().proceedAction = MakeChoice;
        choiceBox.SetActive(true);
    }

    private void MakeChoice()
    {
        int choiceNumber = choiceBox.GetComponent<ChoiceSelect>().choice_number;
        currentStory.ChooseChoiceIndex(choiceNumber);

        choiceBox.SetActive(false);
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
                    portrait.sprite = Resources.Load<Sprite>($"Portraits/{tag_value}");
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
}
