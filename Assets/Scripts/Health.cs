using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DamageType
{
    Damage,
    Heal
}

public class Health: MonoBehaviour 
{
    private int currentHealth;
    private int maxHealth;

    private void Start() 
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageType affect, int amount)
    {
        if(affect == DamageType.Damage)
        {
            if(currentHealth < amount)
            {
                Die();
            }
            currentHealth -= amount;
        }
        else
        {
            if(currentHealth + amount > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else 
            {
                currentHealth += amount;
            }
        }
    }

    public void Die() 
    {
        Destroy(gameObject);
    }

    //Apply some feedback showing the player they got hurt/ enemy got hurt
    public void HitFeedback()
    {

    }
}