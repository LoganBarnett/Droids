using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NetworkedPlayerSpawner : MonoBehaviour {
	public GameObject playerCharacterPrefab;
	public GameObject spawnParent;
	public float respawnDelayInSeconds = 5.0f;
	public List<Color> playerColors = new List<Color>();
	
	
	private int lastColorIndex = 0;
	private Transform[] spawnPoints;
	private GameObject playerCharacter;
	public List<GameObject> Droids {get; private set;}
	public Dictionary<string, Color> networkPlayerColors = new Dictionary<string, Color>();
	
	
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
		Droids.Remove(droid);
		
		if (droid.networkView.isMine) Camera.main.transform.parent = droid.transform;
		
		if (Network.isServer) {
			
			StartCoroutine(DelayedDroidRespawn(respawnDelayInSeconds, droid.networkView.owner));
		}
	}
	
	IEnumerator DelayedDroidRespawn(float delayInSeconds, NetworkPlayer player) {
		yield return new WaitForSeconds(delayInSeconds);
		
		var color = GetColorForPlayer(player.guid);
		Debug.Log(string.Format("Telling player '{0}' to respawn", player.guid));
		Debug.Log(string.Format("I am player    '{0}'", Network.player.guid));
		if (Network.player.guid == player.guid) {
			SpawnDroid(color);
		} else {
			networkView.RPC("SpawnDroid", player, color.r, color.g, color.b, color.a);
		}
	}
	
	void Start () {
		Droids = new List<GameObject>();
		spawnPoints = spawnParent.GetComponentsInChildren<Transform>().Where(s => s.gameObject != spawnParent).ToArray();
		Debug.Log("points: " + spawnPoints.Length);
		if (Network.isServer) {
			SpawnDroid(GetColorForPlayer(Network.player.guid));
		}
	}
	
	void OnServerInitialized() {
		Debug.Log("Started server, spawning my character");
		SpawnDroid(GetColorForPlayer(Network.player.guid));
	}
	
	void OnConnectedToServer() {
		Debug.Log("Connected to server");
	}
	
	Transform GetSpawnPosition() {
		return spawnPoints[Random.Range(0, spawnPoints.Length)];
	}
	
	
	[RPC]
	void SpawnDroid(float r, float g, float b, float a) {
		SpawnDroid(new Color(r, g, b, a));
	}
	
	private void SpawnDroid(Color color) {
		if (Droids.Any(d => d.networkView.isMine)) {
			Debug.Log("Attempted to spawn duplicate droid.");
			return;
		}
		
		var spawnPosition = GetSpawnPosition();
		var droid = (GameObject)Network.Instantiate(playerCharacterPrefab, spawnPosition.position, spawnPosition.rotation, 0);
		droid.GetComponent<DroidLife>().Spawner = this;
		Droids.Add(droid);
		Camera.main.transform.position = new Vector3(droid.transform.position.x, droid.transform.position.y, Camera.main.transform.position.z);
		Camera.main.transform.parent = droid.transform;
		
		Debug.Log("Setting color to " + color);
		droid.GetComponent<DroidColor>().SetColor(color);
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		if (Network.isClient) return;
		
		var color = GetColorForPlayer(player.guid);
		networkView.RPC("SpawnDroid", player, color.r, color.g, color.b, color.a);
	}
	
	public Color UseColor() {
		if (Network.isClient) throw new System.Exception("Client cannot UseColor!");
		var color = playerColors[lastColorIndex++];
		if (lastColorIndex >= playerColors.Count) lastColorIndex = 0;
		return color;
	}
	
	public Color GetColorForPlayer(string playerGuid) {
		if (networkPlayerColors.ContainsKey(playerGuid)) {
			return networkPlayerColors[playerGuid];
		} else {
			var color = networkPlayerColors[playerGuid] = UseColor();
			return color;
		}
	}
}

