using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PhysicalCell {
    public Vector3 position;
	/*
    public GameObject playerIndicator = null;
    public GameObject enemyIndicator = null;
    public ParticleSystem playerEffect = null;
    public ParticleSystem enemyEffect = null;
	*/
	public class TypeSpecificData {
		public GameObject indicator;
		public ParticleSystem effect;
	}

	public Dictionary<DwellerType, TypeSpecificData> data;

	public PhysicalCell() {
		data = new Dictionary<DwellerType, TypeSpecificData>();
	}
}


public class PhysicalGrid : MonoBehaviour {

	[System.Serializable]
	public struct DwellerInfo {
		public DwellerType type;
		public GameObject indicatorPrefab;
		public GameObject optionalEffect;
	}

    [Tooltip(
        "This prefab will be instantiated for every single cell in the grid. The prefab should be "
        + "1 unit big, and will be scaled to the actual size of the grid."
    )]
    public GameObject cellPrefab;
	/*
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
	*/
	public DwellerInfo[] dwellerInfoSet;

    private List<CellPosition> allCells;
    private List<PhysicalCell> physicalCells = new List<PhysicalCell>();
    private GridWorld gridWorld;

	/// <summary>
    /// Handles scaling the prefab to match the grid world's cell size.
	/// </summary>
	/// <param name="original"></param>
	/// <param name="position"></param>
	/// <returns></returns>
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

			/*
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
			*/

			foreach(DwellerInfo info in dwellerInfoSet) {
				physicalCell.data.Add(info.type, new PhysicalCell.TypeSpecificData());

				if(info.optionalEffect != null) {
					GameObject effect = Instantiate(info.optionalEffect, physicalCell.position);
					physicalCell.data[info.type].effect = effect.GetComponentInChildren<ParticleSystem>();
					physicalCell.data[info.type].effect.Stop();
				}
			}

            physicalCells.Add(physicalCell);
        }
    }

    void Update() {
        for (int i = 0; i < allCells.Count; i++) {
            CellPosition cellPosition = allCells[i];
            PhysicalCell physicalCell = physicalCells[i];

			foreach(DwellerInfo generalTypeInfo in dwellerInfoSet) {
				if (gridWorld.IsTypeInCell(cellPosition, generalTypeInfo.type)) {
					if (physicalCell.data[generalTypeInfo.type].indicator == null) {
						physicalCell.data[generalTypeInfo.type].indicator = 
							Instantiate(generalTypeInfo.indicatorPrefab, physicalCell.position);
						if (physicalCell.data[generalTypeInfo.type].effect != null) {
							physicalCell.data[generalTypeInfo.type].effect.Play();
						}
					}
				} else {
					if (physicalCell.data[generalTypeInfo.type].indicator != null) {
						Destroy(physicalCell.data[generalTypeInfo.type].indicator);
						physicalCell.data[generalTypeInfo.type].indicator = null;
						if (physicalCell.data[generalTypeInfo.type].effect != null) {
							physicalCell.data[generalTypeInfo.type].effect.Stop();
						}
					}
				}
			}

        }
    }
}
