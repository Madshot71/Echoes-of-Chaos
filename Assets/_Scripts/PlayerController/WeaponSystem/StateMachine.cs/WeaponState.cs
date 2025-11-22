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

    public virtual void StartState()
    {
        Debug.Log($"{this} is Starting");
    }
    public abstract void UpdateState();

    public virtual void ExitState()
    {
        Debug.Log($"{this} is exiting");
    }
    public abstract void OnAnimatorIK();
}