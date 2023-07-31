using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField] private string questId; public string QuestID => questId;
    [SerializeField] private bool isCompleted; public bool is_completed => isCompleted;

    [TextArea(5,5)]
    [SerializeField] private string ultimateGoalDescription;
    [SerializeField] private List<CarryGoal> carryGoals = new List<CarryGoal>();
    [SerializeField] private List<DefeatGoal> defeatGoals = new List<DefeatGoal>();
    [SerializeField] public Reward reward;
    
    public bool CheckQuestComplete()
    {
        List<Goal> totalGoals = ReturnTotalGoals();

        foreach (Goal goal in totalGoals) {
            if (!goal.Evaluate())
                return false;
        }
        return true;
    }

    public void InitQuestGoals()
    {
        List<Goal> totalGoals = ReturnTotalGoals();

        foreach (Goal goal in totalGoals)
            goal.Init();
    }

    public void UnsubscribeGoalEvents()
    {
        List<Goal> totalGoals = ReturnTotalGoals();

        foreach (Goal goal in totalGoals)
            goal.UnsubsribeEvent();
    }

    public void GiveReward()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            UnsubscribeGoalEvents();

            foreach (ItemQuantity iq in reward.items)
                Inventory.instance.AddItem(iq.item, iq.count);
            
            GameManager.gm_instance.battle_system.tomatostatus.UpdatePlayerMoney(reward.coinAmount);
        }
    }

    public void UpdateQuestCompletion(bool state)
    {
        isCompleted = state;
    }

    // Must update this function as goal types increase
    private List<Goal> ReturnTotalGoals()
    {
        List<Goal> totalGoals = new List<Goal>();
        totalGoals.AddRange(carryGoals);
        totalGoals.AddRange(defeatGoals);
        return totalGoals;
    }
}

[System.Serializable]
public class QuestData
{
    public string questName;
    public bool questCompleted;
}
