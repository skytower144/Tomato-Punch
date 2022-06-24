using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatedText : MonoBehaviour
{
    private TypeEffect typeEffect;
    private GameObject battle_text, text_box, cursor;
    [SerializeField] private GameObject fadeOut;
    private List<string> textList = new List<string>();
    private int textIndex;
    private bool startText = false;
    void Start()
    {
        battle_text = transform.GetChild(0).gameObject;
        text_box = battle_text.transform.GetChild(0).gameObject;
        cursor = text_box.transform.GetChild(0).gameObject;

        typeEffect = text_box.GetComponent<TypeEffect>();
        textIndex = -1;

        Invoke("SpawnTextBox", 1.5f);
    }

    void Update()
    {
        if (startText){
            if(Input.GetKeyDown(KeyCode.O)){
                CallText();
            }
        }
    }

    private void SpawnTextBox()
    {
        battle_text.SetActive(true);
        InitializeText();
        
        CallText();
        startText = true;
    }

    private void InitializeText()
    {
        string moneyMessage = "You lost ? coins.";
        string ExitMessage = "Immense fatigue overwhelms you...";

        textList.Add(moneyMessage);
        textList.Add(ExitMessage);
    }

    private void CallText()
    {
        if(!typeEffect.isPrinting){
            textIndex += 1;
        }
        
        if (textIndex == textList.Count-1){
            startText = false;
            typeEffect.SetMessage(textList[textIndex]);
            Invoke("ScreenFadeOut", 0.75f);
        }
        
        else {
            typeEffect.SetMessage(textList[textIndex]);
        }
    }

    private void ScreenFadeOut()
    {
        Instantiate(fadeOut, transform.parent);
    }
}
