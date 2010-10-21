using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class StationSpawner : MonoBehaviour {
//	public GameObject[] spawnPoints;
	public int initialStationCount;
	public GameObject stationPrefab;
	public GameObject spawnPointParent;
	
	List<GameObject> remainingSpawnPoints;
	
	void Start() {
		var spawnPoints = new List<GameObject>();
		foreach (Transform t in spawnPointParent.transform) { spawnPoints.Add(t.gameObject); }
		remainingSpawnPoints = spawnPoints.ToList();
		SpawnInitialStations();
	}

//	void Update() {
//
//	}
	
	public void SpawnStations(int stationCount) {
		for (var i = 0; i < stationCount; ++i) {
			var randomSpawnIndex = Random.Range(0, remainingSpawnPoints.Count - 1);
			var randomSpawn = remainingSpawnPoints[randomSpawnIndex];
			remainingSpawnPoints.RemoveAt(randomSpawnIndex);
			
			Network.Instantiate(stationPrefab, randomSpawn.transform.position, Quaternion.identity, 0);
		}
	}
	
	void SpawnInitialStations() {
		if (Network.isClient) return;
		
		SpawnStations(initialStationCount);
	}
}
