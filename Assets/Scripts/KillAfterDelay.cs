using UnityEngine;
using ReachBeyond.VariableObjects;

public class KillAfterDelay : MonoBehaviour {

	[SerializeField] FloatConstReference delay;
	[Tooltip(
		"If false, tries to kill once and then disables self. " +
		"Otherwise, keeps trying until self dies."
	)]
	[SerializeField] BoolConstReference keepRetrying;

	private float startTime;

	private void OnEnable() {
		startTime = Time.time;
	}

	private void Update() {

		if(Time.time > startTime + delay.ConstValue) {

			if(!keepRetrying.ConstValue) {
				this.enabled = false;
			}

			//MortalityHandler.Kill(gameObject);
			Destroy(gameObject);
		}
	}
}
