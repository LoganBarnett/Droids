using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientConnector : MonoBehaviour {
	bool isFindingGames;
	HostData[] data;
	List<string> log = new List<string>();
	string directIp = "";
	
	public void FindGames() {
		isFindingGames = true;
		StartCoroutine(PollHostData());
	}
	
	void Start() {
		
	}
	
	IEnumerator PollHostData() {
		while (isFindingGames) {
			Debug.Log("Polling for new games...");
//			MasterServer.ClearHostList();
			MasterServer.RequestHostList(Networking.MASTER_SERVER_TYPE);
			data = MasterServer.PollHostList();
			yield return new WaitForSeconds(1.0f);
		}
	}
		
	
	void OnGUI() {
		if (!isFindingGames) return;
		if (data == null) return;
		
		GUILayout.BeginArea(new Rect(0.0f, 150.0f, Screen.width, Screen.height - 50.0f));
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Direct Connection: ");
		directIp = GUILayout.TextField(directIp);
		if (GUILayout.Button("Join Directly")) {
			ConnectTo(new HostData() { ip = new string[] {directIp}, port = Networking.PORT});
		}
		GUILayout.EndHorizontal();
		
		var dotCount = (System.DateTime.Now.Second % 3) + 1;
		var dots = "";
		for(var i = 0; i < dotCount; ++i)
		{
			dots += ".";
		}
		GUILayout.Label(string.Format("Found {0} servers", data.Length) + dots);
		// Go through all the hosts in the host list
		foreach (var hostData in data)
		{
			GUILayout.BeginHorizontal();	
			var name = hostData.gameName + " " + hostData.connectedPlayers + " / " + hostData.playerLimit;
			GUILayout.Label(name);	
			GUILayout.Space(5);
			var hostInfo = "[";
			foreach (var host in hostData.ip)
			{
				hostInfo = hostInfo + host + ":" + hostData.port + " ";
			}
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);	
			GUILayout.Space(5);
			GUILayout.Label(hostData.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Connect"))
			{
				ConnectTo(hostData);
			}
			GUILayout.EndHorizontal();	
		}
		
		foreach(var logEntry in log) GUILayout.Label(logEntry);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	void ConnectTo(HostData hostData) {
		// Set NAT functionality based on the host information
		Network.useNat = hostData.useNat;
		if (Network.useNat)
			log.Add("Using Nat punchthrough to connect to host");
		else
			log.Add("Connecting directly to host: " + string.Join(" ", hostData.ip));
		Network.Connect(hostData.ip, hostData.port);
		Application.LoadLevel("Test");
	}
}

