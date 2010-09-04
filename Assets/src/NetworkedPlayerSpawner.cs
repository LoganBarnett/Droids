using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkedPlayerSpawner : MonoBehaviour {
	public GameObject playerCharacterPrefab;
	public GameObject spawnParent;
	public float respawnDelayInSeconds = 5.0f;
	
	
	private Transform[] spawnPoints;
	private GameObject playerCharacter;
	private NetworkPlayer player;
	public NetworkPlayer Player {
		get;
		set;
	}
	
	public void DroidDied(GameObject droid) {
		if (droid.networkView.isMine) {
			StartCoroutine(DelayedDroidRespawn(respawnDelayInSeconds));
		}
	}
	
	IEnumerator DelayedDroidRespawn(float delayInSeconds) {
		yield return new WaitForSeconds(delayInSeconds);
		SpawnDroid();
	}
	
	void Start () {
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
		return spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
	}
	
	private void SpawnDroid() {
		var spawnPosition = GetSpawnPosition();
		playerCharacter = (GameObject)Network.Instantiate(playerCharacterPrefab, spawnPosition.position, spawnPosition.rotation, 0);
		playerCharacter.GetComponent<DroidLife>().Spawner = this;
	}
}

