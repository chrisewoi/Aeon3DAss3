using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : RaycastFromScreenCentre
{
    [SerializeField] private float power;

    private DamageSource damageSource;
    private GunEffect effect;

    protected override void Start()
    {
        // base. refers to the parent script
        // base.Start() wil run the parent's Start() on this script
        base.Start();
        damageSource = GetComponent<DamageSource>();
        effect = GetComponentInChildren<GunEffect>();
    }

    public void Shoot()
    {
        // TryToHit() comes from our parent script (RaycastFromScreenCentre)
        // If we hit something, hit.collider will have a value. Else, hit.collider will be null
        RaycastHit hit = TryToHit();
        
        // If we did hit something...
        if (hit.collider)
        {
            if (hit.rigidbody && hit.rigidbody.TryGetComponent<Damageable>(out Damageable agent))
            {
                agent.TakeDamage(damageSource.GetDamage());
            }
        }
        effect.Play(hit.point);
    }
}
