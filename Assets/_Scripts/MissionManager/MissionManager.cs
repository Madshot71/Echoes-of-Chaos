using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GhostBoy.Mission
{
    public class MissionManager : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] public List<Mission> Missions = new List<Mission>();
        private int index;

        public UnityEvent OnStart;
        public UnityEvent OnComplete;


        private bool begin;
        private bool Completed;

        private void Awake()
        {
            foreach (var item in Missions)
            {
                if (item != null)
                {
                    item.character = player;
                }
            }
        }
        
        private void Start()
        {
            Begin();
        }

        private void LateUpdate()
        {
            if (begin == false)
            {
                return;
            }

            if (Missions[index].isDone)
            {
                index++;

                if (index >= Missions.Count)
                {
                    Complete();
                }
                return;
            }
            
            //StartMission
            if (Missions[index].started == false)
            {
                Missions[index].StartMission();
            }

        }
        
        private void Complete()
        {
            OnComplete?.Invoke();
            begin = false;
        }

        public void Begin()
        {
            OnStart?.Invoke();
            begin = true;
        }
    }
}