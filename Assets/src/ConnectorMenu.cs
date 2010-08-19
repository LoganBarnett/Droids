using UnityEngine;
using System.Collections;

public class ConnectorMenu : MonoBehaviour {
	public GameObject clientConnector;
	public GameObject serverConnector;
	
	void Start() {
//		clientConnector = GameObject.Find("ClientConnector");
//		serverConnector = GameObject.Find("ServerConnector");
		
//		serverConnector.active = clientConnector.active = false;
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	void OnGUI() {
		GUILayout.BeginVertical();
		if (GUILayout.Button("Start a game"))
		{
			serverConnector.GetComponent<ServerConnector>().Host();
//			serverConnector.active = true;
//			clientConnector.active = false;
		}
		
		if (GUILayout.Button("Join a game"))
		{
			clientConnector.GetComponent<ClientConnector>().FindGames();
//			serverConnector.active = false;
//			clientConnector.active = true;
		}
		
		GUILayout.EndVertical();
	}
}
