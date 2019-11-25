using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    Vector3 velocity = Vector3.up;
    GridDweller dweller;

    public void SetVelocity(Vector3 newVelocity) {
        velocity = newVelocity;
    }

    void Start() {
        dweller = GetComponent<GridDweller>();
        SetVelocity(Vector3.back * 4.0f);
    }

    void Update() {
        gameObject.transform.Translate(velocity * Time.deltaTime);
        if (dweller.GetGridWorld().IsTypeInCell(dweller.GetCurrentCell(), DwellerType.Player)) {
            // TODO: Deduct health.
            Debug.Log("HIT THE PLAYER! (TODO: DEDUCT HEALTH)");
            Destroy(gameObject);
        }
    }
}
