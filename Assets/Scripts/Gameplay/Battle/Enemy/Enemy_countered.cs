using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_countered : MonoBehaviour
{
    private Animator anim;
    private string string_countered;
    [SerializeField] private EnemyBase _enemyBase;
    [SerializeField] private GameObject counterEffect, counterPunch_effect, screenFlash;
    [HideInInspector] public static bool enemy_isCountered;
    private GameObject _instance1;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        string_countered = _enemyBase.Countered_AnimationString;
    }

    void Update()
    {
        if(tomatoControl.enemyFreeze)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        Enemy_is_hurt.enemy_isPunched = false;
        enemy_isCountered = true;
        if(col.gameObject.tag.Equals("tomato_LP") || (col.gameObject.tag.Equals("tomato_RP")))
        {
            Instantiate (counterEffect, new Vector2 (transform.position.x + 2.3f , transform.position.y-0.2f), Quaternion.identity);
            _instance1 = Instantiate (counterPunch_effect, new Vector2 (transform.position.x + 4.7f , transform.position.y - 0.4f), Quaternion.identity);
            Destroy(_instance1,0.38f);
            Instantiate (screenFlash, new Vector2 (transform.position.x + 2.3f , transform.position.y - 0.5f), Quaternion.identity);
            
            anim.Play(string_countered,-1,0f);
            Destroy(gameObject);
        }
    }
}
