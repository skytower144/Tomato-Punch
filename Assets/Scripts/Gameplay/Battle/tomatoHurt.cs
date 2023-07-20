using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tomatoHurt : MonoBehaviour
{
    private Animator anim;
    [System.NonSerialized] static public bool isTomatoHurt = false;
    [SerializeField] private GameObject hurtEffect, faintBurstEffect;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextSpawn textSpawn;
    
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if((!Enemy_parried.isParried) && !tomatoGuard.preventDamageOverlap)
        {
            isTomatoHurt = true;
            Instantiate(hurtEffect, new Vector2 (transform.position.x -3.8f, transform.position.y - 0.8f), Quaternion.identity);
            TakeDamage(tomatoguard.damage);

            if(!tomatoControl.isFainted){
                if(col.gameObject.tag.Equals("enemy_LA"))
                {
                    anim.Play("tomato_L_hurt",-1,0f);
                }
                else if(col.gameObject.tag.Equals("enemy_RA"))
                {
                    anim.Play("tomato_R_hurt",-1,0f);
                }
                else if(col.gameObject.tag.Equals("enemy_DA"))
                {
                    anim.Play("tomato_D_hurt",-1,0f);
                }
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

        if(tomatocontrol.currentHealth == 0){
            tomatoControl.isFainted = true;
            GameManager.gm_instance.battle_system.enemy_control.EraseAllAttacks();

            textSpawn.Invoke("AskContinue", 1.8f);

            tomatocontrol.ReleaseGuard();
            
            anim.Play("tomato_faint",-1,0f);
            Instantiate(faintBurstEffect);

            GameManager.gm_instance.battle_system.battleTimeManager.SetSlowSetting(0.01f, 0.8f);
            GameManager.gm_instance.battle_system.battleTimeManager.DoSlowmotion();
        }

        else if (tomatocontrol.current_guardPt == 0)
        {
            anim.Play("tomato_L_hurt",-1,0f);
        }

        GameManager.gm_instance.battle_system.featherPointManager.ResetFeather();
    }
}
