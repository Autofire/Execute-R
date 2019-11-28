using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
	public Vector3 velocity = new Vector3(0, 0, -4);
	public int damage = 5;
    GridDweller dweller;

	/*
    public void SetVelocity(Vector3 newVelocity) {
        velocity = newVelocity;
    }
	*/

    void Start() {
        dweller = GetComponent<GridDweller>();
		//SetVelocity(Vector3.back * 4.0f);
		//velocity = Vector3.back * 4.0f;
    }

    void Update() {
		//Vector3 curPos = transform.position;
		//dweller.SyncCellPositionToRealPosition();
		//transform.Translate(velocity * Time.deltaTime);
		//curPos += velocity * Time.deltaTime;
		transform.position += velocity * Time.deltaTime;

		GridDweller[] contents = dweller.GetGridWorld().CellContents(dweller.GetCurrentCell());
		GridDweller player = contents.FirstOrDefault((gd) => gd.type == DwellerType.Player);

		//if(dweller.GetGridWorld().IsTypeInCell(dweller.GetCurrentCell(), DwellerType.Player)) {
		if(player != null) {
			// TODO: Deduct health.
			//Debug.Log("HIT THE PLAYER! (TODO: DEDUCT HEALTH)");

			player.GetComponentInParent<Health>().TakeDamage(DamageType.Damage, damage);
			Destroy(gameObject);
		}
    }
}
