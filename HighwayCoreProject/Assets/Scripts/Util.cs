using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VariablePool<T>
{
    public WeightedVar<T>[] Pool;
    float totalWeight;

    public WeightedVar<T> GetRandomVar()
    {
        if(totalWeight == 0f)
        {
            foreach(WeightedVar<T> var in Pool)
            {
                totalWeight += var.weight;
            }
        }

        float random = Random.Range(0f, totalWeight);
        foreach(WeightedVar<T> var in Pool)
        {
            if(random <= var.weight)
                return var;

            random -= var.weight;
        }
        return null;
    }
}

[System.Serializable]
public class WeightedVar<T>
{
    public float weight = 1f;
    public T variable;

    public WeightedVar(float _weight, T var)
    {
        weight = _weight;
        variable = var;
    }

    public static implicit operator T(WeightedVar<T> a) => a.variable;
}