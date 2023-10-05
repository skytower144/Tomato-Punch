using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum QuestList { Unassigned, Assigned }
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; private set; }
    [SerializeField] private List<Quest> assignedQuests = new List<Quest>();
    [SerializeField] private List<Quest> unassignedQuests = new List<Quest>();

    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public void AddQuest(string quest_id)
    {
        Quest newQuest = FindQuest(quest_id, QuestList.Unassigned);
        Quest cache = newQuest;

        if (newQuest == null) {
            Debug.LogError($"Quest : {quest_id} Not Found.");
            return;
        }
        unassignedQuests.Remove(cache);
        
        newQuest.InitQuestGoals();
        assignedQuests.Add(newQuest);
    }

    public Quest FindQuest(string quest_id, QuestList questType)
    {
        List<Quest> quests = (questType == QuestList.Assigned) ? assignedQuests : unassignedQuests;
        foreach (Quest quest in quests) {
            if (quest.QuestID == quest_id)
                return quest;
        }
        return null;
    }

    public List<Quest> ReturnQuestState(bool isAssignedQuest)
    {
        return (isAssignedQuest ? assignedQuests : unassignedQuests);
    }

    public void UpdateQuestState(List<Quest> assigned, List<Quest> unassigned)
    {
        // Recover Quest State
        assignedQuests = assigned;
        unassignedQuests = unassigned;
        DeSerializeQuests();
        
        foreach (Quest quest in assignedQuests)
            if (!quest.is_completed) quest.InitQuestGoals();
    }

    public void SerializeQuests()
    {
        List<Quest> quests;
        for (int i = 0; i < 2; i++) {
            quests = i > 0 ? assignedQuests : unassignedQuests;
            
            foreach (Quest quest in quests) {
                foreach (CarryGoal goal in quest.carryGoals)
                    goal.SerializeItemName();
                
                foreach (DefeatGoal goal in quest.defeatGoals)
                    goal.SerializeEnemyName();
                
                foreach (ItemQuantity itemQuantity in quest.reward.items)
                    itemQuantity.SerializeItemName();
            }
        }
    }

    private void DeSerializeQuests()
    {
        List<Quest> quests;
        for (int i = 0; i < 2; i++) {
            quests = i > 0 ? assignedQuests : unassignedQuests;
            
            foreach (Quest quest in quests) {
                foreach (CarryGoal goal in quest.carryGoals)
                    goal.DeSerializeItemName();
                
                foreach (DefeatGoal goal in quest.defeatGoals)
                    goal.DeSerializeEnemyName();
                
                foreach (ItemQuantity itemQuantity in quest.reward.items)
                    itemQuantity.DeSerializeItemName();
            }
        }
    }
}