using System;
using System.Collections.Generic;
using UnityEngine;

public enum DwellerType {
    Player,
    Enemy
}

public enum GridClass {
    PlayerGrid,
    EnemyGrid
}

[Serializable]
public class CellPosition {
    public uint x, z;
    public GridClass side;

    public CellPosition() {
        this.x = 0;
        this.z = 0;
        this.side = GridClass.PlayerGrid;
    }

    public CellPosition(uint x, uint z, GridClass side) {
        this.x = x;
        this.z = z;
        this.side = side;
    }
}

public class InvalidCellPositionException : Exception {
    public InvalidCellPositionException(CellPosition position, GridWorld world)
        : base(String.Format(
            "Grid position ({0},{1}) is outside the bounds of ({2},{3})", 
            position.x, 
            position.z, 
            world.width, 
            world.length
        ))
    { }
}

/// Holds data for the in-game grid, like which squares are occupied. It is not responsible for 
/// drawing this information inside the game. Instead, other objects can use helper methods from
/// this class to determine which grid squares are occupied and present graphics accordingly.
public class GridWorld : MonoBehaviour {
    public float cellSize = 1.0f, neutralZone = 4.0f;
    public int width = 4, length = 4;
    private List<GridDweller>[,] playerSideContents, enemySideContents;
    private static GridWorld instance;

    public static GridWorld getInstance() {
        if (instance == null) {
            throw new NullReferenceException("No grid world exists in this level.");
        }
        return instance;
    }

    /// We need to put this in Awake() because GridDwellers will try and place themselves on the
    /// grid with Start(), so the arrays that hold that data needs to be initialized before any 
    /// Start() methods run.
    void Awake() {
        instance = this;
        playerSideContents = new List<GridDweller>[width, length];
        enemySideContents = new List<GridDweller>[width, length];
        for (uint x = 0; x < width; x++) {
            for (uint z = 0; z < length; z++) {
                playerSideContents[x, z] = new List<GridDweller>();
                enemySideContents[x, z] = new List<GridDweller>();
            }
        }
    }

    /// Returns false if <c>CellPosition</c> is outside the bounds of this grid world.
    public bool IsValid(CellPosition cellPosition) {
        return cellPosition.x < width && cellPosition.z < length;
    }

    public int ItemCountInCell(CellPosition position) {
        if (!IsValid(position)) {
            throw new InvalidCellPositionException(position, this);
        }
        if (position.side == GridClass.PlayerGrid) {
            return playerSideContents[position.x, position.z].Count;
        } else {
            return enemySideContents[position.x, position.z].Count;
        }
    }

    public bool IsTypeInCell(CellPosition position, DwellerType type) {
        if (!IsValid(position)) {
            throw new InvalidCellPositionException(position, this);
        }
        if (position.side == GridClass.PlayerGrid) {
            return playerSideContents[position.x, position.z].Exists(
                delegate(GridDweller dweller) { return dweller.type == type; }
            );
        } else {
            return enemySideContents[position.x, position.z].Exists(
                delegate(GridDweller dweller) { return dweller.type == type; }
            );
        }
    }

    public bool IsCellEmpty(CellPosition position) {
        return ItemCountInCell(position) == 0;
    }

    /// This method should only be used by GridDweller. Use that component to represent something on
    /// the grid. Never interact with this method directly. Throws an exception if trying to set
    /// a non-emtpy cell to something non-empty, or if trying to set an empty cell as empty again.
    public void AddDwellerToCell(CellPosition position, GridDweller dweller) {
        if (position.side == GridClass.PlayerGrid) {
            playerSideContents[position.x, position.z].Add(dweller);
        } else {
            enemySideContents[position.x, position.z].Add(dweller);
        }
    }

    /// This method should only be used by GridDweller. Use that component to represent something on
    /// the grid. Never interact with this method directly. Throws an exception if trying to set
    /// a non-emtpy cell to something non-empty, or if trying to set an empty cell as empty again.
    public void RemoveDwellerFromCell(CellPosition position, GridDweller dweller) {
        if (position.side == GridClass.PlayerGrid) {
            playerSideContents[position.x, position.z].Remove(dweller);
        } else {
            enemySideContents[position.x, position.z].Remove(dweller);
        }
    }

    /// Returns a list containing one CellPosition for every cell in the grid world.
    public List<CellPosition> ListAllCells() {
        List<CellPosition> result = new List<CellPosition>();
        for (uint x = 0; x < width; x++) {
            for (uint z = 0; z < length; z++) {
                result.Add(new CellPosition(x, z, GridClass.PlayerGrid));
                result.Add(new CellPosition(x, z, GridClass.EnemyGrid));
            }
        }
        return result;
    }

    /// Returns a Vector3 indicating the real-world position of the center of the specified cell.
    /// Throws an InvalidCellPositionException if the provided grid position is outside the bounds
    /// of this grid world.
    public Vector3 GetRealPosition(CellPosition cellPosition) {
        if (!IsValid(cellPosition)) {
            throw new InvalidCellPositionException(cellPosition, this);
        }
        Vector3 position = Vector3.zero;
        if (cellPosition.side == GridClass.PlayerGrid) {
            // Player grid is in the -Z direction.
            position += Vector3.back * (neutralZone / 2 + cellSize * length);
        } else {
            // Enemy grid is in the +Z direction.
            position += Vector3.forward * neutralZone / 2;
        }
        position += Vector3.left * (cellSize * width / 2);
        // Position is now at the negative X, Z corner of whichever side cellPosition is on.

        // Add in an offset based on the actual x and z coordinates of cellPosition. At the same
        // time, add an offset of (cellSize / 2) so that we are centered in the middle of the cell.
        position += Vector3.right * (cellSize * cellPosition.x + cellSize / 2);
        position += Vector3.forward * (cellSize * cellPosition.z + cellSize / 2);

        return position;
    }

