using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleJola_parried : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private string string_parried;
    [SerializeField] private GameObject parryEffect, parryCircle;
    [HideInInspector] public static bool isParried = false;
    [HideInInspector] public static bool pjParried = false;
    
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.tag.Equals("tomato_PRY"))
        {
            Instantiate (parryEffect, new Vector2 (transform.position.x - 0.2f , transform.position.y - 1.3f), Quaternion.identity);

            if(battleJolaControl.isPhysical)    // if isParried True -> enemy cannot move
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
