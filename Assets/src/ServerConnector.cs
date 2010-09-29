using UnityEngine;
using System.Collections;

public class ServerConnector : MonoBehaviour {


	public void Host(bool useNat) {
		Debug.Log("Starting server...");
		//Network.useNat = !Network.HavePublicAddress();
		// Network.useNat = useNat;
		Network.InitializeServer(4, Networking.PORT, useNat);
		MasterServer.RegisterHost(Networking.MASTER_SERVER_TYPE, Networking.GAME_NAME, "play with me!");
		Debug.Log("Created server");
		Application.LoadLevel(Networking.STARTING_LEVEL);
	}
	
	void Update () {
	
	}
}

