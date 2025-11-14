using UnityEngine;

public class NPC_Controller : PlayerController
{
    // AI Use
    private Transform headDirection;

    public override void SetCameraTransform()
    {
        headDirection = new GameObject("AI_HeadDirection").transform;
        headDirection.parent = transform;
        _camera = headDirection;
    }

    public void Direction(Vector3 direction)
    {
        headDirection.position = animator.GetBoneTransform(HumanBodyBones.Head).position;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        headDirection.rotation = Quaternion.Slerp(headDirection.rotation, rotation, 50 * Time.deltaTime);
    }
}