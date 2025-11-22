using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(PlayerController) , typeof(AudioSource))]
public class WeaponSystem : MonoBehaviour
{

    [SerializeField] public WeaponSystemConfig config;
    
    /// <summary>
    /// max number of weapons
    /// </summary>
    public const int max = 3;

    [Header("Settings")]
    [SerializeField] private float maxAimDistance;
    [field : SerializeField] public Weapon current {get; private set;}
    [SerializeField] private Transform weaponHolder;


    [Header("Holsters")]
    [SerializeField] private Holster[] holsters = new Holster[max];
    [SerializeField][Range(0, 1)] private float switchSensetivity;
    [SerializeField] private Vector3 size;

    [Header("Audio")]
    [SerializeField] private AudioClip switchClip;
    [SerializeField] private int weaponLayer;
    public bool isAimming = false;

    
    [Header("Rigging")]
    [SerializeField] public Transform hipPoint;
    [field : SerializeField] public Transform aimPoint {get; private set;}
    [SerializeField] private float switchSpeed;
    private Transform head;
    private Transform shoulder;
    private Transform rightHand;

    //Required Components
    private PlayerController controller;
    internal Animator animator => controller.animator;
    internal AudioSource audioSource {get; private set;}
    internal Transform _camera {get; private set;}

    private bool isSwitchingWeapon;
    private float switchDelay;
    private float next_SwitchTime;

    /// <summary>
    /// Current weapon index
    /// </summary>
    private int index = 0;
    private WeaponState currentState;
    public int peakDirection = 0;

    private void OnValidate()
    {
        controller ??= GetComponent<PlayerController>();
    }

    private void Awake()
    {
        _camera ??= Camera.main.transform;
        head ??= animator.GetBoneTransform(HumanBodyBones.Head);
        shoulder ??= animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        rightHand ??= animator.GetBoneTransform(HumanBodyBones.RightHand);

        hipPoint ??= new GameObject("hipPoint").transform;
        aimPoint ??= new GameObject("aimPoint").transform;

        hipPoint.forward = transform.forward;
        aimPoint.forward = transform.forward;

        hipPoint.parent = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        aimPoint.parent = animator.GetBoneTransform(HumanBodyBones.Head);
    }
    
    private void Update()
    {
        if (current == null)
        {
            if (currentState != null)
            {
                currentState.ExitState();
                currentState = null;
            }
            return;
        }

        var requiredState = GetCurrentState(current.weaponType);
        if (currentState == null || currentState.GetType() != requiredState.GetType())
        {
            ChangeState(requiredState);
        }
    }

    private void LateUpdate()
    {
        if (current == null || current._config == null)
        {
            SetAim(false);
            Weaponindex(0);
            animator.SetLayerWeight(weaponLayer, 0f);
            return;
        }

        UpdateHolder();
        SetPoints();
        currentState?.UpdateState();
        animator.SetLayerWeight(weaponLayer, 1f);
        Weaponindex(current._config.AnimationIndex);
        SetAim(isAimming);
    }

    private void ChangeState(WeaponState newState)
    {
        if(currentState?.GetType() == newState?.GetType()) return;
        
        currentState?.ExitState();
        currentState = newState;
        currentState?.StartState();
    }

    #region Animation

    private void OnAnimatorIK(int index)
    {
        if(current == null){
            return;
        }

        currentState?.OnAnimatorIK();
    }

    internal void UpdateHand(AvatarIKGoal hand , Transform point)
    {
        if(point == null){
            //Reset
            animator.SetIKPositionWeight(hand , 0);
            animator.SetIKRotationWeight(hand , 0);
            return;
        }

        animator.SetIKPositionWeight(hand , 1);
        animator.SetIKRotationWeight(hand , 1);
        animator.SetIKPosition(hand , point.position);
        animator.SetIKRotation(hand , point.rotation);
    }

    internal void UpdateHand(AvatarIKGoal hand , Vector3 point)
    {
        animator.SetIKPositionWeight(hand , 1);
        animator.SetIKPosition(hand , point);
    }

    internal void UpdateHand(AvatarIKGoal hand , Quaternion rotation)
    {
        animator.SetIKRotationWeight(hand , 1);
        animator.SetIKRotation(hand , rotation);
    }

    private void UpdateHolder()
    {
        weaponHolder.position = rightHand.position;
        weaponHolder.forward = rightHand.forward;
    }

    private void SetPoints()
    {
        if(current == null)
        {
            return;
        }
        
        //Where the weapons Aim should be
        aimPoint.localPosition = config.aimPointPosition;
        hipPoint.localPosition = config.hipPointPosition;
    }

    #endregion

    #region Actions
    public void Attack()
    {
        if (current != null)
        {
            current.Attack();
        }
    }

