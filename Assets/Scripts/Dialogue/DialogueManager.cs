using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    private PlayerMovement playerMovement;
    
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Image portrait; 
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypeEffect dialogue_typeEffect;

    private Story currentStory;
    private string currentSentence;
    private bool dialogueIsPlaying;

    private const string PORTRAIT_TAG = "portrait";

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
        dialogueBox.SetActive(false);

        playerMovement = PlayerMovement.instance;
    }

    void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        else if (playerMovement.Press_Key("Interact"))
        {
            if (dialogue_typeEffect.isPrinting)
                dialogue_typeEffect.SetMessage(currentSentence);
            else
                ContinueStory();
        }
    }

    public void EnterDialogue(TextAsset inkJSON)
    {
        playerMovement.SetIsInteracting(true);

        currentStory = new Story(inkJSON.text);
        portrait.sprite = Resources.Load<Sprite>("Portraits/Tomato_neutral");

        dialogueBox.SetActive(true);
        dialogueIsPlaying = true;
    }

    public void ExitDialogue()
    {
        dialogueIsPlaying = false;
        dialogueBox.SetActive(false);
        dialogueText.text = "";

        playerMovement.SetIsInteracting(false);
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue){
            currentSentence = currentStory.Continue();
            HandleTags(currentStory.currentTags);
            dialogue_typeEffect.SetMessage(currentSentence);
        }
        else
            ExitDialogue();
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
                default:
                    Debug.Log("Tag detected but not handled." + tag);
                    break;
            }
        }
    }
}
