using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
	//public Vector3 velocity;
	public float velocity;

	//private Rigidbody rb;

	private void Start() {
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.velocity = transform.forward * velocity;
	}
}
