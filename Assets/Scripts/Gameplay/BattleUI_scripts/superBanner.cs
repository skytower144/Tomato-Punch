using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class superBanner : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Animator tomato_anim;

    [Header("Key: ItemName | Value: AnimationString name")]
    [SerializeField] private StringStringDictionary superAnimDict;

    void superSelection()
    {
        tomato_anim.enabled = true;
        tomato_anim.Play(superAnimDict[tomatocontrol.tomatoSuperEquip.ItemName], -1, 0f);
        gameObject.SetActive(false);
    }
    
}
