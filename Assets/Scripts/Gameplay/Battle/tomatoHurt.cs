using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tomatoHurt : MonoBehaviour
{
    private Animator anim;
    [HideInInspector] public bool isTomatoHurt = false;
    [SerializeField] private GameObject hurtEffect;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private HealthBar healthBar;
    
    
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if((!tomatoControl.isGuard || !Enemy_parried.isParried) && !tomatoGuard.preventDamageOverlap)
        {
            isTomatoHurt = true;
            Instantiate(hurtEffect, new Vector2 (transform.position.x -3.8f, transform.position.y - 0.8f), Quaternion.identity);
            if(col.gameObject.tag.Equals("enemy_LA"))
            {
                anim.Play("tomato_L_hurt",-1,0f);
                TakeDamage(tomatoguard.damage);
            }
            else if(col.gameObject.tag.Equals("enemy_RA"))
            {
                anim.Play("tomato_R_hurt",-1,0f);
                TakeDamage(tomatoguard.damage);
            }
            else if(col.gameObject.tag.Equals("enemy_DA"))
            {
                anim.Play("tomato_D_hurt",-1,0f);
                TakeDamage(tomatoguard.damage);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        if(tomatocontrol.currentHealth < damage)
            damage = tomatocontrol.currentHealth;
        tomatocontrol.currentHealth -= damage;

        healthBar.SetHealth(tomatocontrol.currentHealth);
        healthBar.hpShrinkTimer = HealthBar.HP_SHRINKTIMER_MAX;
    }
}
