using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkedPlayerSpawner : MonoBehaviour {
	public GameObject playerCharacterPrefab;
	public GameObject spawnParent;
	
	
	private Transform[] spawnPoints;
	private GameObject playerCharacter;
	private NetworkPlayer player;
	public NetworkPlayer Player {
		get;
		set;
	}
	
	void Start () {
		spawnPoints = spawnParent.GetComponentsInChildren<Transform>().Where(s => s.gameObject != spawnParent).ToArray();
		Debug.Log("points: " + spawnPoints.Length);
//		Debug.Log("Connected? " + (Network.isClient || Network.isServer));
//		Debug.Log("Level started, creating my player");
		if (Network.isServer) {
			var spawnPosition = GetSpawnPosition();
			Debug.Log("Spawning player at: " + spawnPosition.position);
			playerCharacter = (GameObject)Network.Instantiate(playerCharacterPrefab, spawnPosition.position, spawnPosition.rotation, 0);
		}
//		Debug.Log("Null? " + playerCharacter == null);
//		playerCharacter.GetComponent<NetworkControlledPlayer>().Player = Network.player;
		
	}
	
//	void Update () {
//		if (Mathf.Abs(Input.GetAxis("Move")) > 0.0f)
//		{
//			playerCharacter.transform.Translate(Vector3.right * Input.GetAxis("Move") * Time.deltaTime * movementSpeed);
//		}
//	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("A player connected, creating their character");
//		playerCharacter = (GameObject)Instantiate(playerCharacterPrefab, Vector3.up, Quaternion.identity);
//		var component = playerCharacter.GetComponent<NetworkControlledPlayer>().playerCharacter.GetComponent<NetworkView>();
//		component
		
//		playerCharacter.GetComponent<NetworkControlledPlayer>().Player = player;
	}
	
	void OnServerInitialized() {
		Debug.Log("Started server, spawning my character");
		var spawnPosition = GetSpawnPosition();
		Debug.Log("spawn point: " + spawnPosition.position);
		playerCharacter = (GameObject)Network.Instantiate(playerCharacterPrefab, spawnPosition.position, spawnPosition.rotation, 0);
//		playerCharacter.GetComponent<NetworkControlledPlayer>().Player = Network.player;
	}
	
	void OnConnectedToServer() {
		Debug.Log("Connected to server, spawning my character");
		var spawnPosition = GetSpawnPosition();
		playerCharacter = (GameObject)Network.Instantiate(playerCharacterPrefab, spawnPosition.position, spawnPosition.rotation, 0);
//		playerCharacter.GetComponent<NetworkControlledPlayer>().Player = Network.player;
	}
	
	Transform GetSpawnPosition() {
		return spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
	}
}

