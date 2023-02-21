using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; private set; }
    [SerializeField] private List<Quest> assignedQuests = new List<Quest>();

    void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public void AddQuest(Quest newQuest)
    {
        if (newQuest != null)
            assignedQuests.Add(newQuest);
    }

    public Quest FindQuest(string quest_id)
    {
        foreach (Quest quest in assignedQuests) {
            if (quest.QuestName == quest_id)
                return quest;
        }
        return null;
    }
}
