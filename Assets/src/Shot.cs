using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {
	public float speed = 10.0f;
	public float damage = 5.0f;
	
	CharacterController controller;
	Vector3 movementVector;
	
	void Start() {
		movementVector = transform.forward * speed;
		
	}
	
	void Update() {
		transform.Translate(movementVector * Time.deltaTime, Space.World);
	}
	
	void OnTriggerEntered(Collider collider) {
		Destroy(gameObject);
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
			
			transform.position = position;
			transform.rotation = rotation;
		}
	}
}
