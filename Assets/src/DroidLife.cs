using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class DroidLife : MonoBehaviour {
	public float criticalBaseChance = 0.0f;
	public float deathForce = 100.0f;
	public GameObject droidModel;
	public AudioClip droidDeathSound;
	public float partFadeawayInSeconds = 5.0f;
	
	float currentCriticalChance;
	
	public NetworkedPlayerSpawner Spawner { get; set; }
	
	void Start() {
		currentCriticalChance = criticalBaseChance;
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo networkInfo) {
		Spawner = GameObject.Find("Networking").GetComponent<NetworkedPlayerSpawner>();
		Spawner.RegisterDroid(gameObject);
	}
	
	void OnTriggerEnter(Collider collider) {
		var damager = collider.GetComponent<DroidDamager>();
		if (damager == null) return;
		
		AudioSource.PlayClipAtPoint(damager.damageSound, transform.position);
		
		if (networkView.isMine) {
			var criticalChance = damager.criticalChance + currentCriticalChance;
			if (Random.value <= criticalChance) {
				networkView.RPC("Die", RPCMode.Others);
				Die();
			} else {
				currentCriticalChance += damager.criticalModification;
			}
		}
		Debug.Log("Chance: " + currentCriticalChance);
	}
	
	[RPC]
	void Die() {
		Debug.Log("*****Dead!*****");
		ScatterParts();
		droidModel.transform.DetachChildren();
		Destroy(gameObject.GetComponent<PlayerDroidController>());
		Destroy(gameObject.GetComponent<CharacterController>());
		AudioSource.PlayClipAtPoint(droidDeathSound, transform.position);
		
		if( Spawner != null) Spawner.DroidDied(gameObject);
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		stream.Serialize(ref currentCriticalChance);
	}
	
	private void ScatterParts() {
		var parts = new List<GameObject>();
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
			
			// Ignore collision doesn't work with character controllers ):
			// Instead use translation hack 
//			foreach( var controller in Spawner.Droids.Select( d => d.GetComponent<CharacterController>() )) {				
//				Physics.IgnoreCollision(controller, collider, true);
//			}
			child.position = child.position + new Vector3(0.0f, 0.0f, 10.0f);
			parts.Add(child.gameObject);
		}
		StartCoroutine(FadeParts(parts));
	}
	
	IEnumerator FadeParts(List<GameObject> parts) {
		yield return new WaitForSeconds(partFadeawayInSeconds);
		// TODO: Make alpha fadeout on part before destroying
		foreach (var part in parts) { Destroy(part); }
		
		yield return new WaitForSeconds(1.0f);
		
		if (networkView.isMine) {
			Network.Destroy(gameObject);
		}
	}
}
