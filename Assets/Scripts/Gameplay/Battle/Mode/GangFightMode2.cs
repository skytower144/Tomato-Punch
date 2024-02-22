using System.Collections.Generic;
using UnityEngine;

public class GangFightMode2 : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Collider2D _col;
    [SerializeField] private string _counteredTag;
    [SerializeField] private List<GameObject> _projectile;
    private GangFightMode _gangFight; 

    void Start()
    {
        _gangFight = GameManager.gm_instance.battle_system.gangFightMode;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        string hitType = "";

        if (col.gameObject.tag.Equals("tomato_LP") || col.gameObject.tag.Equals("tomato_SK"))
            hitType = "L";

        if (col.gameObject.tag.Equals("tomato_RP"))
            hitType = "R";
        
        if (!string.IsNullOrEmpty(hitType)) {
            DisableHitBox();
            _gangFight.RemoveCurrentAttack();
            // MarkDead();

            _anim.Play(_counteredTag, -1, 0f);
            GameManager.gm_instance.battle_system.enemy_control.enemy_hurt.HitEffect(hitType);
            GameManager.gm_instance.battle_system.enemy_control.enemy_Countered.CounterEffect();
        }
    }
    void EnableHitBox()
    {
        _col.enabled = true;
    }
    void DisableHitBox()
    {
        Invoke("DelayedDisableHitBox", 0.06f);
    }
    void DelayedDisableHitBox()
    {
        _gangFight.DuplicateRd.InitFlash();
        _col.enabled = false;
    }
    void DisableIsAttacking(string idle)
    {
        _gangFight.IsAction = false;
        _anim.Play(idle, -1, 0f);
    }
    void DisableObject(int index)
    {
        _gangFight.AnimList[index].gameObject.SetActive(false);
    }
    void HitFrame(string type)
    {
        // PH_LEFT | PJ_1

        string[] typeInfo = type.Split('_');

        _gangFight.CurrentGuy = gameObject;
        _gangFight.CurrentState = typeInfo[0] == "PH" ? GangFightMode.GangParry.Physical : GangFightMode.GangParry.Projectile;

        if (_gangFight.CurrentState == GangFightMode.GangParry.Physical) {
            _gangFight.SpawnAttackBox(typeInfo[1]);
        }
        else {
            _gangFight.ParriedPjIndex = int.Parse(typeInfo[1]);
            _gangFight.SpawnAttackBox(typeInfo[0]);
        }
    }
    void HitDamage(float dmg)
    {
        GameManager.gm_instance.battle_system.enemy_control.gangFightDmg = dmg;
    }
    void SpawnProjectile(int index)
    {
        _gangFight.CurrentProjectile = Instantiate(_projectile[index], transform.parent);
    }
    void SpawnObjectAt(string tag)
    {
        //projectileIndex_posX_posY_scaleX_scaleY

        string[] tagInfo = tag.Split('_');

        int index = int.Parse(tagInfo[0]);
        float posX = float.Parse(tagInfo[1]);
        float posY = float.Parse(tagInfo[2]);
        int scaleX = int.Parse(tagInfo[3]);
        int scaleY = int.Parse(tagInfo[4]);

        GameObject spawned = Instantiate(_projectile[index], GameManager.gm_instance.battle_system.enemy_control.transform);
        spawned.transform.localPosition = new Vector2(posX, posY);
        spawned.transform.localScale = new Vector2(scaleX, scaleY);
    }
    void PlayAnimation(string tag)
    {
        // 1-empl_1_atk
        string[] tagInfo = tag.Split('-');

        if (tagInfo.Length > 1) {
            int index = int.Parse(tagInfo[0]);
            
            if (!_gangFight.IsDead(index)) {
                _gangFight.AnimList[index].Play(tagInfo[1], -1, 0f);
                ShowTarget(index);
            }
            return;
        }
        _anim.Play(tag, -1, 0f);
    }
    void EnableAction()
    {
        _gangFight.IsAction = false;
    }
    private bool ActExists(string findingName)
    {
        foreach (GangFightMode.ActDetailWithAnim act in _gangFight.Attacks) {
            if (act.Name == findingName)
                return true;
        }
        return false;
    }
    void AddAct(string tag)
    {
        // AnimIndex-Name-Percentage
        string[] actInfo = tag.Split('-');

        int animIndex = int.Parse(actInfo[0]);       
        string name = actInfo[1];
        int percentage = int.Parse(actInfo[2]);

        if (ActExists(name))
            return;
        
        _gangFight.Attacks.Add(new GangFightMode.ActDetailWithAnim(
            animIndex,
            name,
            percentage
        ));
        _gangFight.SetMaxPercent(percentage);
    }
    void CheckBigEmployeeParry()
    {
        bool empl14 = !_gangFight.IsDead(9);
        bool empl16 = !_gangFight.IsDead(10);

        _gangFight.RemoveAttacks(8);

        if (empl14 && empl16)
            AddAct("8-empl_big_atk-10");

        else if (!empl14 && empl16)
            AddAct("8-empl_big_rightAtk-10");
        
        else if (empl14 && !empl16)
            AddAct("8-empl_big_leftAtk-10");
        
        else if (!empl14 && !empl16)
            Invoke("DelaySadAnimation", 0.1f);
    }
    private void DelaySadAnimation()
    {
        PlayAnimation("8-empl_big_sad");
    }
    void MarkDead(int index)
    {
        _gangFight.MarkDead(index);
    }
    void RemoveAttacks(int index)
    {
        _gangFight.RemoveAttacks(index);
    }
    void HideTarget(int index)
    {
        _gangFight.HideTarget(index);
    }
    private void ShowTarget(int index)
    {
        _gangFight.ColorIndex = index;
        _gangFight.Invoke("ShowTarget", 0.05f);
    }
    void HurtBlink(int index)
    {
        _gangFight.DuplicateRd.InitEnemySr(_gangFight.SrList[index]);
        _gangFight.DuplicateRd.InitFlash();
        StartCoroutine(_gangFight.DuplicateRd.BlinkEffect(2, 0.04f));
    }
    void CounterFlash(int index)
    {
        _gangFight.DuplicateRd.InitEnemySr(_gangFight.SrList[index]);
        _gangFight.DuplicateRd.FlashEffect(0.3f, 1);
    }
}