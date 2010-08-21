using UnityEngine;
using System.Collections.Generic;

public class NetworkNotifier : MonoBehaviour {
	private NetworkPlayer[] players;
	private List<string> networkLog = new List<string>();
	
	// Use this for initialization
	void Start () {
//		players = Network.connections;
		if (Network.isServer) {
			networkLog.Add("Hosting game");
		}
		else {
			networkLog.Add("Joined game");
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if (players.Length > Network.connections.Length) {
//			networkLog.Add("A player left the game.");
//		}
//		else if (players.Length < Network.connections.Length) {
//			networkLog.Add("A player has joined the game.");	
//		}
//		
//		players = Network.connections;
		
		if (networkLog.Count > 5)
		{
			networkLog.RemoveAt(0);
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("Player connected");
		networkLog.Add("A player has joined the game.");
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player left");
		networkLog.Add("A player has left the game.");	
	}
	
	void OnGUI() {
		GUILayout.BeginVertical();
		networkLog.ForEach(nl => {
			GUILayout.Label(nl);
			GUILayout.Space(5); 
		} );
		
		GUILayout.EndVertical();
	}
}

