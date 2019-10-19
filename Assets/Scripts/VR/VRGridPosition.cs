using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReachBeyond.VariableObjects;

public class VRGridPosition : MonoBehaviour
{
	public TransformConstReference vrPosition;
	public GridDweller dweller;

	public float leashDistance = 1f;

	public void Update() {
		Vector3 targetPos = vrPosition.ConstValue.position;
		targetPos.y = 0;

//		Debug.Log(Vector3.Distance(targetPos, transform.position));

		Vector3 distance = targetPos - transform.position;

		// Ok, circular distance isn't going to work. We'll use a
		// distance across each axis instead.
		if(Mathf.Abs(distance.x) > leashDistance || Mathf.Abs(distance.z) > leashDistance) {

			Vector3 oldPosition = transform.position;

			SetPosition(targetPos);

			if(dweller.GetSpacePosition() == Vector3.zero) {
				SetPosition(oldPosition);
			}

			Debug.Log(dweller.GetSpacePosition());
		}

	}

	private void SetPosition(Vector3 newPos) {
		transform.position = newPos;
		dweller.SyncCellPositionToRealPosition();
		dweller.SyncRealPositionToCellPosition();
	}

}
