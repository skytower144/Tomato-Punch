using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tomatoHurt : MonoBehaviour
{
    private Animator anim;
    [System.NonSerialized] static public bool isTomatoHurt = false;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private GameObject hurtEffect, faintBurstEffect;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextSpawn textSpawn;
    private BattleSystem _battleSystem;
    private EnemyControl _enemyControl;
    
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        _battleSystem = GameManager.gm_instance.battle_system;
        _enemyControl = _battleSystem.enemy_control;
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if((!Enemy_parried.isParried) && !tomatoGuard.preventDamageOverlap)
        {
            isTomatoHurt = true;
            tomatocontrol.DestroyAllMatoPunches();
            _enemyControl.enemyHitTypes.DetermineHitResponse(col.gameObject.tag);
            TakeDamage(_enemyControl.GetCurrentAttackDamage());
        }
    }
    public void TakeDamage(float damage)
    {
        if (tomatoControl.isFainted) return;
        
        if(tomatocontrol.currentHealth < damage)
            damage = tomatocontrol.currentHealth;
        tomatocontrol.currentHealth -= damage;

        healthBar.SetHealth(tomatocontrol.currentHealth);
        healthBar.hpShrinkTimer = HealthBar.HP_SHRINKTIMER_MAX;

        if(tomatocontrol.currentHealth == 0){
            tomatoControl.isFainted = true;
            _enemyControl.EraseAllAttacks();
            _enemyControl.enemyHitTypes.StopAllCoroutines();

            textSpawn.Invoke("AskContinue", 1.8f);

            tomatocontrol.ReleaseGuard();
            
            anim.Play("tomato_faint",-1,0f);
            Instantiate(faintBurstEffect);
            tomatocontrol.ResetGaksung();

            _battleSystem.battleTimeManager.SetSlowSetting(0.01f, 0.8f);
            _battleSystem.battleTimeManager.DoSlowmotion();
        }

        else if (tomatocontrol.current_guardPt == 0)
        {
            anim.Play("tomato_L_hurt",-1,0f);
        }

        _battleSystem.featherPointManager.ResetFeather();
    }

    public void SetHitBox(bool state)
    {
        _collider.enabled = state;
    }
}
