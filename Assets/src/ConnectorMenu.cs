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
		if (GUI.Button(new Rect(350.0f, 50.0f, 250.0f, 100.0f), "Start a game"))
		{
			serverConnector.GetComponent<ServerConnector>().Host();
//			serverConnector.active = true;
//			clientConnector.active = false;
		}
		
		if (GUI.Button(new Rect(350.0f, 250.0f, 250.0f, 100.0f), "Join a game"))
		{
			clientConnector.GetComponent<ClientConnector>().FindGames();
//			serverConnector.active = false;
//			clientConnector.active = true;
		}
		
		
	}
}
