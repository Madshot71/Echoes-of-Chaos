using Cinemachine;
using UnityEngine;

public class ControllerCamera : MonoBehaviour 
{
    [Require][SerializeField] private PlayerController controller;
    [Require][SerializeField] private CameraConfig config;

    [Header("Settings")]
    [Require][SerializeField] private CinemachineVirtualCamera third;
    [Require][SerializeField] private CinemachineVirtualCamera first;
    [Require][SerializeField] private CinemachineVirtualCamera aim;
    [SerializeField] private bool isFirstPerson = false;
    private bool isAimming = false;
    private Transform playerHead;
    public Transform follow { get; private set; }
    private Vector3 rotationEuler;
    public Vector2 input;

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
    }

    private void Init()
    {
        follow ??= new GameObject("follow").transform;
        follow.parent = transform;

        first.Follow = follow;
        first.LookAt = follow;
        third.Follow = follow;
        third.LookAt = follow;
        aim.Follow = follow;
        aim.LookAt = follow;
    }

    private void Follow()
    {
        follow.position = playerHead.position;
    }

    private void Rotate()
    {
        input.Normalize();
        rotationEuler.x += Sensitivity(input.y) * config.pitchSpeed * Time.deltaTime;
        rotationEuler.y += Sensitivity(input.x) * config.yawSpeed * Time.deltaTime;

        rotationEuler.x = Mathf.Clamp(rotationEuler.x, config.minPivot, config.maxPivot);
        rotationEuler.z = 0;

        follow.rotation = Quaternion.Euler(rotationEuler);
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

        if (isAimming == false)
        {
            aim.Priority = 0;
        }
        else
        {
            aim.Priority = 11;
        }
    }

    
    private float Sensitivity(float value)
    {
        if (Mathf.Abs(value) > config.Sensitivity)
        {
            return value;
        }

        return 0;
    }

    public void TogglePov()
    {
        if(!isFirstPerson)
        {
            // Switch to third person
            first.Priority = 0;
            third.Priority = 10;
            isFirstPerson = true;
        }
        else
        {
            // Switch to first person
            first.Priority = 10;
            third.Priority = 0;
            isFirstPerson = false;
        }
    }
}