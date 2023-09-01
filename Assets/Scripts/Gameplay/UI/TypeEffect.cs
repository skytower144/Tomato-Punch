using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TypeEffect : MonoBehaviour
{
    public Action proceed_action = null;
    public int CharPerSeconds;
    [System.NonSerialized] public bool isPrinting;
    [SerializeField] private GameObject arrow;
    private string targetMessage;
    private int index;
    private float interval;
    private TextMeshProUGUI messageText;
    
    private bool isRichTextTag = false;

    private void Awake()
    {
        messageText = GetComponent<TextMeshProUGUI>();
    }
    public void SetMessage(string message)
    {
        targetMessage = message;
        messageText.text = message;
        messageText.maxVisibleCharacters = 0;
        index = 0;

        if(isPrinting){
            messageText.maxVisibleCharacters = targetMessage.Length;
            EffectEnd();
        }
        else {
            EffectStart();
        }
    }
    private void EffectStart()
    {
        isPrinting = true;
        arrow.SetActive(false);

        //Start Animation
        interval = 1.0f / CharPerSeconds;
        StartCoroutine(Effecting(interval));
    }

    IEnumerator Effecting(float inputInterval)
    {
        //End Animation
        if (index == targetMessage.Length) {
            messageText.maxVisibleCharacters++;
            if (isPrinting)
                EffectEnd();
            yield break;
        }

        if ((targetMessage[index] == '<') || (isRichTextTag))
        {
            isRichTextTag = true;
            inputInterval = 0;

            if (targetMessage[index] == '>')
                isRichTextTag = false;
        }
        else
            messageText.maxVisibleCharacters++;

        index++;
        yield return WaitForCache.GetWaitForSecondReal(inputInterval);

        //Recursive
        StartCoroutine(Effecting(interval));
    }

    private void EffectEnd()
    {
        isPrinting = false;
        isRichTextTag = false;
        
        if (proceed_action == null)
            arrow.SetActive(true);
        else {
            StopAllCoroutines();
            proceed_action?.Invoke();
            proceed_action = null;
        }
    }
}
