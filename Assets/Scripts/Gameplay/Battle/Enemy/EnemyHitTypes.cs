using UnityEngine;

public class EnemyHitTypes : MonoBehaviour
{
    private tomatoControl _tomatoControl;
    private EnemyControl _enemyControl;
    private EnemyAnimControl _enemyAnimControl;
    [SerializeField] private GameObject hurtEffect;
    void Start()
    {
        _tomatoControl = GameManager.gm_instance.battle_system.tomato_control;
        _enemyControl = GameManager.gm_instance.battle_system.enemy_control;
        _enemyAnimControl = _enemyControl.enemyAnimControl;
    }
    
    public void DetermineHitResponse(string colliderTag)
    {
        switch (_enemyAnimControl.CurrentHitType) {
            case HitType.Normal:
                Instantiate(hurtEffect, new Vector2 (_tomatoControl.transform.position.x - 3.8f, _tomatoControl.transform.position.y - 0.8f), Quaternion.identity);

                if(!tomatoControl.isFainted){
                    if(colliderTag.Equals("enemy_LA"))
                    {
                        _tomatoControl.tomatoAnim.Play("tomato_L_hurt",-1,0f);
                    }
                    else if(colliderTag.Equals("enemy_RA"))
                    {
                        _tomatoControl.tomatoAnim.Play("tomato_R_hurt",-1,0f);
                    }
                    else if(colliderTag.Equals("enemy_DA"))
                    {
                        _tomatoControl.tomatoAnim.Play("tomato_D_hurt",-1,0f);
                    }
                }
                break;
            
            case HitType.Absorb:
                _enemyAnimControl.CancelScheduledInvokes();

                string animName = "Donut_attack_2_absorb";
                float attackFinishedTime = 5 / _enemyAnimControl.FpsDict[animName].Item1;

                _enemyControl.enemyAnim.Play(animName, -1, 0f);
                _tomatoControl.tomatoAnim.Play("tomato_absorbed", -1, 0f);
                _tomatoControl.gaksung_OFF();
                _tomatoControl.tomatoHurtStart();

                StartCoroutine(_tomatoControl.ChangeAnimationState("tomato_D_hurt", attackFinishedTime));
                StartCoroutine(_enemyAnimControl.SetCollider(true, attackFinishedTime));
                _enemyControl.Invoke("actionOver", _enemyAnimControl.FpsDict[animName].Item2);
                break;
        }
    }
}

public enum HitType { Normal, Absorb }