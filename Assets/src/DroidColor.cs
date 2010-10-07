using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DroidColor : MonoBehaviour {
	List<Color> colors;
	
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
	
	public void SetColor(Color color) {
		var controller = GetComponent<PlayerDroidController>();
		var childRenderers = controller.model.GetComponentsInChildren<Renderer>();
			
		foreach (var childRenderer in childRenderers) {
			childRenderer.material.SetColor("_Color", color);
		}
	}
	
}
