using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace GhostBoy.Mission
{
    public class WayPointMission : Mission, IMission<Transform>
    {
        public float distance { get; private set; }
        [field : SerializeField] public List<Transform> Targets { get; set; } = new List<Transform>();
        public Transform measure
        {
            get
            {
                return character.controller.transform;
            }
            set
            {
                if (value is not Transform)
                {
                    value = character.transform;
                }
            }

        }
        public float Margin;

        public void LateUpdate()
        {
            if (index >= Targets.Count)
            {
                isDone = true;
                OnComplete?.Invoke();
                return;
            }
        }

        public override string Info()
        {
            return $"{info} :\n {distance}";
        }

        public override bool RunCondition()
        {
            return started == true && character.controller.hitBox.Alive();
        }

        public override void StartMission()
        {
            started = true;
            OnStart?.Invoke();
        }

        public override int Count() => Targets.Count;

        public override void Execution()
        {
            if(started == false || isDone)
                return;
            
            if(started && isDone == false)
            {
                distance = Vector3.Distance(Targets[index].position, measure.position);
            }

            if (distance < Margin)
            {
                index++;
            }

        }
    }
}
