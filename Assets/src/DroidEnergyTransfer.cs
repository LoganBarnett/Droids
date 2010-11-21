
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class DroidEnergyTransfer : MonoBehaviour {
	public GameObject lightningBolt;
	public Material red;
	public Material green;
	public Material blue;
	public float transferDistance = 10f;
	
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
				lightningBolt.GetComponent<LightningBolt>().target = station.transform;
				lightningBolt.active = true;
				var particleRenderer = lightningBolt.GetComponent<ParticleRenderer>();
				if (redInput) particleRenderer.material = red;
				else if (greenInput) particleRenderer.material = green;
				else if (blueInput) particleRenderer.material = blue;
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