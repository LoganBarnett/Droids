using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NetworkedPlayerSpawner : MonoBehaviour {
	public GameObject playerCharacterPrefab;
	public GameObject spawnParent;
	public float respawnDelayInSeconds = 5.0f;
	
	
	private Transform[] spawnPoints;
	private GameObject playerCharacter;
	public List<GameObject> Droids {get; private set;}
	
	
	private NetworkPlayer player;
	public NetworkPlayer Player {
		get;
		set;
	}
	
	public void RegisterDroid(GameObject droid) {
		if (droid.networkView.isMine) return;
		Droids.Add(droid);	
	}
	
	public void DroidDied(GameObject droid) {
		if (droid.networkView.isMine) {
			Camera.main.transform.parent = droid.transform;
			StartCoroutine(DelayedDroidRespawn(respawnDelayInSeconds));
		}
		Droids.Remove(droid);
	}
	
	IEnumerator DelayedDroidRespawn(float delayInSeconds) {
		yield return new WaitForSeconds(delayInSeconds);
		SpawnDroid();
	}
	
	void Start () {
		Droids = new List<GameObject>();
		spawnPoints = spawnParent.GetComponentsInChildren<Transform>().Where(s => s.gameObject != spawnParent).ToArray();
		Debug.Log("points: " + spawnPoints.Length);
//		Debug.Log("Connected? " + (Network.isClient || Network.isServer));
//		Debug.Log("Level started, creating my player");
		if (Network.isServer) {
			SpawnDroid();
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("A player connected, creating their character");
	}
	
	void OnServerInitialized() {
		Debug.Log("Started server, spawning my character");
		SpawnDroid();
	}
	
	void OnConnectedToServer() {
		Debug.Log("Connected to server, spawning my character");
		SpawnDroid();
	}
	
	Transform GetSpawnPosition() {
		return spawnPoints[Random.Range(0, spawnPoints.Length)];
	}
	
	
	
	private void SpawnDroid() {
		var spawnPosition = GetSpawnPosition();
		var droid = (GameObject)Network.Instantiate(playerCharacterPrefab, spawnPosition.position, spawnPosition.rotation, 0);
		droid.GetComponent<DroidLife>().Spawner = this;
		Droids.Add(droid);
		Camera.main.transform.position = new Vector3(droid.transform.position.x, droid.transform.position.y, Camera.main.transform.position.z);
		Camera.main.transform.parent = droid.transform;
	}
}

