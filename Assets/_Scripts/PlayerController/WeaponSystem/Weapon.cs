using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource), typeof(WeaponInteract), typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class Weapon : MonoBehaviour
{
    public BoxCollider boxCollider { get; private set; }
    public Rigidbody rb { get; private set; }
    public AudioSource audioSource { get; private set; }
    public WeaponInteract weaponInteract { get; private set; }
    [field : SerializeField] public WeaponConfig _config { get; protected set; }
    [field : SerializeField] public Transform LeftHandIK { get; protected set; }
    [field : SerializeField] public Transform adsPoint {get; protected set;}
    public WeaponType weaponType;

    internal abstract void Attack();
    internal abstract void Reload();

    public bool Valid
    {
        get
        {
            return _config != null;
        }
    }

    private void OnValidate()
    {
        rb ??= GetComponent<Rigidbody>();
        audioSource ??= GetComponent<AudioSource>();
        weaponInteract ??= GetComponent<WeaponInteract>();
        boxCollider ??= GetComponent<BoxCollider>();
    }

    internal virtual void HIT()
    {

    }

    internal abstract float ReloadProgress();

    protected virtual void PlayAudio(AudioClip clip)
    {
        if(clip == null || audioSource == null)
        {
            return;
        }
        
        audioSource.PlayOneShot(clip);
    }
    public enum WeaponType
    {
        Melee,
        Bow,
        Gun
    }
}