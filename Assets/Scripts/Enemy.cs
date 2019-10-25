using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float minimumMoveDelay = 2, maximumMoveDelay = 3;
    public float moveTime = 0.5f;
    public int health = 3;

    private GridDweller dweller;
    private float moveTimer;

    private void ResetMoveTimer() {
        moveTimer = Random.Range(minimumMoveDelay, maximumMoveDelay);
    }

    void Start() {
        dweller = GetComponent<GridDweller>();
        ResetMoveTimer();
    }

    void Update() {
        moveTimer -= Time.deltaTime;
        if (moveTimer < 0) {
            ResetMoveTimer();
            moveTimer += moveTime;
            CellPosition next 
                = GridWorld.getInstance()
                .FindRandomCellWithout(DwellerType.Enemy, GridClass.EnemyGrid);
            dweller.AnimateToCell(next, 0, moveTime);
        }
    }

    void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "PlayerAttack") {
            health -= 1;
            Debug.Log("Enemy hit!");
            Destroy(c.gameObject);
            if (health == 0) {
                Die();
            }
        }
    }

    private void Die() {
        Destroy(this.gameObject);
    }
}
