using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {
	public float speed = 10.0f;
	public float damage = 5.0f;
	public float lifeInSeconds = 10.0f;
	public float syncPositionDistanceThreshold = 1.0f;
	
	CharacterController controller;
	Vector3 movementVector;
	bool isDead = false;
	
	void Start() {
		movementVector = transform.forward * speed;
		StartCoroutine(Expire());
	}
	
	IEnumerator Expire() {
		yield return new WaitForSeconds(lifeInSeconds);
		if (gameObject != null && !isDead && networkView.isMine) {
			Network.Destroy(gameObject);
			Network.RemoveRPCs(networkView.viewID);
		}
	}
	
	void Update() {
		transform.Translate(movementVector * Time.deltaTime, Space.World);
	}
	
	void OnTriggerEnter(Collider collider) {
		if (networkView.isMine) {
//			Debug.Log("Destroying from collision: " + networkView.viewID);
			Network.Destroy(gameObject);
			Network.RemoveRPCs(networkView.viewID);
			isDead = true;
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			var position = transform.position;
			var rotation = transform.rotation;
			
			stream.Serialize(ref movementVector);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
		} else {
			var position = Vector3.zero;
			var rotation = Quaternion.identity;
			
			stream.Serialize(ref movementVector);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
			
			if (Vector3.Distance(transform.position, position) > syncPositionDistanceThreshold) {
				transform.position = position;
			}
			transform.rotation = rotation;
		}
	}
}
