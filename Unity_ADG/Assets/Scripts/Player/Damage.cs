using System;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public Healt healt;

    public DamageTable damageTable;

    public ParticleSystem particleEffect;

    public Action<float> onDamaged;

    DamageUI damageUI;

    private void OnEnable()
    {
        damageUI = FindObjectOfType<DamageUI>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        float curDamage = GetDamage(collision.impulse.sqrMagnitude);
        if (curDamage > 0)
        {
            damageUI.AppearDamageText(curDamage);
            damageUI.AppearDamageScreenEffect();
            SpawnParticle(collision.contacts[0].point);

            healt.TakeDamage(curDamage);

            if (onDamaged != null)
            {
                onDamaged.Invoke(curDamage);
            }
        }
    }
    //Max damage = 1500
    float GetDamage(float value)
    {
        if (damageTable)
        {
            if (damageTable.slots.Length > 0)
            {
                for (int i = 0; i < damageTable.slots.Length; i++)
                { 
                    if (value >= damageTable.slots[i].minThresh && value <= damageTable.slots[i].maxThresh)
                    {
                        return damageTable.slots[i].amount;
                    }
                }
            }
        }
        return 0;
    }
    void SpawnParticle(Vector3 position)
    {
        if (particleEffect)
        {
            Instantiate(particleEffect.gameObject, position, Quaternion.identity);
        }
    }
}
