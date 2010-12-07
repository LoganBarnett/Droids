
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class DroidEnergyTransfer : MonoBehaviour {
	public GameObject lightningBolt;
	public Material redMaterial;
	public Material greenMaterial;
	public Material blueMaterial;
	public float transferDistance = 10f;
	public float energyTransferRate = 0f;
	
	float redEnergy = 0f;
	float greenEnergy = 0f;
	float blueEnergy = 0f;
	
	void Start() {
		lightningBolt.active = false;
	}

	void Update() {
		var redInput = Input.GetButton("Red");
		var greenInput = Input.GetButton("Green");
		var blueInput = Input.GetButton("Blue");
		
		var useNetworkInput = false;
		
		if (!useNetworkInput && redInput || greenInput || blueInput) {
			GameObject station;
			if (TryShoot(out station)) {
				var transferAmount = energyTransferRate * Time.deltaTime;
				
				lightningBolt.GetComponent<LightningBolt>().target = station.transform;
				lightningBolt.active = true;
				var particleRenderer = lightningBolt.GetComponent<ParticleRenderer>();
				if (redInput) particleRenderer.material = redMaterial;
				else if (greenInput) particleRenderer.material = greenMaterial;
				else if (blueInput) particleRenderer.material = blueMaterial;
				
				
			} else {
				lightningBolt.active = false;
			}
		}
		else
		{
			lightningBolt.active = false;
		}
	}
	
	bool TryShoot(out GameObject closestStation) {
		closestStation = null;
		var stations = GameObject.FindGameObjectsWithTag("Station");
		if (stations.Count() == 0) return false;
		
		var stationDistances = stations.Select(s => {
			var distance = Vector3.Distance(s.transform.position, transform.position);
			return new { station = s, distance = distance };
		});
		var closestStationDistance = stationDistances.OrderBy(sd => sd.distance).ToList()[0];
		closestStation = closestStationDistance.station;

		return closestStationDistance.distance < transferDistance;
	}
	
	
}