using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePool : MonoBehaviour
{
    public Pool<Vehicle>[] VehiclePools;

    void Awake()
    {
        foreach(Pool<Vehicle> pool in VehiclePools)
        {
            for(int i = pool.pool.Count; i < pool.startAmount; i++)
            {
                GameObject obj = Instantiate(pool.Object.gameObject, transform);
                obj.SetActive(false);
                pool.pool.Add(obj.GetComponent<Vehicle>());
            }
        }
    }

    public Vehicle GetVehicle(int index)
    {
        foreach(Vehicle vehicle in VehiclePools[index].pool)
        {
            if(!vehicle.gameObject.activeInHierarchy)
            {
                vehicle.gameObject.SetActive(true);
                return vehicle;
            }
        }
        GameObject obj = Instantiate(VehiclePools[index].Object.gameObject, transform);
        Vehicle vec = obj.GetComponent<Vehicle>();
        VehiclePools[index].pool.Add(vec);
        return vec;
    }
}

[System.Serializable]
public class Pool<T> where T : Component
{
    public List<T> pool = new List<T>();
    public T Object;
    public int startAmount;
}