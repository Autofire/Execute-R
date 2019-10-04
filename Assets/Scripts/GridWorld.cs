using System;
using UnityEngine;

public enum CellContents {
    Empty,
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

public class CellAlreadyOccupiedException: Exception {
    public CellAlreadyOccupiedException(
        CellPosition position, 
        CellContents type, 
        CellContents existing
    ) : base(String.Format(
            "Tried to place a {0} on ({1}, {2}) where there was already a {3}.", 
            type,
            position.x, 
            position.z, 
            existing
        ))
    { }
}

/// Holds data for the in-game grid, like which squares are occupied. It is not responsible for 
/// drawing this information inside the game. Instead, other objects can use helper methods from
/// this class to determine which grid squares are occupied and present graphics accordingly.
public class GridWorld : MonoBehaviour {
    public float cellSize = 1.0f, neutralZone = 4.0f;
    public int width = 4, length = 4;
    private CellContents[,] playerSideContents, enemySideContents;

    /// We need to put this in Awake() because GridDwellers will try and place themselves on the
    /// grid with Start(), so the arrays that hold that data needs to be initialized before any 
    /// Start() methods run.
    void Awake() {
        playerSideContents = new CellContents[width, length];
        enemySideContents = new CellContents[width, length];
        for (uint x = 0; x < width; x++) {
            for (uint z = 0; z < length; z++) {
                playerSideContents[x, z] = CellContents.Empty;
                enemySideContents[x, z] = CellContents.Empty;
            }
        }
    }

    /// Returns false if <c>CellPosition</c> is outside the bounds of this grid world.
    public bool IsValid(CellPosition cellPosition) {
        return cellPosition.x < width && cellPosition.z < length;
    }

    public CellContents GetContentsInCell(CellPosition position) {
        if (!IsValid(position)) {
            throw new InvalidCellPositionException(position, this);
        }
        if (position.side == GridClass.PlayerGrid) {
            return playerSideContents[position.x, position.z];
        } else {
            return enemySideContents[position.x, position.z];
        }
    }

    public bool IsCellEmpty(CellPosition position) {
        return GetContentsInCell(position) == CellContents.Empty;
    }

    /// This method should only be used by GridDweller. Use that component to represent something on
    /// the grid. Never interact with this method directly. Throws an exception if trying to set
    /// a non-emtpy cell to something non-empty, or if trying to set an empty cell as empty again.
    public void SetContentsInCell(CellPosition position, CellContents contents) {
        CellContents existing = GetContentsInCell(position);
        if (
            existing == CellContents.Empty && contents == CellContents.Empty
            || existing != CellContents.Empty && contents != CellContents.Empty
        ) {
            throw new CellAlreadyOccupiedException(position, contents, existing);
        }
        if (position.side == GridClass.PlayerGrid) {
            playerSideContents[position.x, position.z] = contents;
        } else {
            enemySideContents[position.x, position.z] = contents;
        }
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
                    CellContents c = playerSideContents[x, z];
                    if (c == CellContents.Player) DrawDiamondGizmo(
                        playerGridCorner + cellWidth * x + cellLength * z, Color.blue
                    ); else if (c == CellContents.Enemy) DrawDiamondGizmo(
                        playerGridCorner + cellWidth * x + cellLength * z, Color.red
                    );
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
    }

    void DrawDiamondGizmo(Vector3 position, Color color) {
        Vector3 x10 = Vector3.right * cellSize, z10 = Vector3.forward * cellSize;
        Vector3 x2 = x10 / 5, x5 = x10 / 2, z2 = z10 / 5, z5 = z10 / 2;

        Gizmos.color = color;
        Gizmos.DrawLine(position + x2 + z5, position + x5 + z10 - z2);
        Gizmos.DrawLine(position + x10 - x2 + z5, position + x5 + z10 - z2);
        Gizmos.DrawLine(position + x10 - x2 + z5, position + x5 + z2);
        Gizmos.DrawLine(position + x2 + z5, position + x5 + z2);
    }
}
