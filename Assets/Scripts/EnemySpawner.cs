using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab;
	public float refreshPeriod = 0.5f;
	public int maxEnemies = 2;

	public List<GameObject> currentEnemies;

	private float timeForNextRefresh = 0f;

	private void Start() {
		SpawnEnemy();
	}

	private void Update() {
		if(Time.time > timeForNextRefresh) {
			timeForNextRefresh = Time.time + refreshPeriod;

			PruneList();

			if(currentEnemies.Count < maxEnemies) {
				SpawnEnemy();
			}
		}
	}

	private void PruneList() {
		currentEnemies.RemoveAll((obj) => obj == null);	
	}

	private void SpawnEnemy() {
		GameObject newEnemy = Instantiate(enemyPrefab, transform) as GameObject;
		currentEnemies.Add(newEnemy);
	}


}
