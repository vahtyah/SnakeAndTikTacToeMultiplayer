// using ExitGames.Client.Photon;
// using Photon.Pun;
// using Photon.Realtime;
// using Sirenix.OdinInspector;

using Unity.Netcode;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else Destroy(gameObject);
    }

    protected virtual void OnApplicationQuit() { Destroy(_instance); }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}

// public abstract class SingletonPunCallbacks<T> : MonoBehaviourPunCallbacks where T : SingletonPunCallbacks<T>
// {
//     private static T _instance;
//     public static T Instance => _instance;
//
//     protected virtual void Awake()
//     {
//         if (_instance == null) _instance = this as T;
//         else Destroy(gameObject);
//     }
//
//     protected virtual void OnApplicationQuit() { Destroy(_instance); }
//
//     protected virtual void OnDestroy()
//     {
//         if (_instance == this) _instance = null;
//     }
// }

// public abstract class SerializedSingleton<T> : SerializedMonoBehaviour where T : SerializedSingleton<T>
// {
//     private static T _instance;
//     public static T Instance => _instance;
//
//     protected virtual void Awake()
//     {
//         if (_instance == null) _instance = this as T;
//         else Destroy(gameObject);
//     }
//
//     protected virtual void OnApplicationQuit() { Destroy(_instance); }
//
//     protected virtual void OnDestroy()
//     {
//         if (_instance == this) _instance = null;
//     }
// }

public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if(transform.parent != null) transform.SetParent(null);
        if (_instance == null) _instance = this as T;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit() { Destroy(_instance); }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}

public abstract class SingletonNetwork<T> : NetworkBehaviour where T : SingletonNetwork<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else Destroy(gameObject);
    }

    protected virtual void OnApplicationQuit() { Destroy(_instance); }

    public override void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}

// public abstract class PersistentSingletonPunCallbacks<T> : MonoBehaviourPunCallbacks where T : PersistentSingletonPunCallbacks<T>
// {
//     private static T _instance;
//     public static T Instance => _instance;
//
//     protected virtual void Awake()
//     {
//         if(transform.parent != null) transform.SetParent(null);
//         if (_instance == null) _instance = this as T;
//         else Destroy(gameObject);
//         DontDestroyOnLoad(gameObject);
//     }
//
//     protected virtual void OnApplicationQuit() { Destroy(_instance); }
//
//     protected virtual void OnDestroy()
//     {
//         if (_instance == this) _instance = null;
//     }
// }