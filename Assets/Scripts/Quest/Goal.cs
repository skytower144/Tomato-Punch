using UnityEngine;

[System.Serializable]
public class Goal
{
    public int currentAmount;
    public int requiredAmount;

    public virtual void Init()
    {
        // Default initialize
    }

    public virtual void UnsubsribeEvent()
    {
        // Default Unsub
    }

    public virtual bool Evaluate()
    {
        return (currentAmount >= requiredAmount);
    }
}
