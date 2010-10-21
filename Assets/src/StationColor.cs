using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class StationColor : MonoBehaviour {
	public GameObject modelNode;
	
	Color color;
	
	public Color Color {
		get { return color; }
		set {
			SetModelColor(value);
			color = value;
		}
	}
	
	void Start() {

	}

	void Update() {

	}
	
	void SetModelColor(Color color) {
		foreach(Transform child in modelNode.transform) {
			
			TrySetGameObjectColor(child.gameObject, color);
		}
	}
	
	void TrySetGameObjectColor(GameObject gameObject, Color color) {
		var renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer == null) return;
		renderer.material.color = color;
	}
}
