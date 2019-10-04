using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWorld : MonoBehaviour
{
    public float cellSize = 1.0f, neutralZone = 4.0f;
    public int width = 4, length = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Vector3 neutralZoneOffset = Vector3.forward * (neutralZone / 2);
        Vector3 totalLength = Vector3.forward * (cellSize * length);
        Vector3 totalWidth = Vector3.right * (cellSize * width);
        Vector3 cellWidth = Vector3.right * cellSize;
        Vector3 cellLength = Vector3.forward * cellSize;

        // Negative X, negative Z corner of the player's grid.
        Vector3 playerGridCorner = -neutralZoneOffset - totalLength - totalWidth / 2;
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

        // Negative X, negative Z corner of the enemy grid.
        Vector3 enemyGridCorner = neutralZoneOffset - totalWidth / 2;
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
}
