using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitColor : MonoBehaviour
{
    private Color originalColor;
    private Color curColor;
    private float normalizedTimer;
    private float timer = 0f;

    //Initialized to default values. Can change in inspector.
    public float colorTime = 1f;
    public Color hitColor = Color.red;

    private void Start()
    {
        originalColor = gameObject.GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        if (timer > 0)
        {
            Fade();
        }
    }

    private void Fade()
    {
        normalizedTimer = timer / colorTime;
        curColor = Color.Lerp(originalColor, hitColor, normalizedTimer);
        gameObject.GetComponent<Renderer>().material.color = curColor;
        timer -= Time.deltaTime;
    }

    public void Hit()
    {
        timer = colorTime;
    }
}
