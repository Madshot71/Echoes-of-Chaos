using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static bool isClosestTo<T>(this Vector3 input, T[] values, Vector3 target, Func<T, Vector3> selector) where T : Component
    {
        return values.OrderBy(v => Vector3.Distance(selector(v), target)).First().transform.position == input;
    }

    public static T Closest<T>(this List<T> array, Vector3 target , Func<T , Vector3> selector) where T : Component
    {
        return array.Where(v => selector(v) != Vector3.zero)
            .OrderBy(v => Vector3.Distance(selector(v), target))
            .First();
    }
    public static float DivideBy(this float value, float by)
    {
        if (value == 0 || by == 0)
        {
            return 0;
        }

        return value / by;
    }
    
    public static float DivideBy(this int value, int by)
    {
        if (value == 0 || by == 0)
        {
            return 0;
        }

        return (float)value / (float)by;
    }
}
