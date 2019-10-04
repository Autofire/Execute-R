using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// One option for doing movement in the game is to rigidly lock the enemies to a grid. This is
/// simple but results in clunky animation.
public class SnappyExample : MonoBehaviour {
    GridDweller dweller;
    CellPosition[] patrol = {
        new CellPosition(0, 0, GridClass.PlayerGrid),
        new CellPosition(1, 0, GridClass.PlayerGrid),
        new CellPosition(2, 0, GridClass.PlayerGrid),
        new CellPosition(3, 0, GridClass.PlayerGrid),
        new CellPosition(3, 1, GridClass.PlayerGrid),
        new CellPosition(3, 2, GridClass.PlayerGrid),
        new CellPosition(3, 3, GridClass.PlayerGrid),
        new CellPosition(2, 3, GridClass.PlayerGrid),
        new CellPosition(1, 3, GridClass.PlayerGrid),
        new CellPosition(0, 3, GridClass.PlayerGrid),
        new CellPosition(0, 2, GridClass.PlayerGrid),
        new CellPosition(0, 1, GridClass.PlayerGrid),
    };
    float timer = 0.0f;
    public float stepDuration = 0.5f;

    void Start() {
        dweller = GetComponent<GridDweller>();
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer > patrol.Length * stepDuration) {
            timer -= patrol.Length * stepDuration;
        }

        // Set our position on the grid. This does not change our "real" aka "screen" position.
        dweller.MoveToCell(patrol[Mathf.FloorToInt(timer / stepDuration)]);
        // Set our real position to mimic our grid position.
        dweller.SyncRealPositionToCellPosition();
    }
}
