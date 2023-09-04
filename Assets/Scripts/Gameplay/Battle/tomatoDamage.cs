using UnityEngine;
using System;

public class tomatoDamage
{
    public static float NormalPunch(float currentAtk)
    {
        return currentAtk * 0.2f;
    }

    public static float GatlePunch(float currentAtk)
    {
        return NormalPunch(currentAtk) / 4f;
    }
    public static float UpperPunch(float currentAtk)
    {
        return 4 * NormalPunch(currentAtk);
    }
    public static float SkillAttack(float currentAtk, float skillDmg)
    {
        return  2 * NormalPunch(currentAtk) * ((skillDmg + 100f) / 100f);
    }
    public static float DunkAttack(float currentAtk)
    {
        return 3 * NormalPunch(currentAtk);
    }
}
