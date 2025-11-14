using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GhostBoy.Mission
{
    public abstract class Mission : MonoBehaviour 
    {
        public CharacterBase character { get; set; }

        [field : SerializeField] public string missionName {get; protected set;}
        [SerializeField][TextArea(3 , 7)]protected string info;
        public int index { get; protected set; } = 0;
        public bool started { get; protected set; }
        public bool isDone { get; protected set; } = false;

        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        public void Update()
        {
            if (RunCondition() == false || started == false)
            {
                return;
            }
            
            Execution();
        }

        /// <summary>
        /// The number of Targets
        /// </summary>
        /// <returns></returns>
        public abstract int Count();
        public abstract bool RunCondition();
        public virtual string Info()
        {
            return info;
        }

        public abstract void StartMission();
        public abstract void Execution();


        public virtual T Selector<T>(Func<CharacterBase , T> select)
        {
            return select(character);
        }

    }
}