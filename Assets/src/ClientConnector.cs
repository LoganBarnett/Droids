using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientConnector : MonoBehaviour {
	bool isFindingGames;
	List<string> log = new List<string>();
	
	public void FindGames() {
		MasterServer.RequestHostList(Networking.MASTER_SERVER_TYPE);
		isFindingGames = true;
	}
	
	void Update () {
	
	}
	
	void OnGUI() {
		if (!isFindingGames) return;
		var data = MasterServer.PollHostList();
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
				// Set NAT functionality based on the host information
				Network.useNat = hostData.useNat;
				if (Network.useNat)
					log.Add("Using Nat punchthrough to connect to host");
				else
					log.Add("Connecting directly to host: " + string.Join(" ", hostData.ip));
				Network.Connect(hostData.ip, hostData.port);	
				Application.LoadLevel("Test");
			}
			GUILayout.EndHorizontal();	
		}
		
		foreach(var logEntry in log) GUILayout.Label(logEntry);
	}
}

