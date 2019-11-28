using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DamageType
{
    Damage,
    Heal
}

public class Health : MonoBehaviour
{
    private int currentHealth;
    public int maxHealth = 15;      //Default of 15. Customize through inspector.

    public UnityEvent hitFeedback;
    public UnityEvent deathFeedback;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageType affect, int amount)
    {
        if (affect == DamageType.Damage)
        {
            if (currentHealth <= amount)
            {
                HitFeedback();
                DeathFeedback();
                Die();
            }

            currentHealth -= amount;
            HitFeedback();

            //Temporary; used to show current health of object.
            Debug.Log(gameObject.name + " health: " + currentHealth);
        }
        else
        {
            if (currentHealth + amount > maxHealth)
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

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    //Apply some feedback showing the player they got hurt/ enemy got hurt
    public void HitFeedback()
    {
        hitFeedback.Invoke();
    }

    public void DeathFeedback()
    {
        deathFeedback.Invoke();
    }
}