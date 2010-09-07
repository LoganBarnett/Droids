using UnityEngine;
using System.Collections;

public class ConnectorMenu : MonoBehaviour {
	public GameObject clientConnector;
	public GameObject serverConnector;
	
	string externalIp;
	bool useNat;
	
	void Start() {
//		clientConnector = GameObject.Find("ClientConnector");
//		serverConnector = GameObject.Find("ServerConnector");
		
//		serverConnector.active = clientConnector.active = false;
		Debug.Log(Application.platform);
		if (Application.platform != RuntimePlatform.OSXWebPlayer &&
		    Application.platform != RuntimePlatform.WindowsWebPlayer) {
			var dns = new WWW("http://checkip.dyndns.org");
			while (!dns.isDone && string.IsNullOrEmpty(dns.error)) {}
			var html = dns.data;
			var startIndex = html.IndexOf(": ") + 1;
			var endIndex = html.IndexOf("</body>");
			externalIp = html.Substring(startIndex, endIndex - startIndex);
		}
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	
	void OnGUI() {
		GUILayout.BeginVertical();
	    GUILayout.BeginHorizontal();
		GUILayout.Label("DROIDS");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Start a game"))
		{
			serverConnector.GetComponent<ServerConnector>().Host(useNat);
//			serverConnector.active = true;
//			clientConnector.active = false;
		}
//		GUILayout.Label("Use Nat?");
		useNat = GUILayout.Toggle(useNat, "Use Nat?");
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("Join a game"))
		{
			clientConnector.GetComponent<ClientConnector>().FindGames();
//			serverConnector.active = false;
//			clientConnector.active = true;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Your internal IP: " + Network.player.ipAddress);
//		if (GUILayout.Button("Copy IP")) {
//			
//		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Your external IP: " + externalIp);
//		if (GUILayout.Button("Copy IP")) {
//			
//		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		var instructionWidth = 200.0f;
		GUILayout.BeginArea(new Rect(Screen.width - instructionWidth, 0.0f, instructionWidth, Screen.height));
		GUILayout.BeginVertical();
		GUILayout.Label("Instructions:");
		GUILayout.Label("Use the arrow keys to move");
		GUILayout.Label("Press space to jump");
		GUILayout.Label("Press shift to shoot");
		GUILayout.EndVertical();
		GUILayout.EndArea();
		
	}
}
