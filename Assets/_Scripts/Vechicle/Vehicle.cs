using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Vehicle : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    public abstract string EnterAnimation();

    public void OnValidate()
    {
        _rigidbody ??= GetComponent<Rigidbody>();
    }
}
