using Cinemachine;
using UnityEngine;

public class ControllerCamera : MonoBehaviour 
{
    [Require][SerializeField] private PlayerController controller;
    [Require][SerializeField] private CameraConfig config;

    [Header("Settings")]
    [Require][SerializeField] private CinemachineVirtualCamera third;
    [Require][SerializeField] private CinemachineVirtualCamera first;
    [SerializeField] private bool isFirstPerson = false;
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
    }

    private void Follow()
    {
        follow.position = playerHead.position;
    }

    private void Rotate()
    {
        input.Normalize();
        rotationEuler.x += Sensitivity(input.y) * config.rotateSpeed * Time.deltaTime;
        rotationEuler.y += Sensitivity(input.x) * config.rotateSpeed * Time.deltaTime;
        Quaternion direction = Quaternion.Euler(rotationEuler);
        direction.z = 0;

        follow.rotation = Quaternion.Slerp(follow.rotation, direction, config.lerpSpeed * Time.deltaTime);
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
        if(isFirstPerson)
        {
            // Switch to third person
            first.Priority = 0;
            third.Priority = 10;
        }
        else
        {
            // Switch to first person
            first.Priority = 10;
            third.Priority = 0;
        }
    }
}