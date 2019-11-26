using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestroySelfOnHit : MonoBehaviour
{
	public string[] tagsToIgnore;

	private void OnTriggerEnter(Collider other) {
		if(!tagsToIgnore.Any(other.CompareTag)) {
			Destroy(gameObject);
		}
	}
}
