using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePool : ObjectPool<Vehicle>{}

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    public Pool<T>[] ObjectPools;

    void Awake()
    {
        foreach(Pool<T> pool in ObjectPools)
        {
            for(int i = pool.pool.Count; i < pool.startAmount; i++)
            {
                GameObject obj = Instantiate(pool.Object.gameObject, transform);
                obj.SetActive(false);
                pool.pool.Add(obj.GetComponent<T>());
            }
        }
    }

    public T GetObject(int index)
    {
        foreach(T tObj in ObjectPools[index].pool)
        {
            if(!tObj.gameObject.activeInHierarchy)
            {
                tObj.gameObject.SetActive(true);
                return tObj;
            }
        }
        GameObject obj = Instantiate(ObjectPools[index].Object.gameObject, transform);
        T t = obj.GetComponent<T>();
        ObjectPools[index].pool.Add(t);
        return t;
    }
}

[System.Serializable]
public class Pool<T> where T : Component
{
    public List<T> pool = new List<T>();
    public T Object;
    public int startAmount;
}