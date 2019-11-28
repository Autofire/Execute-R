using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthExample : MonoBehaviour
{
    private Health health;

    //Initialized with default values. Change in inspector
    public KeyCode damageKey = KeyCode.Space;
    public int damageAmount = 5;

    private void Start()
    {
        health = gameObject.GetComponent<Health>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(damageKey))
        {
            Debug.Log("Player hit");
            health.TakeDamage(DamageType.Damage, damageAmount);
        }
    }

    public void BasicFeedback()
    {
        Debug.Log(gameObject.name + " has been hit!");
    }
}
