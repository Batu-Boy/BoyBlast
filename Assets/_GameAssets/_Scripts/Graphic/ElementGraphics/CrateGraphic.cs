using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateGraphic : ElementGraphic
{
    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private ParticleSystem destroyParticle;

    public override void Explode()
    {
        base.Explode();
        if (_element is IDamageable damageable) 
            OnCrateDamaged(damageable.Health);
    }

    public void OnCrateDamaged(int remainingHealth)
    {
        if (remainingHealth <= 0)
        {
            StartCoroutine(DestroyCoroutine());
        }
        else
        {
            StartCoroutine(DamageCoroutine(remainingHealth));
        }
    }

    private IEnumerator DamageCoroutine(int remainingHealth)
    {
        if(damageParticle)
            damageParticle.Play();
        
        ChangeIconAtIndex(remainingHealth);
        
        yield break;
    }

    private IEnumerator DestroyCoroutine()
    {
        if(destroyParticle)
            destroyParticle.Play();
        
        ChangeIconAtIndex(0);
        yield return new WaitForSeconds(.25f);
        ResetIconValues();
        //TODO: pool
        Destroy(gameObject);
    }
}
