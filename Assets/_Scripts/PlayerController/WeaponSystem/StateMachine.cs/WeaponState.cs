using UnityEngine;

public abstract class WeaponState 
{
    public WeaponSystem handler;
    public WeaponSystemConfig config => handler.config;
    public Animator animator => handler.animator;
    public Weapon current => handler.current;

    public WeaponState(WeaponSystem handler)
    {
        this.handler = handler;
    }


    public abstract void UpdateState();
    public abstract void OnAnimatorIK();
}