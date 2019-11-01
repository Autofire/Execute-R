using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDweller : MonoBehaviour {
    public DwellerType type = DwellerType.Player;

    private GridWorld dwellsOn;
    private CellPosition position = null;
    private float animTimer = 0.0f, animDuration = 0.0f, animDelay = 0.0f;
    private bool animating = false;
    Vector3 animStart, animEnd;

    private void Start() {
        dwellsOn = GridWorld.getInstance();
    }

    void StartAt(CellPosition position) {
        Start();
        MoveToCell(position);
        SyncRealPositionToCellPosition();
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

	/// <summary>
    /// Moves the real (screen) position of this object to whatever its position in the grid world
    /// is. (Grid world position is set with MoveToCell()). Call this if you want changes made with
    /// MoveToCell() to be reflected verbatim on screen.
	/// </summary>
    public void SyncRealPositionToCellPosition() {
        if (position != null) {
            gameObject.transform.position = dwellsOn.GetRealPosition(position);
        }
    }

	/// <summary>
	/// Grabs the position of the cell nearest to the given world
	/// position. If the position falls outside of the grid, then null
	/// is returend instead.
	/// </summary>
	/// <param name="realPosition">A world position</param>
	/// <returns>The position of the requested cell, or null.</returns>
	public Vector3? GridizeRealPostion(Vector3 realPosition) {
		CellPosition cell = dwellsOn.GridizeRealPosition(realPosition);

		if(cell != null) {
			return dwellsOn.GetRealPosition(cell);
		}
		else {
			return null;
		}

	}

	/// <summary>
    /// Determines which cell the object is occupying based on its real (screen) position and then
    /// moves this object's cell position to that cell. Call this if you want manual changes to the
    /// object's position to be reflected in the game logic.
	/// </summary>
    public void SyncCellPositionToRealPosition() {
        MoveToCell(dwellsOn.GridizeRealPosition(gameObject.transform.position));
    }

	/// <summary>
	/// Tries to move to a different cell on the grid. Note that this does NOT change the REAL
	/// position of the object. It will appear in the same position in the world. This method just
	/// changes which cell it is marked as occupying. If null is passed as newPosition, the object 
	/// will be effectively 'removed' from the grid world. Moving to a non-null cell will 'place' 
	/// the object back in the world.
	/// </summary>
	/// <param name="newPosition"></param>
	public void MoveToCell(CellPosition newPosition) {
        if (newPosition == position) return;
        if (position != null) dwellsOn.RemoveDwellerFromCell(position, this);
        position = newPosition;
        if (position != null) dwellsOn.AddDwellerToCell(position, this);
    }

	/// <summary>
    /// Like MoveToCell(CellPosition), but moves to a different position on whatever side of the
    /// world this object is already on. For example, if the object is on the Enemy side, it will
    /// try to move to the cell on the enemy side with the specified coordinates.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
    public void MoveToCell(uint x, uint z) {
        MoveToCell(new CellPosition(x, z, position.side));
    }


	public Vector3 GetSpacePosition() {
		if(position != null) {
			return dwellsOn.GetRealPosition(position);
		}
		else {
			return Vector3.zero;
		}
	}

    void OnDestroy() {
        MoveToCell(null);
    }

}
