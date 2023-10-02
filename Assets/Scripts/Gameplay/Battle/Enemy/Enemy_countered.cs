using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_countered : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private EnemyControl enemyControl;
    
    [SerializeField] private Enemy_is_hurt enemy_is_hurt;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private CounterTrack counterTrack;
    [SerializeField] private GameObject counterEffect, hitEffect, counterPunch_effect, screenFlash;
    [HideInInspector] public static bool enemy_isCountered;
    [HideInInspector] public bool counter_is_initialized = false;

    [System.NonSerialized] public int totalCounter = 0;
    void OnEnable()
    {
        if (!counter_is_initialized)
        {
            counter_is_initialized = true;
            enemy_isCountered = false;
            
            anim = GetComponentInParent<Animator>();
        }
    }

    void Update()
    {
        if(tomatoControl.enemyFreeze)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        enemyControl.enemyAnimControl.CancelScheduledInvokes();
        
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_is_hurt.enemyIsHit = false;
        GameManager.gm_instance.assistManager.SetIsBlast(false);
        enemy_isCountered = true;
        
        totalCounter += 1;

        if(tomatocontrol.tomatoes<5){
            tomatocontrol.tomatoes += 1;
            counterTrack.CounterTracker();
        }

        enemy_is_hurt.enemyHurtDamage(tomatocontrol.dmg_normalPunch);
        if(!enemy_is_hurt.checkDefeat("CTR"))
        {
            Instantiate (counterEffect, new Vector2 (transform.position.x + 2.3f , transform.position.y-0.2f), Quaternion.identity);
            Instantiate (hitEffect, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
            Instantiate (counterPunch_effect, new Vector2 (transform.position.x + 4.7f , transform.position.y - 0.4f), Quaternion.identity);
            
            Instantiate (screenFlash, new Vector2 (transform.position.x + 2.3f , transform.position.y - 0.5f), Quaternion.identity);
            
            enemyControl.enemyAnimControl.Act(enemyControl._base.Knockback_AnimationString, BattleActType.Knockback);
            gameObject.SetActive(false);
        }
    }

    public void ResetCounterPoints()
    {
        totalCounter = 0;
        tomatocontrol.tomatoes = 0;
        counterTrack.CounterTracker();
    }
}
