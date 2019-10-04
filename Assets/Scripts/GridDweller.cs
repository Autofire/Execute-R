using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDweller : MonoBehaviour {
    public DwellerType type = DwellerType.Player;
    GridWorld dwellsOn;
    private CellPosition position = null;
    private float animTimer = 0.0f, animDuration = 0.0f, animDelay = 0.0f;
    private bool animating = false;
    Vector3 animStart, animEnd;

    // Start is called before the first frame update
    void Start() {
        dwellsOn = GridWorld.getInstance();
    }

    // Update is called once per frame
    void Update() {
        animTimer += Time.deltaTime;
        if (animating && animTimer > animDelay) {
            // We only need to animate the X and Z axes, so make the code agnostic to any changes
            // in the Y value.
            if (animTimer < (animDelay + animDuration)) {
                float progress = (animTimer - animDelay) / animDuration;
                float oldY = gameObject.transform.position.y;
                gameObject.transform.position = Vector3.Lerp(animStart, animEnd, progress);
                SyncCellPositionToRealPosition();
            } else {
                animEnd.y = gameObject.transform.position.y;
                gameObject.transform.position = animEnd;
                animating = false;
            }
        }
    }

    public void AnimateToCell(CellPosition position, float delay, float duration) {
        animTimer = 0.0f;
        animDelay = delay;
        animDuration = duration;
        animStart = gameObject.transform.position;
        animEnd = dwellsOn.GetRealPosition(position);
        animating = true;
    }

    /// Moves the real (screen) position of this object to whatever its position in the grid world
    /// is. (Grid world position is set with MoveToCell()). Call this if you want changes made with
    /// MoveToCell() to be reflected verbatim on screen.
    public void SyncRealPositionToCellPosition() {
        if (position != null) {
            gameObject.transform.position = dwellsOn.GetRealPosition(position);
        }
    }

    /// Determines which cell the object is occupying based on its real (screen) position and then
    /// moves this object's cell position to that cell. Call this if you want manual changes to the
    /// object's position to be reflected in the game logic.
    public void SyncCellPositionToRealPosition() {
        MoveToCell(dwellsOn.GridizeRealPosition(gameObject.transform.position));
    }

    /// Tries to move to a different cell on the grid. Note that this does NOT change the REAL
    /// position of the object. It will appear in the same position in the world. This method just
    /// changes which cell it is marked as occupying. If null is passed as newPosition, the object 
    /// will be effectively 'removed' from the grid world. Moving to a non-null cell will 'place' 
    /// the object back in the world.
    public void MoveToCell(CellPosition newPosition) {
        if (newPosition == position) return;
        if (position != null) dwellsOn.RemoveDwellerFromCell(position, this);
        position = newPosition;
        if (position != null) dwellsOn.AddDwellerToCell(position, this);
    }

    /// Like MoveToCell(CellPosition), but moves to a different position on whatever side of the
    /// world this object is already on. For example, if the object is on the Enemy side, it will
    /// try to move to the cell on the enemy side with the specified coordinates.
    public void MoveToCell(uint x, uint z) {
        MoveToCell(new CellPosition(x, z, position.side));
    }
}
