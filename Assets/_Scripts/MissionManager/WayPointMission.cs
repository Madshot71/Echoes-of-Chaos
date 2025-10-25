using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class WayPointMission : MissionManager<PlayerController>
{
    
}

public class Point : Mission<Transform>
{
    public void Update()
    {

    }

    public override void Execution()
    {
        if(!CompareTargets<Vector3>(i => i.position , measure))
        {
            return;
        }
    }
}