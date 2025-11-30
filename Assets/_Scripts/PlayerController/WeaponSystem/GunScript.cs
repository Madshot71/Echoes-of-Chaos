using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace GhostBoy
{
    [DisallowMultipleComponent]
    public class GunScript : Weapon
    {
        [SerializeField] private LayerMask mask;
        private GunConfig config => _config as GunConfig;
        [SerializeField] private Transform muzzle;

        //Shooting
        public float currentReloadTime {get; private set;}
        private float nextShootTime;

        //Pooling && tracking
        [SerializeField] private List<Bullet> activeBullets = new List<Bullet>();
        private ObjectPool<Bullet> pool;

        //VFX
        [SerializeField] private VisualEffect muzzleFlash;

        //Jobs
        NativeArray<BulletData> bullets;
        NativeArray<RaycastCommand> raycasts;
        NativeArray<RaycastHit> results;

        private bool isReloading => currentReloadTime < config.reloadTime;

        private void Awake()
        {
            currentAmmo = config.clipSize;

            //Creating the bullet pool
            pool = new ObjectPool<Bullet>(
                () => Instantiate(config.bullet.Prefab, transform),
                actionOnGet: bullet =>
                {
                    //Setting the bullets positon and rotation to the muzzles
                    bullet.transform.SetParent(null);
                    bullet.transform.position = muzzle.position;
                    bullet.transform.forward = muzzle.forward;
                    bullet.Init();
                    bullet.gameObject.SetActive(true);
                    activeBullets.Add(bullet);
                },
                bullet =>
                {
                    bullet.gameObject.SetActive(false);
                    bullet.transform.SetParent(transform);
                    bullet.transform.localPosition = Vector3.zero;
                    bullet.transform.localRotation = Quaternion.identity;
                    activeBullets.Remove(bullet);
                },
                bullet =>
                {
                    Destroy(bullet.gameObject);
                },
                true, 50, 100
            );
        }

        private void Update()
        {
            if (isReloading)
            {
                currentReloadTime += Time.deltaTime;

                if (currentReloadTime >= config.reloadTime)
                {
                    currentAmmo = config.clipSize;
                }
            }
        }

        private void FixedUpdate() {
            Execute();
        }

        internal override void Attack()
        {
            if(CanShoot() == false)
            {
                return;
            }

            nextShootTime = Time.time + (1 / config.fireRate);
            currentAmmo--;
            StartCoroutine(Shoot());
        }

        internal override void Reload()
        {
            if(isReloading == false)
                ReloadAmmo();
        }

        private bool CanShoot()
        {
            if(Time.time < nextShootTime)
            {
                return false;
            }

            if(isReloading) 
            {
                return false;
            }

            if(currentAmmo <= 0)
            {
                Reload();
                return false;
            }

            return true;
        }

        internal override float ReloadProgress()
        {
            return currentReloadTime.DivideBy(config.reloadTime);
        }

        internal override int MaxAmmo()
        {
            return config.maxBullets;
        }
        
        private void ReloadAmmo()
        {
            currentReloadTime = 0;
        }
        
        private IEnumerator Shoot()
        {
            // Get bullet from pool;
            var bullet = pool.Get();

            yield return null;

            bullet.Shoot();

            //play Aucio
            PlayAudio(config.shootSFX);

            //VFX
            muzzleFlash?.Play();
        }

        #region Jobs

        private void Execute()
        {
            if(activeBullets.Count <= 0)
            {
                return;
            }

            bullets = new NativeArray<BulletData>(activeBullets.Count , Allocator.TempJob);

            for (int i = 0; i < activeBullets.Count; i++)
            {
                var bullet = activeBullets[i];
                bullets[i] = new BulletData()
                {
                    // Applying data 
                    currentTime = bullet.currentTime,
                    startPos = bullet.startPos,
                    startDir = bullet.startDirection,
                    startTime = bullet.startTime,
                    active = bullet.active
                };
            }

            var gunjob = new GunScriptJob()
            {
                time = Time.time,
                fixedDeltaTime = Time.fixedDeltaTime,
                gravity = config.bullet.gravity, 
                speed = config.bullet.Speed,
                bullets = this.bullets
            };

            var job = gunjob.Schedule(activeBullets.Count , 64);
            job.Complete();

            for (int i = 0; i < activeBullets.Count; i++)
            {
               var bullet = activeBullets[i];
               var data = bullets[i];

               bullet.transform.position = data.position;
               bullet.currentTime = data.currentTime;
            }

            RunRaycasts();

            bullets.Dispose();
        }

        private void RunRaycasts()
        {
            for (int i = 0; i < bullets.Count(); i++)
            {
                var data = bullets[i];

                //Release bullet if it reach its max distance
                if(Vector3.Distance(data.startPos , data.position) > config.bullet.distance)
                {
                    ReleaseBullet(activeBullets[i]);
                }

                if(Raycast(data.prevPosition , data.position , out RaycastHit hit))
                {
                    CheckHit(hit , i);
                    continue;
                }
                else if(Raycast(data.position , data.nextPosition , out hit))
                {
                    CheckHit(hit , i);
                    continue;
                }
            }
        }

        private void CheckHit(RaycastHit hit , int index)
        {
            if(hit.collider == null)
            {
                return;
            }

            Debug.Log($"{hit.collider} hit something");
            //Removing bullet from active list because of hit
            ReleaseBullet(activeBullets[index]);

            if(hit.collider.TryGetComponent<HitBox>(out HitBox hitbox))
            {
                hitbox.TakeDamage(config.bullet.Damage);
            }
        }

        private void ReleaseBullet(Bullet bullet)
        {
            pool.Release(bullet);
        }

        private bool Raycast(Vector3 position , Vector3 endPoint , out RaycastHit hit)
        {
            return Physics.Raycast(position , position - endPoint , out hit, Vector3.Distance(position , endPoint) , mask);
        }

        #endregion
    }

    [BurstCompile]
    public struct GunScriptJob : IJobParallelFor
    { 
        [ReadOnly] public float time;
        [ReadOnly] public float fixedDeltaTime;
        [ReadOnly] public float gravity;
        [ReadOnly] public float speed;
        [ReadOnly] public float3 down;

        public NativeArray<BulletData> bullets;

        public void Execute(int index)
        {
            var bullet = bullets[index];
            
            if(bullet.active == false)
            {
                return;
            }

            if(bullet.startTime < 0)
            {
                bullet.startTime = time;
            }

            bullet.currentTime = time - bullet.startTime;
            float nextTime = bullet.currentTime + fixedDeltaTime;
            float prevTime  = bullet.currentTime - fixedDeltaTime;

            //Setting the position in the bullet
            bullet.position = FindPointOnParabola(bullet.currentTime , bullet);
            bullet.nextPosition = FindPointOnParabola(nextTime , bullet);
            bullet.prevPosition = bullet.position;

            if(prevTime > 0)
            {
                bullet.prevPosition = FindPointOnParabola(prevTime , bullet);
            }

            // passing back the values
            bullets[index] = bullet;
        }

        private float3 FindPointOnParabola(float _time , BulletData bullet)
        {
            down = new float3(0 , -0.001f, 0);
            float3 point = bullet.startPos + (bullet.startDir * speed * _time);
            float3 gravityVec = down * gravity * _time * _time;
            return point + gravityVec;
        }
    }

    [BurstCompile]
    public struct BulletData
    {
        //Time
        public float currentTime;

        //positions
        public float3 position;
        public float3 nextPosition;
        public float3 prevPosition;

        //Init
        public float3 startPos;
        public float3 startDir;
        public float startTime;
        public bool active;

    }

}

