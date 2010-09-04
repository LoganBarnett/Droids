using UnityEngine;
using System.Collections;

public class DroidLife : MonoBehaviour {
	public float criticalBaseChance = 0.0f;
	public float deathForce = 100.0f;
	public GameObject droidModel;

	float currentCriticalChance;
	
	void Start() {
		currentCriticalChance = criticalBaseChance;
	}
	
	void OnTriggerEnter(Collider collider) {
		var damager = collider.GetComponent<DroidDamager>();
		if (damager == null) return;
		
		AudioSource.PlayClipAtPoint(damager.damageSound, transform.position);
		
		var criticalChance = damager.criticalChance + currentCriticalChance;
		if (Random.value <= criticalChance) {
			Die();
		} else {
			currentCriticalChance += damager.criticalModification;
		}
		Debug.Log("Chance: " + currentCriticalChance);
	}
	
	void Die() {
		Debug.Log("*****Dead!*****");
		foreach (Transform child in droidModel.transform) {
//			if (droidModel.transform == child) continue;
//			if (droidModel.transform.parent == child) continue;
			
			Debug.Log(child.gameObject.name);
			// Can't do this here, because it mutates the enumerable!
			//child.parent = null;
			var meshFilter = child.gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null) { Debug.Log("Skipping " + child.gameObject.name); continue;}
			var rigidbody = child.gameObject.AddComponent<Rigidbody>();
			var collider = child.gameObject.AddComponent<MeshCollider>();
			collider.sharedMesh = meshFilter.sharedMesh;
			
			rigidbody.useGravity = true;
			rigidbody.mass = 1.0f;
			
			var center = droidModel.transform.position;
			var outwardsDirection = child.transform.position - center;
			rigidbody.AddForce(outwardsDirection * deathForce);
		}
		droidModel.transform.DetachChildren();
		Destroy(gameObject.GetComponent<PlayerDroidController>());
		Destroy(gameObject.GetComponent<CharacterController>());
		
		
	}
}
