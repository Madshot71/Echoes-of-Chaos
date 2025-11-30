using UnityEngine;
using UnityEngine.XR;

public class RangeWeaponState : WeaponState
{
    Quaternion _targetRotation;

    private Transform rightHand;
    private Transform camera;

    public RangeWeaponState(WeaponSystem system) : base(system)
    {
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        camera = system._camera;
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
        if(current.LeftHandIK == null)
        {
            return;
        }

        handler.UpdateHand(AvatarIKGoal.LeftHand , current.LeftHandIK.position);
        handler.UpdateHand(AvatarIKGoal.LeftHand , current.LeftHandIK.rotation);
    }

    private void RightHand()
    {
        if(handler.shoulderPoint == null && handler.ironSightAim == null)
        {
            return;
        }

        if(handler.isAimming == false && handler.attack == false)
        {
            handler.SetRightHandAimWeight(0);
            return;
        }

        Debug.Log($"Running {this} ");

        Vector3 point = Vector3.zero;
        if(handler.ironSightAim != null && handler.useIronSight)
        {
            point = handler.ironSightAim.TransformPoint(current._config.aimDownOffset);
        }
        else if(handler.shoulderPoint != null && handler.useIronSight == false)
        {
            point = handler.shoulderPoint.TransformPoint(current._config.hipOffset);
        }
        
        handler.UpdateHand(AvatarIKGoal.RightHand , point);
        handler.SetRightHandAimWeight(1);
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