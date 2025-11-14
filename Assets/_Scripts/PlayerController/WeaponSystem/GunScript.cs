using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace GhostBoy
{

    [DisallowMultipleComponent]
    public class GunScript : Weapon
    {
        [SerializeField] private LayerMask mask;
        [SerializeField] private GunConfig config;
        [SerializeField] private Transform muzzle;

        //Shooting
        private float nextShootTime;

        //Pooling && tracking
        private List<Bullet> activeBullets = new List<Bullet>();
        private ObjectPool<Bullet> pool;

        //Jobs
        NativeArray<BulletData> bullets;
        NativeArray<RaycastCommand> commands;
        NativeArray<RaycastHit> hits;


        private void Awake()
        {
            _config = config;
            //Creating the bullet pool
            pool = new ObjectPool<Bullet>(
                () => Instantiate(config.bullet.Prefab, transform),
                actionOnGet: bullet =>
                {
                    //Setting the bullets positon and rotation to the muzzles
                    bullet.transform.SetParent(null);
                    bullet.transform.position = muzzle.position;
                    bullet.transform.forward = muzzle.forward;
                    activeBullets.Add(bullet);
                },
                bullet =>
                {
                    bullet.transform.SetParent(transform);
                    bullet.transform.position = Vector3.zero;
                    bullet.transform.rotation = Quaternion.identity;
                    activeBullets.Remove(bullet);
                },
                bullet =>
                {
                    Destroy(bullet.gameObject);
                },
                true, 50, 100
            );

            //Loading the Pool with instances of bullets
            for (int i = 0; i < config.maxBullets; i++)
            {
                var bullet = pool.Get();
                pool.Release(bullet);
            }
        }


        #region Shooting
        internal override void Attack()
        {
            if (Time.time > nextShootTime)
            {
                return;
            }

            nextShootTime = Time.time + (1 / config.fireRate);

            StartCoroutine(Shoot());
        }

        internal override void Reload()
        {

        }

        IEnumerator Shoot()
        {
            var bullet = pool.Get();

            yield return null;
            bullet.trail.enabled = true;
            PlayAudio(config.shootSFX);
        }

        #endregion


        #region Burst / Jobs

        private void FixedUpdate()
        {
            //Run job
            RunJobs();
        }

        private void LateUpdate()
        {
            //Clean Up at the end 
            Dispose();
        }


        private void RunJobs()
        {
            if (activeBullets.Count <= 0)
            {
                return;
            }

            bullets = new NativeArray<BulletData>(activeBullets.Count, Allocator.TempJob);

            for (int i = 0; i < activeBullets.Count; i++)
            {
                var bullet = activeBullets[i];
                bullets[i] = new BulletData()
                {
                    startPos = bullet.startPos,
                    direction = bullet.startDirection,
                    speed = config.bullet.Speed,
                    gravity = config.bullet.gravity,
                };
            }

            GunJobUpdate gunJob = new GunJobUpdate()
            {
                time = Time.time,
                fixedDeltaTime = Time.fixedDeltaTime,
                bullets = this.bullets
            };

            //Run job 
            JobHandle job = gunJob.Schedule(activeBullets.Count, 16);
            job.Complete();

            //Update positions
            UpdatePositions();
            RayCasts();
            
        }
            
        private void UpdatePositions()
        {
            for(int i = 0; i < activeBullets.Count; i++)
            {
                activeBullets[i].transform.position = bullets[i].position;
            }
        }

        private void RayCasts()
        {
            var casts = new RaycastCommand[activeBullets.Count * 2];

            for (int i = 0; i < activeBullets.Count; i++)
            {
                var bullet = bullets[i];

                //Check from prev to current position
                casts[2 * i] = RayCast(bullet.prev_Pos, bullet.position);

                //check from current to next position
                casts[2* i + 1] = RayCast(bullet.position, bullet.next_Pos);
            }

            commands = new NativeArray<RaycastCommand>(activeBullets.Count * 2, Allocator.TempJob);
            JobHandle job = RaycastCommand.ScheduleBatch(commands, hits, 4);
            job.Complete();

            RaycastHits();
        }

        private void RaycastHits()
        {
            for (int i = 0; i < activeBullets.Count; i++)
            {
                //2x + 1 = n;
                int n = 2 * i + 1;
                int x = i * 2;

                RaycastHit hit1 = hits[x], hit2 = hits[n];

                if (CheckHit(hit1))
                {
                    if (hit1.collider.TryGetComponent<HitBox>(out HitBox hitbox))
                    {
                        hitbox.TakeDamage(config.bullet.Damage);
                    }
                    ReleaseBullet(activeBullets[i]);
                    continue;
                }

                if (CheckHit(hit2))
                {
                    if (hit2.collider.TryGetComponent<HitBox>(out HitBox hitbox))
                    {
                        hitbox.TakeDamage(config.bullet.Damage);
                    }

                    ReleaseBullet(activeBullets[i]);
                    continue;
                }
            }
        }

        private void ReleaseBullet(Bullet bullet)
        {
            pool.Release(bullet);
        }

        private bool CheckHit(RaycastHit hit)
        {
            if (hit.collider == null)
            {
                return false;
            }

            return true;
        }


        private void Dispose()
        {
            if (activeBullets.Count <= 0)
            {
                return;
            }

            bullets.Dispose();
            commands.Dispose();
            hits.Dispose();
        }
        #endregion

        #region Internal Helper Functions
        
        private void PlayAudio(AudioClip clip)
        {
            if(audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        private RaycastCommand RayCast(Vector3 position, Vector3 endPoint)
        {
            return new RaycastCommand(position, position - endPoint, Vector3.Distance(position, endPoint), mask);
        }
        #endregion


        [BurstCompile]
        public struct BulletData
        {
            //Time
            public float currentTime { get; private set; }

            //Position
            public float3 position { get; private set; }
            public float3 next_Pos;
            public float3 prev_Pos;

            //Init
            public float3 startPos;
            public float3 direction;
            public float speed;
            public float gravity;


            public void Update(float time, float fixedDeltaTime)
            {
                currentTime = time + fixedDeltaTime;
                float nextTime = currentTime + fixedDeltaTime;
                float prevTime = currentTime - fixedDeltaTime;

                position = FindPointOnPorabla(currentTime);
                next_Pos = FindPointOnPorabla(nextTime);

                if (prevTime > 0)
                {
                    prev_Pos = FindPointOnPorabla(prevTime);
                }
            }

            private float3 FindPointOnPorabla(float time)
            {
                float3 point = startPos + (direction * speed * time);
                float3 gravityVec = new float3(0 , -1 ,0) * gravity * time * time;
                return point + gravityVec;
            }
        }

        [BurstCompile]
        public struct GunJobUpdate : IJobParallelFor
        {
            [ReadOnly] public float time;
            [ReadOnly] public float fixedDeltaTime;
            public NativeArray<BulletData> bullets;

            public void Execute(int index)
            {
                bullets[index].Update(time, fixedDeltaTime);
            }
        }
    }
}