using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DroidColor : MonoBehaviour {
//	List<Color> colors;
	Color droidColor = Color.black;
	
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
		droidColor = color;
	}
	
	
	void OnPlayerConnected(NetworkPlayer player) {
		if (Network.isClient) return;
		networkView.RPC("SetColor", player, droidColor.r, droidColor.g, droidColor.b, droidColor.a);
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			stream.Serialize(ref droidColor.r);
			stream.Serialize(ref droidColor.g);
			stream.Serialize(ref droidColor.b);
			stream.Serialize(ref droidColor.a);
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
