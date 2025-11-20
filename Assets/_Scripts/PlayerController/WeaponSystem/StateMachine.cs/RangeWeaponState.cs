using UnityEngine;
using UnityEngine.XR;

public class RangeWeaponState : WeaponState
{
    Transform aimPoint;
    Quaternion _targetRotation;

    public RangeWeaponState(WeaponSystem system) : base(system)
    {
        aimPoint = system.aimPoint;
    }
    public override void UpdateState()
    {
        Peak();
    }

    public override void OnAnimatorIK()
    {
        RightHand();
        LeftHand();
    }

    private void LeftHand()
    {
        //Update hand to position
        handler.UpdateHand(AvatarIKGoal.LeftHand , current.LeftHandIK);
    }

    private void RightHand()
    {
        if(handler.aimPoint == null)
        {
            return;
        }

        Vector3 aimPos = handler.aimPoint.TransformPoint(current._config.aimDownOffset);
        Vector3 point = handler.isAimming ? handler.hipPoint.position : aimPos;
        handler.UpdateHand(AvatarIKGoal.RightHand , point);
    }

    private void Peak()
    {
        Transform bone = animator.GetBoneTransform(HumanBodyBones.Spine);
        
        Quaternion baseRotation = bone.localRotation;

        Quaternion peakRotation = Quaternion.Euler(0 , config.peakAngle * handler.peakDirection , 0);
        _targetRotation = baseRotation * peakRotation;

        bone.localRotation = Quaternion.Slerp(bone.rotation , _targetRotation , 50 * Time.deltaTime);

        //reset peakDirection
        handler.peakDirection = 0;
    }

}