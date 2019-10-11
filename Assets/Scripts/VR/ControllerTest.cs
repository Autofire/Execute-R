using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTest : MonoBehaviour
{

	// (0.3, 0.7, -0.1)
	// (1.1, 1.3, -1.3)
	// (-1.1, 1.4, -1.0)
	// (-1.2, 1.4, 1.1)

	private void Update() {
		if(Input.GetButtonDown("Fire1")) {
			Debug.Log(gameObject.name + ": " + transform.position);
		}
	}
}
