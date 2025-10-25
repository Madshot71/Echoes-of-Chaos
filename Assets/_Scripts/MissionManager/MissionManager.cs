using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class MissionManager<T> : MonoBehaviour
{
        
}

[System.Serializable]
public abstract class Mission<T>
{
    public T measure;
    public List<Target<T>> Targets = new List<Target<T>>();
    protected int index = 0;

    public abstract void Execution();

    protected bool CompareTargets<A>(Func<T, A> selector , T measure)
    {
        if (!selector(Targets[index].value).Equals(selector(this.measure)))
        {
            return true;
        }
        return false;
    }
}

public class Target<T>
{
    public T value;
}