    public void Reload()
    {
        if (current != null)
        {
            current.Reload();
        }
    }
    public void AimToggle(bool value)
    {
        if (current == null)
        {
            isAimming = false;
            return;
        }

        isAimming = value;
    }
    

    public void EquipWeapon(Weapon weapon)
    {
        Holster holster = GetHolster(null); // Find Holster with a null/empty weapon

        if (holster == null)
        {
            holster = holsters[index]; // swap weapons with the current weapon
        }

        index = Array.IndexOf(holsters , holster);
        weapon.rb.isKinematic = true;
        weapon.rb.useGravity = false;
        weapon.boxCollider.enabled = false;
        current = weapon;
        holster.weapon = weapon;
        holster.Unholster(weaponHolder);
    }


    public void HolsterToggle()
    {
        if (current == null)
        {
            // If no weapon is equipped, unholster the one at the current index
            if (holsters[index].weapon != null)
            {
                holsters[index].Unholster(weaponHolder);
                current = holsters[index].weapon;
                Debug.Log("Weapon Unholstered");
            }
        }
        else
        {
            // If a weapon is equipped, holster it
            holsters[index].HolsterWeapon();
            current = null;
            Debug.Log("Weapon Holstered");
        }
    }


    public void Switch(int index)
    {
        if (index < 0 || index > max -1 || Time.time < next_SwitchTime || isSwitchingWeapon)
        {
            return;
        }

        next_SwitchTime = Time.time + switchDelay;
        StartCoroutine(SwitchTo(index));
    }

    public void Switch(float delta)
    {
        if (Mathf.Abs(delta) < switchSensetivity || Time.time < next_SwitchTime || isSwitchingWeapon)
        {
            return;
        }

        next_SwitchTime = Time.time + switchDelay;

        if (delta > 0)
        {
            StartCoroutine(SwitchTo(Wrap(index + 1)));
        }
        else
        {
            StartCoroutine(SwitchTo(Wrap(index - 1)));
        }
    }

    private IEnumerator SwitchTo(int i)
    {
        if (holsters[i].weapon == null || i == index)
        {
            Debug.Log($"Cant switch to {index} , no weapon at that point");
            yield break;
        }

        isSwitchingWeapon = true;
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        PlayAudio(switchClip);
        //Holsters current weapon
        if (current != null)
        {
            holsters[index].HolsterWeapon();
        }

        yield return wait;

        PlayAudio(switchClip);
        //UnHolster the weapon we switched to
        holsters[i].Unholster(weaponHolder);
        yield return wait;

        // Setting the current weapon to our new weapon
        current = holsters[i].weapon;
        index = i;
        isSwitchingWeapon = false;
    }

    #endregion


    #region  Internal Helper Fuctions
    private Holster GetHolster(Weapon weapon)
    {
        for (int i = 0; i < holsters.Length; i++)
        {
            if (holsters[i].weapon == weapon)
            {
                return holsters[i];
            }
        }
        return null;
    }

    private int Wrap(int value)
    {
        switch (value)
        {
            case < 0:
                return max - 1;
            case > max - 1:
                return 0;
            default:
                return value;
        }
    }

    private WeaponState GetCurrentState(Weapon.WeaponType weaponType)
    {
        return weaponType switch
        {
            Weapon.WeaponType.Gun => new RangeWeaponState(this),
            Weapon.WeaponType.Melee => throw new NotImplementedException(),
            Weapon.WeaponType.Bow => throw new NotImplementedException(),
            _=> throw new NotImplementedException(),
        };
    }

    void PlayAudio(AudioClip clip)
    {
        if (clip == null || audioSource == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }

    private void Weaponindex(int value)
    {
        animator?.SetFloat("WeaponIndex", value);
    }

    private void SetAim(bool value)
    {
        animator?.SetBool("Aim", value);
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;

        foreach (var item in holsters)
        {
            if(item.point != null)
                Gizmos.DrawCube(item.point.position, size);
        }    
    }


    #endregion

    [System.Serializable]
    public class Holster
    {
        public Transform point;
        public Weapon weapon;

        public void HolsterWeapon()
        {
            weapon.transform.parent = point;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }

        public void Unholster(Transform hand)
        {
            if (weapon.Valid == false || hand == null)
            {
                Debug.Log($"Cant UnHolster {weapon} , {weapon._config} or {hand} is null");
                return;
            }

            weapon.transform.parent = hand;
            weapon.transform.localPosition = weapon._config.leftHandOffset;
            weapon.transform.localRotation = Quaternion.identity;
        }

        public IEnumerator Drop(Transform hand)
        {
            weapon.transform.parent = null;
            weapon.rb.position = hand.position;
            yield return null;
            weapon.boxCollider.enabled = true;
            weapon.rb.isKinematic = false;
            weapon.rb.useGravity = true;
            yield return null;

        }
        
    }

}
