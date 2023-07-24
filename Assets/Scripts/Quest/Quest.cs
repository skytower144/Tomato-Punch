using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField] private string questId; public string QuestID => questId;
    [SerializeField] private bool isCompleted; public bool is_completed => isCompleted;

    [TextArea(5,5)]
    [SerializeField] private string ultimateGoalDescription;
    [SerializeField] private List<ItemQuantity> itemRewardList = new List<ItemQuantity>();
    [SerializeField] private List<CarryGoal> carryGoals = new List<CarryGoal>();
    [SerializeField] private List<DefeatGoal> defeatGoals = new List<DefeatGoal>();

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

            foreach (ItemQuantity reward in itemRewardList)
                Inventory.instance.AddItem(reward.item, reward.count);
        }
    }

    public void UpdateQuestCompletion(bool state)
    {
        isCompleted = state;
    }

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
