using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DroidColor : MonoBehaviour {
	List<Color> colors;
	Dictionary<string, Color> playerColors = new Dictionary<string, Color>();
	Color myColor;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
		var color = GameObject.Find("Networking").GetComponent<NetworkedPlayerSpawner>().GetColorForPlayer(networkView.owner.guid);
		SetColor(color);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
//	[RPC]
//	void SetColorForClient(float r, float g, float b, float a) {
//		if (Network.isClient) { throw new System.Exception("Invoked client RPC inside server"); }
//		var color = new Color(r, g, b, a);
//		Debug.Log("Setting local color to " + color);
//		SetColor(color);
//	}
//	
//	[RPC]
//	void GetColorFromServer(string playerGuid) {
//		if (Network.isClient) { throw new System.Exception("Invoked server RPC inside client"); }
//		if (string.IsNullOrEmpty(playerGuid)) return;
//		Color color = GetColorFor(playerGuid);
//		
//		Debug.Log("Server setting color for player " + playerGuid + " to " + color);
//		
//		var player = Network.connections.First(p => p.guid == playerGuid);
//		networkView.RPC("SetColorForClient", player, color.r, color.g, color.b, color.a);
//	}
	
	public void SetColor(Color color) {
		var controller = GetComponent<PlayerDroidController>();
		var childRenderers = controller.model.GetComponentsInChildren<Renderer>();
			
		foreach (var childRenderer in childRenderers) {
			childRenderer.material.SetColor("_Color", color);
		}
		myColor = color;
	}
	
//	public Color GetColorFor(string playerGuid) {
//		Color color;
//		try {
//			color = playerColors[playerGuid];
//			Debug.Log("Using previously set color " + color + " for " + playerGuid);
//		} catch {
//			color = colors[0];
//			playerColors[playerGuid] = color;
//			colors.RemoveAt(0);
//			colors.Add(color);
//		}
//		return color;
//	}
}
