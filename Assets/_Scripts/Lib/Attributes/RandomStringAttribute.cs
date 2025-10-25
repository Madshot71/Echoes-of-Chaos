using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStringAttribute : PropertyAttribute
{
    public float Length;

    public RandomStringAttribute(int Length)
    {
        this.Length = Length;
    } 

}
