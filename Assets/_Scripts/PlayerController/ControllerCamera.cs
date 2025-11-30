using Cinemachine;
using UnityEngine;

public class ControllerCamera : MonoBehaviour 
{
    [Require][SerializeField] private PlayerController controller;
    [Require][SerializeField] private CameraConfig config;

    [Header("Settings")]
    [Require][SerializeField] private CinemachineVirtualCamera third;
    [Require][SerializeField] private CinemachineVirtualCamera first;
    [Require][SerializeField] private CinemachineVirtualCamera firstAim;
    [Require][SerializeField] private CinemachineVirtualCamera thirdAim;
    [SerializeField] private Transform follow ;

    [SerializeField] public bool isFirstPerson {get; private set;}= false;
    private bool isAimming = false;
    internal bool useFirstAim {get;set;} = false;
    private Transform playerHead;
    
    private Vector3 rotationEuler;
    public Vector2 input;
    public bool sprint;
    private bool lockRotation = false;

    private void OnValidate()
    {
        playerHead ??= controller.animator.GetBoneTransform(HumanBodyBones.Head);
    }

    private void Awake()
    {
        if (controller == null)
            return;
        Init();
    }

    private void Update()
    {
        SetAim();
    }

    private void LateUpdate()
    {
        Follow();
        Rotate();
        SetRotations();
    }

    private void Init()
    {
        follow ??= new GameObject("follow").transform;
        follow.parent = transform;

        first.Follow = follow;
        third.Follow = follow;
        firstAim.Follow = follow;
        thirdAim.Follow = follow;
    }

    private void Follow()
    {
        follow.position = playerHead.position;
    }

    private void Rotate()
    {
        if(lockRotation)
        {
            Quaternion rotation = controller.transform.rotation;
            follow.rotation = Quaternion.Slerp(follow.rotation , rotation , 40 * Time.deltaTime);
            return;
        }

        input.Normalize();
        rotationEuler.x += Sensitivity(input.y) * config.pitchSpeed * Time.deltaTime;
        rotationEuler.y += Sensitivity(input.x) * config.yawSpeed * Time.deltaTime;

        rotationEuler.x = Mathf.Clamp(rotationEuler.x, config.minPivot, config.maxPivot);
        rotationEuler.z = 0;

        follow.rotation = Quaternion.Euler(rotationEuler);
    }

    private void SetRotations()
    {
        first.transform.rotation = follow.rotation;
        third.transform.rotation = follow.rotation;
        firstAim.transform.rotation = follow.rotation;
        thirdAim.transform.rotation = follow.rotation;
    }

    public void AimToggle(bool value)
    {
        if (controller.weaponSystem == null)
        {
            isAimming = false;
            return;
        }

        isAimming = value;
    }

    private void SetAim()
    {
        if(controller.weaponSystem == null || controller.weaponSystem.current == null)
        {
            return;
        }

        if(useFirstAim || isFirstPerson)
        {
            firstAim.Priority = isAimming ? 11 : 0;
            thirdAim.Priority = 0;
            return;
        }

        thirdAim.Priority = isAimming ? 11 : 0;
        firstAim.Priority = 0;
    }

    private void LockCameraRotation(bool value)
    {
        lockRotation = value; 
    }

    public void ToggleAimView()
    {
        if(useFirstAim || isAimming == false)
        {
            useFirstAim = false;
        }
        else
        {
            useFirstAim = true;
        }
    }

    private float Sensitivity(float value)
    {
        float sensitivity = isFirstPerson? 
            isAimming ? config.fp_aim_sensitivity : config.fp_sensitivity :
            isAimming ? config.tp_aim_sensitivity : config.tp_sensitivity; 
    
        if (Mathf.Abs(value) > sensitivity)
        {
            return value;
        }

        return 0;
    }

    public void TogglePov()
    {
        if(isFirstPerson)
        {
            // Switch to third person
            first.Priority = 0;
            third.Priority = 10;
            isFirstPerson = false;
        }
        else
        {
            // Switch to first person
            first.Priority = 10;
            third.Priority = 0;
            isFirstPerson = true;
        }
    }
}