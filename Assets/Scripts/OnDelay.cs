using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDelay : MonoBehaviour
{
	public float delay;
	public UnityEvent effect;

	public float startTime;

	private void OnEnable() {
		startTime = Time.time;
	}

	void Update() {
		if(Time.time > startTime + delay) {
			effect.Invoke();
			enabled = false;
		}
    }


}
