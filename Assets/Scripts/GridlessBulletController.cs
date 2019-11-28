using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridlessBulletController : MonoBehaviour
{
	public string[] tagsToIgnore;
	public GameObject spawnOnDestruction;
	public DamageType damageType = DamageType.Damage;
	public int damage = 5;

	private void OnTriggerEnter(Collider other) {
		if(!tagsToIgnore.Any(other.CompareTag)) {
			Destroy(gameObject);

			if(spawnOnDestruction != null) {
				Instantiate(spawnOnDestruction, transform.position, spawnOnDestruction.transform.rotation);
			}

			Health otherHP = other.GetComponent<Health>();

			if(otherHP != null) {
				otherHP.TakeDamage(damageType, damage);
			}

		}
	}
}
