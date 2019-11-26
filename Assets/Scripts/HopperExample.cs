using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode {
    Wait,
    Animate,
}


/// One option is to pick a grid square to move to and play an animation to get there. This is the
/// most complicated option but I think it would be worth it.
public class HopperExample : MonoBehaviour {
    const float WAIT_TIME = 1.0f;
    const float ANIMATE_TIME = 0.5f;
    const float ANIMATION_HEIGHT = 3.0f;
    GridDweller dweller;
    CellPosition[] patrol = {
        new CellPosition(0, 0, GridClass.EnemyGrid),
        new CellPosition(2, 0, GridClass.EnemyGrid),
        new CellPosition(2, 2, GridClass.EnemyGrid),
        new CellPosition(0, 2, GridClass.EnemyGrid),
        new CellPosition(3, 3, GridClass.EnemyGrid),
        new CellPosition(1, 3, GridClass.EnemyGrid),
        new CellPosition(1, 1, GridClass.EnemyGrid),
        new CellPosition(3, 1, GridClass.EnemyGrid),
    };
    Mode mode = Mode.Wait;
    uint patrolStep = 0;
    float timer = 0.0f;

    void Start() {
        dweller = GetComponent<GridDweller>();
    }

    /// Roots at x=0 and x=1, vertex at (0.5, 1.0).
    float ParabolicCurve(float progress) {
        return 1.0f - Mathf.Pow(2 * progress - 1, 2.0f);
    }

    void Update() {
        timer += Time.deltaTime;

        if (mode == Mode.Wait && timer >= WAIT_TIME) {
            // Wait time is over, fire up the animation.
            timer -= WAIT_TIME;
            mode = Mode.Animate;
            // Start the anim at the current position...
            patrolStep++;
            if (patrolStep >= patrol.Length) {
                patrolStep = 0;
            }
            dweller.AnimateToCell(patrol[patrolStep], ANIMATE_TIME);
        } else if (mode == Mode.Animate && timer >= ANIMATE_TIME) {
            timer -= ANIMATE_TIME;
            mode = Mode.Wait;

            // Make sure we are on the ground once the animation ends.
            Vector3 newPosition = gameObject.transform.position;
            newPosition.y = 0;
            gameObject.transform.position = newPosition;
        }

        if (mode == Mode.Animate) {
            float progress = timer / ANIMATE_TIME;
            // Compute a parabolic jump animation.
            Vector3 newPosition = gameObject.transform.position;
            newPosition.y = ANIMATION_HEIGHT * ParabolicCurve(progress);
            gameObject.transform.position = newPosition;
        }
    }
}
