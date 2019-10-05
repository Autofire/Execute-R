using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PhysicalCell {
    public Vector3 position;
    public GameObject playerIndicator = null;
    public GameObject enemyIndicator = null;
    public ParticleSystem playerEffect = null;
    public ParticleSystem enemyEffect = null;
}

public class PhysicalGrid : MonoBehaviour {
    [Tooltip(
        "This prefab will be instantiated for every single cell in the grid. The prefab should be "
        + "1 unit big, and will be scaled to the actual size of the grid."
    )]
    public GameObject cellPrefab;
    [Tooltip(
        "This prefab will be instantiated for every cell containing a player. The prefab should be "
        + "1 unit big, and will be scaled to the actual size of the grid."
    )]
    public GameObject playerIndicator;
    [Tooltip(
        "This prefab will be instantiated for every cell containing an enemy. The prefab should be "
        + "1 unit big, and will be scaled to the actual size of the grid."
    )]
    public GameObject enemyIndicator;
    public GameObject optionalPlayerEffect = null;
    public GameObject optionalEnemyEffect = null;

    List<CellPosition> allCells;
    List<PhysicalCell> physicalCells = new List<PhysicalCell>();
    GridWorld gridWorld;

    // Handles scaling the prefab to match the grid world's cell size.
    GameObject Instantiate(GameObject original, Vector3 position) {
        GameObject instance = Instantiate(original, position, Quaternion.identity, gameObject.transform);
        instance.transform.localScale = Vector3.one * gridWorld.cellSize;
        return instance;
    }

    void Start() {
        gridWorld = GridWorld.getInstance();
        allCells = gridWorld.ListAllCells();
        foreach (CellPosition cell in allCells) {
            PhysicalCell physicalCell = new PhysicalCell();
            physicalCell.position = gridWorld.GetRealPosition(cell);
            Instantiate(cellPrefab, physicalCell.position);

            if (optionalPlayerEffect != null) {
                GameObject playerEffect = Instantiate(optionalPlayerEffect, physicalCell.position);
                physicalCell.playerEffect = playerEffect.GetComponentInChildren<ParticleSystem>();
                physicalCell.playerEffect.Stop();
            }

            if (optionalEnemyEffect != null) {
                GameObject enemyEffect = Instantiate(optionalEnemyEffect, physicalCell.position);
                physicalCell.enemyEffect = enemyEffect.GetComponentInChildren<ParticleSystem>();
                physicalCell.enemyEffect.Stop();
            }

            physicalCells.Add(physicalCell);
        }
    }

    void Update() {
        for (int i = 0; i < allCells.Count; i++) {
            CellPosition cellPosition = allCells[i];
            PhysicalCell physicalCell = physicalCells[i];

            if (gridWorld.IsTypeInCell(cellPosition, DwellerType.Player)) {
                if (physicalCell.playerIndicator == null) {
                    physicalCell.playerIndicator = 
                        Instantiate(playerIndicator, physicalCell.position);
                    if (physicalCell.playerEffect != null) {
                        physicalCell.playerEffect.Play();
                    }
                }
            } else {
                if (physicalCell.playerIndicator != null) {
                    Destroy(physicalCell.playerIndicator);
                    physicalCell.playerIndicator = null;
                    if (physicalCell.playerEffect != null) {
                        physicalCell.playerEffect.Stop();
                    }
                }
            }

            if (gridWorld.IsTypeInCell(cellPosition, DwellerType.Enemy)) {
                if (physicalCell.enemyIndicator == null) {
                    physicalCell.enemyIndicator = 
                        Instantiate(enemyIndicator, physicalCell.position);
                    if (physicalCell.enemyEffect != null) {
                        physicalCell.enemyEffect.Play();
                    }
                }
            } else {
                if (physicalCell.enemyIndicator != null) {
                    Destroy(physicalCell.enemyIndicator);
                    physicalCell.enemyIndicator = null;
                    if (physicalCell.enemyEffect != null) {
                        physicalCell.enemyEffect.Stop();
                    }
                }
            }
        }
    }
}
