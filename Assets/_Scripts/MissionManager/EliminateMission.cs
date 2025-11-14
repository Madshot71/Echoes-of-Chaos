using System.Collections.Generic;
using UnityEngine;

namespace GhostBoy.Mission
{
    public class EliminateMission : Mission, IMission<GameObject>
    {
        [field: SerializeField] public List<GameObject> Targets { get; set; } = new List<GameObject>();
        
        private List<HitBox> targetHitBoxes = new List<HitBox>();

        public GameObject measure
        {
            get
            {
                return character.controller.gameObject;
            }
            set
            {
                if (value is not GameObject)
                {
                    value = character.gameObject;
                }
            }
        }
        
        private int eliminatedCount = 0;

        public void Start()
        {
            // Cache HitBox components from targets
            foreach (var target in Targets)
            {
                if (target.TryGetComponent<HitBox>(out var hitBox))
                {
                    targetHitBoxes.Add(hitBox);
                    // Subscribe to death event
                    hitBox.OnDeath.AddListener(OnTargetEliminated);
                }
            }
        }

        public override string Info()
        {
            return $"{info}:\nEliminated: {eliminatedCount}/{Targets.Count}";
        }

        public override bool RunCondition()
        {
            return started && character.controller.hitBox.Alive();
        }

        public override void StartMission()
        {
            started = true;
            OnStart?.Invoke();
        }

        public override int Count() => Targets.Count;

        public override void Execution()
        {
            if (!started || isDone) return;

            // Check if all targets are eliminated
            if (eliminatedCount >= Targets.Count)
            {
                isDone = true;
                OnComplete?.Invoke();
            }
        }

        private void OnTargetEliminated()
        {
            eliminatedCount++;
        }

        private void OnDestroy()
        {
            // Cleanup event subscriptions
            foreach (var hitBox in targetHitBoxes)
            {
                if (hitBox != null)
                {
                    hitBox.OnDeath.RemoveListener(OnTargetEliminated);
                }
            }
        }
    }
}