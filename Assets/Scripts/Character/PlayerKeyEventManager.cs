using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyEventManager : MonoBehaviour
{
    [SerializeField] private List<PlayerKeyEvent> playerKeyevents;
    private PlayerKeyEvent[] cacheKeyEvents = new PlayerKeyEvent[2];

    private PlayerKeyEvent ConvertStringToEnum(string name)
    {
        if (name == "_") return PlayerKeyEvent.None;
        
        return (PlayerKeyEvent)Enum.Parse(typeof(PlayerKeyEvent), name);
    }

    public void AddKeyEvent(PlayerKeyEvent keyEvent)
    {
        if (keyEvent is PlayerKeyEvent.None) {
            // Do nothing
        }
        else if (HasKeyEvent(keyEvent))
            GameManager.DoDebug($"=== {keyEvent} : Keyevent already exists. ===");
        else
            playerKeyevents.Add(keyEvent);

        ClearCacheKeyEvents();
    }

    public void AddKeyEvent(string name)
    {
        PlayerKeyEvent keyEvent = ConvertStringToEnum(name);
        AddKeyEvent(keyEvent);
    }

    private bool HasKeyEvent(PlayerKeyEvent keyEvent)
    {
        return playerKeyevents.Contains(keyEvent);
    }
    public void ApplyGlobalKeyEvent(string name)
    {
        PlayerKeyEvent targetEvent = ConvertStringToEnum(name);

        foreach (ProgressAssistant assistant in ProgressManager.instance.assistants.Values) {
            foreach (ObjectProgress target in assistant.objectProgressList) {
                List<KeyEventProgressData> keyEventProgressList = target.ReturnKeyEventProgressList();

                foreach (KeyEventProgressData bundle in keyEventProgressList) {
                    if (targetEvent == bundle.KeyEvent) {
                        target.ApplyKeyEvent(bundle);
                        keyEventProgressList.Remove(bundle);
                        break;
                    }
                }
            }
        }
    }
    public void CheckProgressKeyEvent(ObjectProgress target)
    {
        List<KeyEventProgressData> keyEventProgressList = target.ReturnKeyEventProgressList();

        foreach (KeyEventProgressData bundle in keyEventProgressList) {
            if (HasKeyEvent(bundle.KeyEvent)) {
                target.ApplyKeyEvent(bundle);
                keyEventProgressList.Remove(bundle);
                return;
            }
            else if (bundle.KeyEvent is PlayerKeyEvent.None) {
                Debug.LogError($"{target.ReturnID()} : is holding a None PlayerKeyEvent");
                return;
            }
        }
    }
    public List<PlayerKeyEvent> ReturnPlayerKeyEvents()
    {
        return playerKeyevents;
    }

    public void RestorePlayerKeyEvents(List<PlayerKeyEvent> keyEvents)
    {
        playerKeyevents = keyEvents;
    }

    public void CacheKeyEvents(string tagValue)
    {
        string[] tagBundle = tagValue.Split('@');

        if (tagBundle.Length == 0 || tagBundle.Length > 2) {
            Debug.LogError($"Incorrect info : {tagBundle}");
            return;
        }
        for (int i = 0; i < tagBundle.Length; i++)
            cacheKeyEvents[i] = ConvertStringToEnum(tagBundle[i]);
    }

    public void ApplyCacheKeyEvents(bool is_victory)
    {
        // Update Player Key Events depending on the battle outcome
        int result = is_victory ? 0 : 1;

        PlayerKeyEvent outcomeKeyEvent = cacheKeyEvents[result];  // [WIN, LOSE]
        AddKeyEvent(outcomeKeyEvent);
    }

    private void ClearCacheKeyEvents()
    {
        Array.Clear(cacheKeyEvents, 0, cacheKeyEvents.Length);
    }
}
// Utilize this if an event needs to change multiple npcs' dialogues.
public enum PlayerKeyEvent
{
    None,           Win_Rupple_StartingPoint,   Lose_Rupple_StartingPoint,         Find_BabyCat_StartingPoint, Win_Number2,
    Lose_Number2,   Win_Donut_StartingPoint,    Scolded_Friend_CompanySecondFloor
}
