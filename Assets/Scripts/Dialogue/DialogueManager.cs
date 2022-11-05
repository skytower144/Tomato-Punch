using System.Collections;
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
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypeEffect dialogue_typeEffect;

    private Story currentStory;
    private string currentSentence;
    private bool dialogueIsPlaying;

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
            dialogue_typeEffect.SetMessage(currentSentence);
        }
        else
            ExitDialogue();
    }

}
