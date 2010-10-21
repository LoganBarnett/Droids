using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Entropy : MonoBehaviour {
	// initial values - expect to change over time
	public int minimumEntropyAmount;
	public int maximumEntropyAmount;
	
	List<GameObject> stations = new List<GameObject>();
	
	
	
	void Start() {
		stations = GameObject.FindGameObjectsWithTag("station").ToList();
	}

	void Update() {

	}
}
