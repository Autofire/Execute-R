using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OverridePlayerMovement : MonoBehaviour
{
	public enum Axis : int {
		X = 0,
		Y = 1,
		Z = 2
	}

	public float moveSpeed;
	public string hAxisName = "Horizontal";
	public string vAxisName = "Vertical";
	public Axis hAxis = Axis.X;
	public Axis vAxis = Axis.Z;

	[Tooltip("This triggers the FIRST time a move is registered.")]
	public UnityEvent onFirstMove;
	private bool triggeredFirstMoveEvent = false;

	public void Update() {
		float hAxisVal = Input.GetAxis(hAxisName);
		float vAxisVal = Input.GetAxis(vAxisName);

		if(Mathf.Abs(hAxisVal) > Mathf.Epsilon || Mathf.Abs(vAxisVal) > Mathf.Epsilon) {

			if(!triggeredFirstMoveEvent) {
				triggeredFirstMoveEvent = true;
				onFirstMove.Invoke();
			}

			Vector3 desiredMove = Vector3.zero;
			desiredMove[(int) hAxis] = hAxisVal * moveSpeed * Time.deltaTime;
			desiredMove[(int) vAxis] = vAxisVal * moveSpeed * Time.deltaTime;

			transform.position += desiredMove;


		}

	}

}
