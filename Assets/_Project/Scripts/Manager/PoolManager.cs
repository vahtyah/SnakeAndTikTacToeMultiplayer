using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}

public class Pool
{
    private static PoolManager _manager;
    private readonly Queue<GameObject> availableObjects = new Queue<GameObject>();
    private Transform parentTransform;

    public static Pool Register(GameObject prefab, Transform parentTransform = null, int initialSize = 0)
    {
        EnsureManagerExists();
        parentTransform ??= _manager.transform;
        var pool = new Pool
        {
            parentTransform = parentTransform
        };
        for (var i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(prefab, parentTransform);
            obj.SetActive(false);
            pool.availableObjects.Enqueue(obj);
            _manager.TrackObject(obj, pool);
        }

        return _manager.RegisterPool(prefab, pool);
    }


    public static GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Spawn(GameObject prefab, Transform parent)
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation = default,
        Transform parent = null)
    {
        EnsureManagerExists();
        var pool = _manager.GetPool(prefab) ?? Register(prefab, parent);
        if (pool.availableObjects.Count > 0)
        {
            var obj = pool.availableObjects.Dequeue();
            _manager.TrackObject(obj, pool);
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        var newObj = Object.Instantiate(prefab, position, rotation, pool.parentTransform);
        _manager.TrackObject(newObj, pool);
        newObj.SetActive(true);
        return newObj;
    }

    public void Return(GameObject obj)
    {
        EnsureManagerExists();

        if (availableObjects.Contains(obj))
            return;

        _manager.UntrackObject(obj);
        obj.SetActive(false);
        availableObjects.Enqueue(obj);
    }

    public static void Despawn(GameObject obj)
    {
        EnsureManagerExists();

        var pool = _manager.UntrackObject(obj);
        if (pool != null)
        {
            obj.SetActive(false);
            pool.availableObjects.Enqueue(obj);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Attempted to return an object that doesn't belong to any pool.");
        }
    }
    

    private static void EnsureManagerExists()
    {
        if (_manager == null)
            _manager = PoolManager.Instance ?? new GameObject("PoolManager").AddComponent<PoolManager>();
    }
}

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<GameObject, Pool> pools = new();
    private readonly Dictionary<GameObject, Pool> trackedObjects = new();

    public Pool GetPool(GameObject prefab) => pools.GetValueOrDefault(prefab);

    public void TrackObject(GameObject obj, Pool pool) => trackedObjects[obj] = pool;

    public Pool UntrackObject(GameObject obj) =>
        !trackedObjects.Remove(obj, out var pool) ? null : pool;
    
    public List<Pool> UntrackAllObjects()
    {
        var poolList = new List<Pool>();
        foreach (var obj in trackedObjects.Keys)
        {
            poolList.Add(trackedObjects[obj]);
            trackedObjects.Remove(obj);
        }

        return poolList;
    }

    public Pool RegisterPool(GameObject prefab, Pool pool)
    {
        pools.TryAdd(prefab, pool);
        return pools[prefab];
    }
}