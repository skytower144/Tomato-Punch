using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_parried : MonoBehaviour
{
    private Animator anim;
    private string string_parried;
    [SerializeField] private EnemyBase _enemyBase;
    [SerializeField] private GameObject parryEffect, parryCircle;
    [HideInInspector] public static bool isParried = false;
    [HideInInspector] public static bool pjParried = false;
    
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        string_parried = _enemyBase.Parried_AnimationString;
    }
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.tag.Equals("tomato_PRY"))
        {
            Instantiate (parryEffect, new Vector2 (transform.position.x - 0.2f , transform.position.y - 1.3f), Quaternion.identity);

            if(EnemyControl.isPhysical)    // if isParried True -> enemy cannot move
            {
                isParried = true;
                Instantiate (parryCircle, new Vector2 (transform.position.x - 0.2f , transform.position.y - 1.3f), Quaternion.identity);
                anim.Play(string_parried);
            }
            else
            {
                pjParried = true;
            }
        }
    }
}
