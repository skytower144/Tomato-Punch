using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class PromptPopup : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    [System.NonSerialized] public string animTag;
    [SerializeField] private Animator anim;
    private ItemMenuNavigation itemMenuNavigation;
    
    void OnDisable()
    {
        Destroy(gameObject);
    }
    void Start()
    {
        UIControl.instance.SetFontData(popupText, "ConfirmPrompt");
        anim.Play(animTag);
        itemMenuNavigation = GameManager.gm_instance.consumable_navigation;
        StartCoroutine(ExitPopup());
    }

    void Update()
    {
        if (!itemMenuNavigation.is_navigating)
        {
            OnDisable();
        }
    }

    IEnumerator ExitPopup()
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(0.7f));
        Destroy(gameObject);
    }
}
