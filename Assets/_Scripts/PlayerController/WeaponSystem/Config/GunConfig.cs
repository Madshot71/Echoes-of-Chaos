using UnityEngine;

[CreateAssetMenu(fileName = "GunConfig", menuName = "Weapons/GunConfig", order = 0)]
public class GunConfig : WeaponConfig
{
    [Min(1)]public float fireRate;
    public int clipSize = 30;
    public int maxBullets = 100;
    public AudioClip shootSFX;
    public BulletConfig bullet;

}