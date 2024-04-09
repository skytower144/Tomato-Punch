using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tomatoHurt : MonoBehaviour
{
    private Animator anim;
    [System.NonSerialized] static public bool isTomatoHurt = false;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private GameObject faintBurstEffect;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextSpawn textSpawn;
    private BattleSystem _battleSystem;
    private EnemyControl _enemyControl;
    [System.NonSerialized] public bool IsHit;
    
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        _battleSystem = GameManager.gm_instance.battle_system;
        _enemyControl = _battleSystem.enemy_control;
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        IsHit = true;

        if((!Enemy_parried.isParried) && !tomatoGuard.preventDamageOverlap)
        {
            isTomatoHurt = true;
            tomatocontrol.DestroyAllMatoPunches();
            TakeDamage(_enemyControl.GetCurrentAttackDamage(), col.gameObject.tag);
        }
    }
    public void TakeDamage(float damage, string colliderTag = "enemy_LA")
    {
        _enemyControl.enemyHitTypes.DetermineHitResponse(damage, colliderTag);

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
        _battleSystem.featherPointManager.ResetFeather();
    }

    public void SetHitBox(bool state)
    {
        _collider.enabled = state;
    }
}
