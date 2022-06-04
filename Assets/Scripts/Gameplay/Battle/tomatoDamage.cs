using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tomatoDamage : MonoBehaviour
{
    public float NormalPunch(float currentAtk)
    {
        return currentAtk * 0.4f;
    }

    public float GatlePunch(float currentAtk)
    {
        return currentAtk * 0.1f;
    }
    public float UpperPunch(float currentAtk)
    {
        return currentAtk * 1.3f;
    }
    public float SkillAttack(float currentAtk, float skillDmg)
    {
        return currentAtk * (skillDmg + 100) / 100 * 0.8f;
    }
    public float SuperAttack(float currentAtk)
    {
        return currentAtk * 1.7f;
    }

}
