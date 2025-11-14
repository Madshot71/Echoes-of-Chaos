using System;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    public TrailRenderer trail { get; private set; }
    internal Vector3 startPos;
    internal Vector3 startDirection;

    private void OnValidate()
    {
        trail ??= GetComponent<TrailRenderer>();
    }

    private void Awake()
    {
        trail ??= GetComponent<TrailRenderer>();
    }
}