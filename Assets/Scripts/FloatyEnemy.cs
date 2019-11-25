using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State {
    Idle,
    Shoot,
    Travel,
}

public class FloatyEnemy : MonoBehaviour {
    private const float IDLE_CYCLE_DURATION = 0.7f, SHOOT_DURATION = 0.5f;
    private const float FLOAT_AMOUNT = 0.5f; // Meters to float off the ground.
    //                  seconds             seconds per meter
    private const float FLIGHT_BASE = 0.4f, FLIGHT_DURATION = 0.2f; 
    private Vector3 GUN_1 = new Vector3(0.4f, 0.1f, -0.4f), GUN_2 = new Vector3(-0.4f, 0.1f, -0.4f);
    public GameObject mesh, bullet;
    private float animTimer = 0.0f, animDuration = IDLE_CYCLE_DURATION * 6.0f;
    private State currentState = State.Idle;
    private GridDweller dweller;

    private CellPosition GetRandomCell() {
        return dweller.GetGridWorld().FindRandomCellWithout(DwellerType.Enemy, GridClass.EnemyGrid);
    }

    void Start() {
        dweller = GetComponent<GridDweller>();
        dweller.StartAt(GetRandomCell());
    }

    void IdleAnim() {
        mesh.transform.localPosition 
            = Vector3.up * (Mathf.Sin(animTimer * Mathf.PI * 2.0f) * 0.05f + FLOAT_AMOUNT);
        mesh.transform.localRotation
            = Quaternion.AngleAxis(6.0f * Mathf.Cos(animTimer * Mathf.PI * 2.0f), Vector3.back);
    }

    void ShootAnim() {
        float progress = animTimer / SHOOT_DURATION * Mathf.PI;
        mesh.transform.localPosition = Vector3.forward * Mathf.Sin(progress);
        mesh.transform.localPosition += Vector3.up * FLOAT_AMOUNT;
        mesh.transform.localRotation
            = Quaternion.AngleAxis(30.0f * Mathf.Cos(progress * 2.0f - 1.5f), Vector3.right);
    }

    void TravelAnim() {
        Vector3 pos = gameObject.transform.localPosition;
        float curveValue = 1.0f - Mathf.Abs(Mathf.Pow(1 - (animTimer / animDuration * 2.0f), 2.0f));
        pos.y = curveValue * 0.8f;
        mesh.transform.localRotation = Quaternion.AngleAxis(10.0f * curveValue, Vector3.right);
        gameObject.transform.localPosition = pos;
    }

    void FireBullet(Vector3 position) {
        position += gameObject.transform.position;
        position.y += FLOAT_AMOUNT;
        GameObject instance = Instantiate(bullet, position, Quaternion.identity);
    }

    void PickNewState() {
        if (currentState == State.Idle) {
            if (Random.Range(0, 2) < 9) {
                currentState = State.Shoot;
                animDuration = SHOOT_DURATION;
                FireBullet(GUN_1);
                FireBullet(GUN_2);
            } else {
                currentState = State.Travel;
                CellPosition newCell = GetRandomCell();
                Vector3 newPos = dweller.GetGridWorld().GetRealPosition(newCell);
                Vector3 currentPos = gameObject.transform.position;
                float distance = (newPos - currentPos).magnitude;
                animDuration = distance * FLIGHT_DURATION + FLIGHT_BASE;
                dweller.AnimateToCell(newCell, animDuration, 0.0f, 2.0f);
            }
        } else {
            currentState = State.Idle;
            animDuration = IDLE_CYCLE_DURATION * Random.Range(4, 8);
        }
    }

    // Update is called once per frame
    void Update() {
        animTimer += Time.deltaTime;
        if (animTimer > animDuration) {
            animTimer -= animDuration;
            PickNewState();
        }
        if (currentState == State.Idle) {
            IdleAnim();
        } else if (currentState == State.Shoot) {
            ShootAnim();
        } else if (currentState == State.Travel) {
            TravelAnim();
        }
    }
}