    /// Attempts to determine which cell the given world position lies in. If it is not inside the
    /// bounds of either the player or enemy grid, null is returned instead.
    public CellPosition GridizeRealPosition(Vector3 realPosition) {
        bool isOnEnemyGrid = realPosition.z > 0; // Enemy grid is in the +z direction.
        // Reorient the coordinate system so that 0, 0 is at the 0, 0 corner of the enemy grid
        if (isOnEnemyGrid) { 
            realPosition -= Vector3.forward * (neutralZone / 2);
            realPosition += Vector3.right * (cellSize * width / 2);
        } else { // Player grid is in the -z direction;
            realPosition -= Vector3.back * (neutralZone / 2);
            realPosition -= Vector3.back * (cellSize * length);
            realPosition += Vector3.right * (cellSize * width / 2);
        }
        // Scale the coordinate system so each cell is one square unit.
        realPosition /= cellSize;
        // Floor to find out which integer cell we are in.
        int x = Mathf.FloorToInt(realPosition.x), z = Mathf.FloorToInt(realPosition.z);
        // If we are out of bounds, return false.
        if (x < 0 || z < 0 || x >= width || z >= width) {
            return null;
        }
        // Otherwise, package up the coordinates we found.
        return new CellPosition(
            //     V         V  We already checked that they are >= 0, so this cast is safe.
            (uint) x, (uint) z, isOnEnemyGrid ? GridClass.EnemyGrid : GridClass.PlayerGrid
        );
    }

    void OnDrawGizmos() {
        Vector3 neutralZoneOffset = Vector3.forward * (neutralZone / 2);
        Vector3 totalLength = Vector3.forward * (cellSize * length);
        Vector3 totalWidth = Vector3.right * (cellSize * width);
        Vector3 cellWidth = Vector3.right * cellSize;
        Vector3 cellLength = Vector3.forward * cellSize;

        // Negative X, negative Z corner of the player's grid.
        Vector3 playerGridCorner = -neutralZoneOffset - totalLength - totalWidth / 2;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            playerGridCorner, 
            playerGridCorner + (Vector3.left + Vector3.back) * (cellSize / 5)
        );
        // <= because we also want to draw a line on the right-hand side of the last grid cell.
        for (int x = 0; x <= width; x++) {
            Gizmos.DrawLine(
                playerGridCorner + cellWidth * x,
                playerGridCorner + cellWidth * x + totalLength
            );
        }
        for (int z = 0; z <= length; z++) {
            Gizmos.DrawLine(
                playerGridCorner + cellLength * z,
                playerGridCorner + cellLength * z + totalWidth
            );
        }
        if (playerSideContents != null) {
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {
                    Vector3 pos = playerGridCorner + cellWidth * x + cellLength * z;
                    pos += (Vector3.right + Vector3.forward) * (cellSize / 2);
                    foreach (GridDweller dweller in playerSideContents[x, z]) {
                        DrawDwellerTypeGizmo(pos, dweller.type);
                        // Stack multiple items on the same cell.
                        pos += Vector3.up * 0.5f;
                    }
                }
            }
        }
        
        // Negative X, negative Z corner of the enemy grid.
        Vector3 enemyGridCorner = neutralZoneOffset - totalWidth / 2;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            enemyGridCorner, 
            enemyGridCorner + (Vector3.left + Vector3.back) * (cellSize / 5)
        );
        // <= because we also want to draw a line on the right-hand side of the last grid cell.
        for (int x = 0; x <= width; x++) {
            Gizmos.DrawLine(
                enemyGridCorner + cellWidth * x,
                enemyGridCorner + cellWidth * x + totalLength
            );
        }
        for (int z = 0; z <= length; z++) {
            Gizmos.DrawLine(
                enemyGridCorner + cellLength * z,
                enemyGridCorner + cellLength * z + totalWidth
            );
        }
        if (enemySideContents != null) {
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {
                    Vector3 pos = enemyGridCorner + cellWidth * x + cellLength * z;
                    pos += (Vector3.right + Vector3.forward) * (cellSize / 2);
                    foreach (GridDweller dweller in enemySideContents[x, z]) {
                        DrawDwellerTypeGizmo(pos, dweller.type);
                        // Stack multiple items on the same cell.
                        pos += Vector3.up * 0.5f;
                    }
                }
            }
        }
    }

    void DrawDwellerTypeGizmo(Vector3 position, DwellerType type) {
        if (type == DwellerType.Player) {
            Gizmos.color = Color.blue;
        } else if (type == DwellerType.Enemy) {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(position, position + Vector3.up * 5);
        Gizmos.DrawCube(position + Vector3.up * 3, new Vector3(0.2f, 0.2f, 0.2f));
    }
}
