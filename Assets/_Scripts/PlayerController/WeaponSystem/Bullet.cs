using System;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    internal TrailRenderer trail;
    internal Vector3 startPos;
    internal Vector3 startDirection;
    internal float currentTime;
    internal float startTime;
    internal bool active = false;

    private void OnValidate()
    {
        trail ??= GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        trail.enabled = false;    
        startTime = -1;
        active = false;
    }

    public void Init()
    {
        startPos = transform.position;
        startDirection = transform.forward;
        currentTime = 0;
        startTime = Time.time;
        active = true;
    }

    internal void Shoot()
    {
        trail.enabled = true;
    }


    private void Awake()
    {
        trail ??= GetComponent<TrailRenderer>();
    }
}