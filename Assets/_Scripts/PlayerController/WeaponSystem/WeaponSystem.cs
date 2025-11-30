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
    [SerializeField] public float maxAimDistance;
    [field : SerializeField] public Weapon current {get; private set;}
    [SerializeField] private Transform weaponHolder;
    [field : SerializeField] internal Transform aimPoint {get; private set;}


    [Header("Holsters")]
    [SerializeField] private Holster[] holsters = new Holster[max];
    [SerializeField][Range(0, 1)] private float switchSensetivity;
    [SerializeField] private Vector3 size;

    [Header("Audio")]
    [SerializeField] private AudioClip switchClip;
    [SerializeField] private int weaponLayer;
    [SerializeField][Range(0 , 1)] private float weight = 0.8f;
    public bool isAimming = false;

    [Header("Rigging")]
    [SerializeField] public Transform shoulderPoint;
    [field : SerializeField] public Transform ironSightAim {get; private set;}
    [SerializeField] public MultiAimConstraint rightHandAim;
    [SerializeField] public MultiAimConstraint spineAim;
    [SerializeField] public MultiAimConstraint headAim;
    [SerializeField] private Direction spineForward;

    internal enum Direction
    {
        X , Y , Z , Xn , Yn , Zn
    }

    private Transform head;
    private Transform shoulder;
    private Transform rightHand;

    //Required Components
    internal PlayerController controller {get; private set;}
    internal Animator animator => controller.animator;
    internal AudioSource audioSource {get; private set;}
    internal Transform _camera {get; private set;}
    internal Transform spine;
    private bool isSwitchingWeapon;
    private float switchDelay;
    private float next_SwitchTime;

    /// <summary>
    /// Current weapon index
    /// </summary>
    private int index = 0;
    private WeaponState currentState;
    public int peakDirection = 0;
    public bool useIronSight = false;
    public bool attack = false;

    #if UNITY_EDITOR
    protected bool LockToShoulder {get; private set;} = false;
    protected bool LockToAim {get ;private set;} = false;
    protected bool Unlocked {get; private set;} = true;


    #endif

    private void OnValidate()
    {
        controller ??= GetComponent<PlayerController>();

        _camera = Camera.main.transform;

        shoulderPoint ??= new GameObject("hipPoint").transform;
        ironSightAim ??= new GameObject("ironSight").transform;

        head ??= animator.GetBoneTransform(HumanBodyBones.Head);
        shoulder ??= animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        rightHand ??= animator.GetBoneTransform(HumanBodyBones.RightHand);
        spine ??= animator.GetBoneTransform(HumanBodyBones.Chest);

        aimPoint ??= new GameObject("AimPoint").transform;

        SetUpRightHandConstrant();
        SetUpBodyRigConstrant();
    }

    private void Awake()
    {
        //Getting Transforms
        shoulderPoint.parent = shoulder;
        ironSightAim.parent = animator.GetBoneTransform(HumanBodyBones.RightEye);

        shoulderPoint.forward = transform.forward;
        ironSightAim.forward = transform.forward;

        shoulderPoint.parent = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        ironSightAim.parent = animator.GetBoneTransform(HumanBodyBones.Head);

        aimPoint.parent = _camera;
        
        Vector3 position = aimPoint.localPosition;
        position.z = maxAimDistance;
        aimPoint.localPosition = position;
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

        
        Weaponindex(current._config.AnimationIndex);
        SetAim(isAimming);
        //UpdateBody();
    }

    private void LateUpdate()
    {
        if (current == null || current._config == null)
        {
            SetAim(false);
            Weaponindex(0);
            animator.SetLayerWeight(weaponLayer, 0f);
            SetRightHandAimWeight(0);
            return;
        }

        SetPoints();
        animator.SetLayerWeight(weaponLayer, weight);
        UpdateHolder();

        if (Unlocked)
        {
           currentState?.UpdateState(); 
        }
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

        if(Unlocked)
        {
            currentState?.OnAnimatorIK();
            return;
        }  
        SetLockedIK();
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

    internal void SetRightHandAimWeight(float amount)
    {
        amount = Mathf.Clamp01(amount);

        rightHandAim.weight = amount;
    }

    private void UpdateHolder()
    {
        weaponHolder.position = rightHand.position;
        weaponHolder.rotation = rightHand.rotation;
    }

    private void SetUpBodyRigConstrant()
    {
        if(spineAim.data.constrainedObject == spine)
        {
            return;
        }

        spineAim.data.sourceObjects.Clear();
        spineAim.data.sourceObjects.Add(new(spine , 1));
    }

    private void SetPoints()
    {
        if(config == null)
            return;
        
        if(ironSightAim != null)
            ironSightAim.localPosition = current._config.hipOffset;
        
        if(shoulderPoint != null)
            shoulderPoint.localPosition = current._config.hipOffset;
    }

    private void UpdateBody()
    {
        if(current == null)
        {
            return;
        }

        Vector3 rootForward = transform.forward;
        Vector3 childAim = GetDirection(spine , spineForward);

        Vector3 projectedRootForward = Vector3.ProjectOnPlane(rootForward , Vector3.up);
        Vector3 projectedChildAim = Vector3.ProjectOnPlane(childAim , Vector3.up);

        float angle = Vector3.SignedAngle(projectedRootForward, projectedChildAim , transform.up);
       
        if(Mathf.Abs(angle) < config.maxTurnAngle)
        {
            return;
        }
        controller.TurnAction(angle);
    }

    #endregion

    #region Actions

    public void Attack(bool value)
    {
        if(value == false)
        {
            return;
        }
        
        if (current != null && CanShoot())
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
        if (holsters[i].weapon == null)
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

        if(value)
            controller.FaceCameraForward();
    }

    private bool CanShoot()
    {
        if(current == null)
        {
            return false;
        }

        Vector3 postion = useIronSight ? ironSightAim.position : shoulderPoint.position;
        float distance = Vector3.Distance(postion , weaponHolder.position);
        if(distance < 0.3f)
        {
            return true;
        }
        else
        {
            Debug.Log("Cant Shoot yet");
            return false;
        }
    }

    private void SetUpRightHandConstrant()
    {
        if(rightHandAim.data.constrainedObject == rightHand)
        {
            return;
        }

        rightHandAim.data.constrainedObject = rightHand;
        rightHandAim.data.sourceObjects.Clear();
        rightHandAim.data.sourceObjects.Add(new WeightedTransform(aimPoint , 1));
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;

        foreach (var item in holsters)
        {
            if(item.point != null)
                Gizmos.DrawCube(item.point.position, size);
        }    
    }

    #if UNITY_EDITOR

    public void UpdateWeapon()
    {
        if(config == null)
        {
            Debug.LogError($"Error : Cant set {config} is null");
            return;
        }

        if(current == null)
        {
            return;
        }

        current.UpdateConfig();
    }

    public void UpdateIronSightAimOffset()
    {
        if(LockToAim)
            current._config.aimDownOffset = ironSightAim.localPosition;
    }

    public void UpdateShoulderAimOffset()
    {
        if(LockToShoulder)
            current._config.hipOffset = shoulderPoint.localEulerAngles;
    }

    public void LockControlletToAim()
    {
        LockToAim = true;
        LockToShoulder = false;
        Unlocked = false;
    }

    public void LockControlletToShoulder()
    {
        LockToAim = false;
        LockToShoulder = true;
        Unlocked = false;
    }

    public void Unlock()
    {
        LockToAim = false;
        LockToShoulder = false;
        Unlocked = true;
        Debug.Log($"{this} Unlocked");
    }

    private void SetLockedIK()
    {
        Vector3 point = Vector3.zero;
        if(ironSightAim != null && LockToAim)
        {
            point = ironSightAim.TransformPoint(current._config.aimDownOffset);
        }
        else if(shoulderPoint != null && LockToShoulder)
        {
            point = shoulderPoint.TransformPoint(current._config.hipOffset);
        }
        
        UpdateHand(AvatarIKGoal.RightHand , point);

        SetRightHandAimWeight(1);
    }

    private Vector3 GetDirection(Transform _transform ,Direction _value)
    {
        return _value switch
        {
            Direction.X => _transform.right,
            Direction.Y => _transform.up,
            Direction.Z => _transform.forward,
            Direction.Xn => _transform.right * -1,
            Direction.Yn => _transform.up * -1,
            Direction.Zn => _transform.forward,
            _=> transform.forward
        } ;
    }

    #endif


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
            weapon.transform.localPosition = weapon._config.rightHandOffset;
            weapon.transform.localRotation = weapon._config.rightHandRotation;
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
