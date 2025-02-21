using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventRegistry
{
    private static Dictionary<string, Action<GameObject>> eventTable = new Dictionary<string, Action<GameObject>>();

    public static void AddEvent(string eventName, Action<GameObject> listener)
    {
        if (!eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] = listener;
        }
        else
        {
            eventTable[eventName] += listener;
        }
    }

    public static void RemoveEvent(string eventName, Action<GameObject> listener)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] -= listener;
            if (eventTable[eventName] == null)
            {
                eventTable.Remove(eventName);
            }
        }
    }

    public static void SendEvent(string eventName, GameObject obj)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName]?.Invoke(obj);
        }
    }
}
