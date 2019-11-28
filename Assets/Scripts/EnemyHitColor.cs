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
	public Renderer targetRenderer;

	private void Awake() {
		if(targetRenderer == null) {
			targetRenderer = GetComponent<Renderer>();
		}
	}

	private void Start()
    {
        originalColor = targetRenderer.material.color;
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
        targetRenderer.material.color = curColor;
        timer -= Time.deltaTime;
    }

    public void Hit()
    {
        timer = colorTime;
    }
}
