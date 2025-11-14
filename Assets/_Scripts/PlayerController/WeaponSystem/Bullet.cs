using System;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    public TrailRenderer trail { get; private set; }
    public Vector3 startPos { get; private set;}
    public Vector3 startDirection { get; private set;}

    private void OnValidate()
    {
        trail ??= GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        startPos = transform.position;
        startDirection = transform.forward;
    }

    private void Awake()
    {
        trail ??= GetComponent<TrailRenderer>();
    }
}