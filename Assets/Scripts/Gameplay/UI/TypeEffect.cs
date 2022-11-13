using System.Collections;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public int CharPerSeconds;
    [System.NonSerialized] public bool isPrinting;
    [SerializeField] private GameObject arrow;
    private string targetMessage;
    private int index;
    private float interval;
    private TextMeshProUGUI messageText;

    private bool isRichTextTag = false;
    private string tempTag = "";

    private void Awake()
    {
        messageText = GetComponent<TextMeshProUGUI>();
    }
    public void SetMessage(string message)
    {
        if(isPrinting){
            messageText.text = targetMessage;
            EffectEnd();
        }
        else {
            targetMessage = message;
            EffectStart();
        }
    }
    private void EffectStart()
    {
        isPrinting = true;

        arrow.SetActive(false);
        messageText.text = "";
        tempTag = "";
        index = 0;

        //Start Animation
        interval = 1.0f / CharPerSeconds;
        StartCoroutine(Effecting(interval));
    }

    IEnumerator Effecting(float inputInterval)
    {
        //End Animation
        if(messageText.text == targetMessage){
            EffectEnd();
            yield break;
        }

        if ((targetMessage[index] == '<') || (isRichTextTag))
        {
            isRichTextTag = true;
            inputInterval = 0;

            tempTag += targetMessage[index];

            if (targetMessage[index] == '>')
            {
                isRichTextTag = false;
                messageText.text += tempTag;
            }
        }
        else
            messageText.text += targetMessage[index];

        index++;
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(inputInterval));

        //Recursive
        StartCoroutine(Effecting(interval));
    }

    private void EffectEnd()
    {
        isPrinting = false;
        isRichTextTag = false;
        arrow.SetActive(true);
    }
    
}
