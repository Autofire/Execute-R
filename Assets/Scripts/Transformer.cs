using UnityEngine;
using ReachBeyond.VariableObjects;

public class Transformer : MonoBehaviour {

	[SerializeField] private Vector3ConstReference moveSpeed;
	[SerializeField] private Vector3ConstReference rotationSpeed;
	[SerializeField] private Vector3ConstReference scaleSpeed;

	void Update () {
		transform.position += moveSpeed.ConstValue * Time.deltaTime;
		//transform.Rotate(rotationSpeed.constValue * Time.deltaTime);
		transform.localRotation = transform.localRotation * Quaternion.Euler(rotationSpeed.ConstValue * Time.deltaTime);
		transform.localScale += scaleSpeed.ConstValue * Time.deltaTime;
	}
}
