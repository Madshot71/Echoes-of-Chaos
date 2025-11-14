using UnityEngine;

[CreateAssetMenu(fileName = "BulletConfig", menuName = "Weapons/BulletConfig", order = 0)]
public class BulletConfig : ScriptableObject 
{

    [Header("Bullet Settings")]
    public float Damage;
    public float Speed;
    public float gravity;
    public float distance;
    public Bullet Prefab;
    public AudioClip hitSFX;
}