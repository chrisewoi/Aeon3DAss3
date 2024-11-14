using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Damageable : MonoBehaviour
{
    [SerializeField] private float healthMax;
    [SerializeField] private UnityEvent onHealthZero;

    private float healthCurrent;

    private bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        healthCurrent = healthMax;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        
        Debug.Log($"The agent {name} took {amount} damage!");
        healthCurrent -= amount;

        if (healthCurrent <= 0) HealthZero();
        
    }

    public void RestoreHealth(float amount)
    {
        isDead = false;
        healthCurrent += amount;
        if (healthCurrent > healthMax)
        {
            healthCurrent = healthMax;
        }
    }

    private void HealthZero()
    {
        isDead = true;
        healthCurrent = 0;
        onHealthZero.Invoke();
        Debug.Log($"The agent {name} has died!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckForDamage(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
       CheckForDamage(other.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        CheckForDamage(other);
    }

    private void CheckForDamage(GameObject possibleSource)
    {
        if (possibleSource.TryGetComponent<DamageSource>(out DamageSource damageSource))
        {
            if (possibleSource.CompareTag(tag))
            {
                return;
            }
            TakeDamage(damageSource.GetDamage());
        }
    }
}
