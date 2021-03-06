using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public int CharPerSeconds;
    public bool isPrinting;
    private string targetMessage;
    private int index;
    private float interval;
    private TextMeshProUGUI messageText;

    private void Awake()
    {
        messageText = GetComponent<TextMeshProUGUI>();
    }
    public void SetMessage(string message)
    {
        if(isPrinting){
            messageText.text = targetMessage;
            CancelInvoke();
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

        transform.GetChild(0).gameObject.SetActive(false);
        messageText.text = "";
        index = 0;

        //Start Animation
        interval = 1.0f / CharPerSeconds;
        Invoke("Effecting", interval);
    }

    private void Effecting()
    {
        //End Animation
        if(messageText.text == targetMessage){
            EffectEnd();
            return;
        }

        messageText.text += targetMessage[index];
        index++;

        //Recursive
        Invoke("Effecting", interval);
    }

    private void EffectEnd()
    {
        isPrinting = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }
    
}
