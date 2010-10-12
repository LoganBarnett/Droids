using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DroidColor : MonoBehaviour {
//	List<Color> colors;
	Color playerColor = Color.black;
	
	public Color PlayerColor {
		get { return playerColor; }
		set { playerColor = value; }
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
//		var color = GameObject.Find("Networking").GetComponent<NetworkedPlayerSpawner>().GetColorForPlayer(networkView.owner.guid);
//		SetColor(color);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	[RPC]
	public void SetColor(float r, float g, float b, float a) {
		SetColor(new Color(r, g, b, a));
	}
	
	public void SetColor(Color color) {
		var controller = GetComponent<PlayerDroidController>();
		var childRenderers = controller.model.GetComponentsInChildren<Renderer>();
			
		foreach (var childRenderer in childRenderers) {
			childRenderer.material.SetColor("_Color", color);
		}
		playerColor = color;
	}
	
	
	void OnPlayerConnected(NetworkPlayer player) {
		if (Network.isClient) return;
		networkView.RPC("SetColor", player, playerColor.r, playerColor.g, playerColor.b, playerColor.a);
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			stream.Serialize(ref playerColor.r);
			stream.Serialize(ref playerColor.g);
			stream.Serialize(ref playerColor.b);
			stream.Serialize(ref playerColor.a);
		}
		else {
			Color newColor = Color.black;
			stream.Serialize(ref newColor.r);
			stream.Serialize(ref newColor.g);
			stream.Serialize(ref newColor.b);
			stream.Serialize(ref newColor.a);
			
			SetColor(newColor);
		}
	}
}
