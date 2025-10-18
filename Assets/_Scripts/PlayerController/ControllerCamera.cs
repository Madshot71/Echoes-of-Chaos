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
    private Transform follow;
    private Transform _camera => Camera.main.transform;

    private void OnValidate()
    {
        if (controller == null)
            return;
        follow ??= new GameObject().transform;
        playerHead ??= controller.animator.GetBoneTransform(HumanBodyBones.Head);
    }

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        Follow();
        Rotate();
    }

    private void Init()
    {
        first.Follow = playerHead;
        first.LookAt = playerHead;
        third.Follow = playerHead;
        third.LookAt = playerHead;
    }

    private void Follow()
    {
        follow.position = playerHead.position;
    }
    
    private void Rotate()
    {
        Quaternion direction = _camera.transform.rotation;
        direction.x = 0;

        follow.rotation = Quaternion.Slerp(follow.rotation, direction, config.rotateSpeed * Time.deltaTime);
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