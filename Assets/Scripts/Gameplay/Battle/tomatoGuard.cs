using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class tomatoGuard : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject guardEffect, guardEffect2, guardBreakEffect;
    [HideInInspector] public static bool isParry = false;
    [HideInInspector] public static bool preventDamageOverlap = false;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private GuardBar guardBar;
    [SerializeField] private tomatoHurt tomatohurt;
    [HideInInspector] public float damage;
    private float leftoverDamage, reducedDamage;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if(tomatoControl.isGuard && !tomatoHurt.isTomatoHurt)
        {
            if (tomatocontrol.PressKey("RightPunch"))
            {
                isParry = true;
                anim.Play("tomato_parry",-1,0f);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(!isParry)
        {
            if(col.gameObject.tag.Equals("enemy_LA") || (col.gameObject.tag.Equals("enemy_RA")))
            {
                guardDamageConversion();
                if(tomatocontrol.current_guardPt >= reducedDamage)
                {
                    playGuardEffects();
                    TakeGuardDamage(reducedDamage);

                    anim.Play("tomato_guardAft");
                }
                else if(0 < tomatocontrol.current_guardPt)
                {
                    preventDamageOverlap = true;

                    playGuardEffects();
                    playGuard_BreakEffects();
                    TakeGuardDamage(reducedDamage);

                    tomatohurt.TakeDamage(leftoverDamage);
                }
                else
                {
                    preventDamageOverlap = true;

                    playGuardEffects();
                    playGuard_BreakEffects();

                    tomatohurt.TakeDamage(damage);
                }
            }
        }
    }
    void TakeGuardDamage(float dmg)
    {
        if(tomatocontrol.current_guardPt < dmg)
        {
            leftoverDamage = dmg - tomatocontrol.current_guardPt;
            dmg = tomatocontrol.current_guardPt;
        }
        tomatocontrol.current_guardPt -= dmg;

        float normalize_guardPt = dmg;
        normalize_guardPt /= tomatocontrol.maxGuard;        // noramalize value because guardDamagedFill ranges 0~1

        guardBar.SetGuardbar(tomatocontrol.current_guardPt);
        guardBar.guardDamaged(normalize_guardPt);
    }

    void playGuardEffects()
    {
        Instantiate(guardEffect, new Vector2 (transform.position.x -2.2f, transform.position.y - 1.7f), Quaternion.identity);
        Instantiate(guardEffect2, new Vector2 (transform.position.x -3.5f, transform.position.y - 2f), Quaternion.identity);
    }

    void playGuard_BreakEffects()
    {
        Instantiate(guardBreakEffect, new Vector2 (transform.position.x -2.5f, transform.position.y - 1.2f), Quaternion.identity);
    }


    void guardDamageConversion()
    {
        if(damage % 2 == 0)
            reducedDamage = damage / 2;
        else
            reducedDamage = (damage + 1) / 2;
    }
}
