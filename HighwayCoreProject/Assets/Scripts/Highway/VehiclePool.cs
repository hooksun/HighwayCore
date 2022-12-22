using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePool : ObjectPool<Vehicle>{}

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    static ObjectPool<T> instance;
    public Pool<T>[] ObjectPools;

    protected virtual void Awake()
    {
        instance = this;
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

    public static T GetObject(int index = 0, bool autoActivate = true) => instance.GetObj(index, autoActivate);
    T GetObj(int index, bool autoActivate)
    {
        foreach(T tObj in ObjectPools[index].pool)
        {
            if(!tObj.gameObject.activeInHierarchy)
            {
                tObj.gameObject.SetActive(autoActivate);
                return tObj;
            }
        }
        GameObject obj = Instantiate(ObjectPools[index].Object.gameObject, transform);
        T t = obj.GetComponent<T>();
        ObjectPools[index].pool.Add(t);
        t.gameObject.SetActive(autoActivate);
        return t;
    }

    public static void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.parent = instance.transform;
    }
}

[System.Serializable]
public class Pool<T> where T : Component
{
    [HideInInspector] public List<T> pool = new List<T>();
    public T Object;
    public int startAmount;
}