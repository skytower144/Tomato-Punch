using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_is_hurt : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect, gatHit1, gatHit2;
    private Animator anim;
    [HideInInspector] public static bool enemy_isPunched;
    [SerializeField] private ParryBar tomatoParryBar;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    void Update()
    {
        
    } 
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(Enemy_parried.isParried && EnemyControl.isPhysical)
        {
            //GATLING MODE
            tomatoParryBar.parryFill.fillAmount += 0.008f;
            tomatoParryBar.SetWhiteBar();
            Invoke("parryWhiteOff", 0.05f);
            tomatoParryBar.parryWhiteBar.SetActive(true);

            if(col.gameObject.tag.Equals("tomato_LP"))
            {
                Instantiate (gatHit1, new Vector2 (transform.position.x + 2.8f, transform.position.y + 0.8f), Quaternion.identity);
                Instantiate (gatHit2, new Vector2 (transform.position.x + 4f, transform.position.y + 2f), Quaternion.identity);
                anim.Play("battleJola_hurt_L",-1,0f);
            }
            else if(col.gameObject.tag.Equals("tomato_RP"))
            {
                Instantiate (gatHit1, new Vector2 (transform.position.x + 4f, transform.position.y - 0.2f), Quaternion.identity);
                Instantiate (gatHit2, new Vector2 (transform.position.x + 6.5f, transform.position.y + 0.2f), Quaternion.identity);
                anim.Play("battleJola_hurt_L",-1,0f);
            }
            
        }
        else
        {
            //NORMAL PUNCHES
            enemy_isPunched = true;
            if(col.gameObject.tag.Equals("tomato_LP"))
            {
                Instantiate (hitEffect, new Vector2 (transform.position.x + 4.5f, transform.position.y), Quaternion.identity);
                anim.Play("battleJola_hurt_L",-1,0f);
            }
            else if(col.gameObject.tag.Equals("tomato_RP"))
            {
                Instantiate (hitEffect, new Vector2 (transform.position.x + 5f, transform.position.y), Quaternion.identity);
                anim.Play("battleJola_hurt_R",-1,0f);
            }          
        }
    }

    public void ParryBonus()
    {
        tomatoParryBar.parryFill.fillAmount += 0.1f;
    }

    private void parryWhiteOff()
    {
        tomatoParryBar.parryWhiteBar.SetActive(false);
    }
    
}
