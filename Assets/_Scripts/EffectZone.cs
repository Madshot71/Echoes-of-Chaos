using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EffectZone : MonoBehaviour 
{
    public ZoneType effect;
    public float amount = 5f;
    List<HitBox> hitboxs = new();


    enum ZoneType
    {
        Heal,
        Damage
    }

    private void FixedUpdate()
    {
        ApplyZoneEffect();
    }

    private void OnTriggerEnter(Collider other) {

        if(hitboxs.Exist(i => i.gameObject == other.gameObject))
        {
            return;
        }

        if(!other.TryGetComponent<HitBox>(out HitBox hitbox))
        {
            return;
        }

        hitboxs.Add(hitbox);
    }    

    private void OnTriggerExit(Collider other) 
    {
        if(hitboxs.Exists(i => i.gameObject == other.gameObject))
        {
            hitboxs.RemoveAll(i => i.gameObject == other.gameObject);
        }
    }


    private void ApplyZoneEffect()
    {
        hitboxs.RemoveAll(i => i == null);
        
        if(effect == ZoneType.Damage)
        {
            foreach (var item in hitboxs)
            {
                item.TakeDamage(amount);
            }
        }
        else if(effect == ZoneType.Heal)
        {
            foreach (var item in hitboxs)
            {
                item.Heal(amount);
            }
        }
    }
}