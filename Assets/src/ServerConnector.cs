using UnityEngine;
using System.Collections;

public class ServerConnector : MonoBehaviour {


	public void Host() {
		Debug.Log("Starting server...");
		Network.useNat = !Network.HavePublicAddress();
		Network.InitializeServer(4, Networking.PORT);
		MasterServer.RegisterHost(Networking.MASTER_SERVER_TYPE, Networking.GAME_NAME, "play with me!");
		Debug.Log("Created server");
		Application.LoadLevel("Test");
	}
	
	void Update () {
	
	}
}

