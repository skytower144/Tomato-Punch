using System.Collections;
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

    public void AddQuest(string quest_id, bool isCompleted = false)
    {
        Quest newQuest = FindQuest(quest_id, QuestList.Unassigned);
        Quest cache = newQuest;

        if (newQuest == null) {
            Debug.LogError($"Quest : {quest_id} Not Found.");
            return;
        }
        newQuest.UpdateQuestCompletion(isCompleted);
        assignedQuests.Add(newQuest);
        unassignedQuests.Remove(cache);
    }

    public Quest FindQuest(string quest_id, QuestList questType)
    {
        List<Quest> quests = (questType == QuestList.Assigned) ? assignedQuests : unassignedQuests;
        foreach (Quest quest in quests) {
            if (quest.QuestName == quest_id)
                return quest;
        }
        return null;
    }

    public List<QuestData> ReturnAssignedQuests()
    {
        List<QuestData> questDataList = new List<QuestData>();
        foreach (Quest quest in assignedQuests) {
            QuestData bundle = new QuestData();
            bundle.questName = quest.QuestName;
            bundle.questCompleted = quest.is_completed;
            questDataList.Add(bundle);
        }
        return questDataList;
    }

    public void UpdateQuestState(List<QuestData> questDataList)
    {
        // Reset All Quests
        List<Quest> allQuests = new List<Quest>();
        if (assignedQuests != null)
            allQuests.AddRange(assignedQuests);
        
        if (unassignedQuests != null)
            allQuests.AddRange(unassignedQuests);
        
        assignedQuests.Clear();
        unassignedQuests.Clear();
        unassignedQuests = allQuests;
        
        // Recover Quest State
        foreach (QuestData bundle in questDataList) {
            AddQuest(bundle.questName, bundle.questCompleted);
        }
    }
}