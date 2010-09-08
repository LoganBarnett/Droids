using UnityEngine;
using System.Collections;

public class DroidDamager : MonoBehaviour {
	public float criticalChance = 0.25f;
	public float criticalModification = 0.05f;
	public AudioClip damageSound;
	
	void OnTriggerEnter(Collider collider) {
		if (networkView.isMine && collider.GetComponent<DroidLife>() != null) {
			collider.networkView.RPC("TakeDamage", RPCMode.All, criticalChance, criticalModification);
		}
		AudioSource.PlayClipAtPoint(damageSound, transform.position);
	}
}
