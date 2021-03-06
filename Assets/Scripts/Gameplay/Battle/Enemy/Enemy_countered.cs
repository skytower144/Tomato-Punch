using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_countered : MonoBehaviour
{
    private Animator anim;
    private string string_countered;
    [SerializeField] private EnemyBase _enemyBase;
    [SerializeField] private Enemy_is_hurt enemy_is_hurt;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private CounterTrack counterTrack;
    [SerializeField] private GameObject counterEffect, counterPunch_effect, screenFlash;
    [HideInInspector] public static bool enemy_isCountered;
    [HideInInspector] public bool counter_is_initialized = false;
    public int totalCounter = 0;
    private GameObject instance1;
    void OnEnable()
    {
        if (!counter_is_initialized)
        {
            counter_is_initialized = true;

            anim = GetComponentInParent<Animator>();
            string_countered = _enemyBase.Countered_AnimationString;
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
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_is_hurt.enemyIsHit = false;
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
            instance1 = Instantiate (counterPunch_effect, new Vector2 (transform.position.x + 4.7f , transform.position.y - 0.4f), Quaternion.identity);
            Destroy(instance1, 0.38f);
            Instantiate (screenFlash, new Vector2 (transform.position.x + 2.3f , transform.position.y - 0.5f), Quaternion.identity);
            
            anim.Play(string_countered,-1,0f);
            gameObject.SetActive(false);
        }
    }
}
