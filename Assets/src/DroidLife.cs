using UnityEngine;
using System.Collections;

public class DroidLife : MonoBehaviour {
	public float criticalBaseChance = 0.0f;
	public float deathForce = 100.0f;
	public GameObject droidModel;
	
	float currentCriticalChance;
	
	public NetworkedPlayerSpawner Spawner { get; set; }
	
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
		ScatterParts();
		droidModel.transform.DetachChildren();
		Destroy(gameObject.GetComponent<PlayerDroidController>());
		Destroy(gameObject.GetComponent<CharacterController>());
		
		if( Spawner != null) Spawner.DroidDied(gameObject);
	}
	
	private void ScatterParts() {
		foreach (Transform child in droidModel.transform) {
			// Can't do this here, because it mutates the enumerable!
			//child.parent = null;
			var meshFilter = child.gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null) continue;
			var rigidbody = child.gameObject.AddComponent<Rigidbody>();
			var collider = child.gameObject.AddComponent<MeshCollider>();
			collider.sharedMesh = meshFilter.sharedMesh;
			
			rigidbody.useGravity = true;
			rigidbody.mass = 1.0f;
			
			var angle = Random.Range(-Mathf.PI, Mathf.PI);
			var x = Mathf.Sin(angle);
			var y = Mathf.Cos(angle);
			var direction = new Vector3(x, y, 0.0f);
			rigidbody.AddForce(deathForce * direction);
		}
	}
}
