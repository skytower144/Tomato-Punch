using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyEventManager : MonoBehaviour
{
    [SerializeField] private List<PlayerKeyEvent> playerKeyevents;

    public void AddKeyEvent(string name)
    {
        PlayerKeyEvent keyEvent = (PlayerKeyEvent)Enum.Parse(typeof(PlayerKeyEvent), name);
        if (HasKeyEvent(keyEvent)) return;

        playerKeyevents.Add(keyEvent);
    }

    public bool HasKeyEvent(PlayerKeyEvent keyEvent)
    {
        return playerKeyevents.Contains(keyEvent);
    }

    public List<PlayerKeyEvent> ReturnPlayerKeyEvents()
    {
        return playerKeyevents;
    }

    public void RestorePlayerKeyEvents(List<PlayerKeyEvent> keyEvents)
    {
        playerKeyevents = keyEvents;
    }
}

public enum PlayerKeyEvent
{
    Win_Rupple_StartingPoint,
    Win_Police_StartingPoint
}
