using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IEventListenerBase
{
};

public interface IEventListener<T> : IEventListenerBase
{
    void OnEvent(T e);
}

public interface ITransferData<T> : IEventListenerBase
{
    void SetData(T data);
}

public static class EventManger
{
    private static readonly Dictionary<Type, List<IEventListenerBase>> subscribers = new();

    public static void AddListener<T>(IEventListener<T> listener) where T : struct
    {
        Type eventType = typeof(T);
        if (!subscribers.TryGetValue(eventType, out var subs))
        {
            subs = new List<IEventListenerBase>();
            subscribers[eventType] = subs;
        }

        if (!subs.Contains(listener))
        {
            subs.Add(listener);
        }
    }

    public static void RemoveListener<T>(IEventListener<T> listener) where T : struct
    {
        Type eventType = typeof(T);
        if (!subscribers.TryGetValue(eventType, out var subs)) return;
        subs.Remove(listener);
        if (subs.Count == 0)
        {
            subscribers.Remove(eventType);
        }
    }

    private static bool SubscriptionExists<T>(Type eventType, IEventListener<T> listener) where T : struct
    {
        return subscribers[eventType].Contains(listener);
    }

    public static void TriggerEvent<T>(T e) where T : struct
    {
        Type eventType = typeof(T);
        if (!subscribers.TryGetValue(eventType, out var subs)) return;
        foreach (var listener in subs.ToList())
        {
            (listener as IEventListener<T>)?.OnEvent(e);
        }
    }
}

public static class EventRegister
{
    public static void StartListening<T>(this IEventListener<T> listener) where T : struct
    {
        EventManger.AddListener(listener);
    }

    public static void StopListening<T>(this IEventListener<T> listener) where T : struct
    {
        EventManger.RemoveListener(listener);
    }
}

// public enum GameEventType
// {
//     GameMainMenu,
//     GamePreStart,
//     GameStart,
//     GameOver,
//     GamePause,
// }
//
// public struct GameEvent
// {
//     public GameEventType EventType;
//     public GameObject Canvas;
//     
//     public GameEvent(GameEventType eventType, GameObject canvas)
//     {
//         EventType = eventType;
//         Canvas = canvas;
//     }
//     
//     public static void Trigger(GameEventType eventType, GameObject canvas)
//     {
//         EventManger.TriggerEvent(new GameEvent(eventType, canvas));
//     }
// }

//How to use:
//1. Create an event class that implements IEventListenerBase
//2. Create a listener class that implements IEventListener<T> where T is the event class
//3. Call EventRegister.StartListening(this) in the listener's OnEnable method
// void OnEnable()
// {
// 	this.StartListening<MMGameEvent>();
// }
//4. Call EventRegister.StopListening(this) in the listener's OnDisable method
// void OnDisable()
// {
// 	this.StopListening<MMGameEvent>();
// }
//5. Implement the EventListener interface for that event. For example :
// public void OnEvent(GameEvent gameEvent)
// {
// 	if (gameEvent.EventName == "GameOver")
//		{
//			// DO SOMETHING
//		}
// } 