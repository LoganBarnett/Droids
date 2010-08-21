using UnityEngine;
using System.Collections;

public class AxisConstraint : MonoBehaviour {
//	public float x;
//	public float y;
	public float z;
	
	void Update() {
		transform.position = new Vector3(transform.position.x, transform.position.y, z);	
	}
}