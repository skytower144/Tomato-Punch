using System.Collections.Generic;
using UnityEngine;

public class superBanner : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Animator tomato_anim;

    void superSelection()
    {
        tomato_anim.enabled = true;
        tomato_anim.Play(tomatocontrol.tomatoSuperEquip.animTag, -1, 0f);
        gameObject.SetActive(false);
    }
}
