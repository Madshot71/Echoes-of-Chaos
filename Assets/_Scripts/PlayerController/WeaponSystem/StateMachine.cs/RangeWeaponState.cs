using UnityEngine;
using UnityEngine.XR;

public class RangeWeaponState : WeaponState
{
    Quaternion _targetRotation;

    public RangeWeaponState(WeaponSystem system) : base(system)
    {
    
    }

    public override void UpdateState()
    {
        //Peak();
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
        if(handler.hipPoint == null && handler.aimPoint == null)
        {
            return;
        }

        Vector3 point = Vector3.zero;
        if(handler.aimPoint != null && handler.isAimming)
        {
            point = handler.aimPoint.TransformPoint(current._config.aimDownOffset);
        }
        else if(handler.hipPoint != null && handler.isAimming == false)
        {
            point = handler.hipPoint.TransformPoint(current._config.hipOffset);
        }

        handler.UpdateHand(AvatarIKGoal.RightHand , point);
    }

    private void Peak()
    {
        if(config == null)
        {
            return;
        }
        Transform bone = animator.GetBoneTransform(HumanBodyBones.Spine);
        
        Quaternion baseRotation = bone.localRotation;

        Quaternion peakRotation = Quaternion.Euler(0 , config.peakAngle * handler.peakDirection , 0);
        _targetRotation = baseRotation * peakRotation;

        bone.localRotation = Quaternion.Slerp(bone.rotation , _targetRotation , 50 * Time.deltaTime);

        //reset peakDirection
        handler.peakDirection = 0;
    }

}