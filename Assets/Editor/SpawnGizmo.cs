using UnityEditor;
using UnityEngine;
using System.Collections;


public class SpawnGizmo : MonoBehaviour {
	[DrawGizmo(GizmoType.NotSelected | GizmoType.Pickable)]
	static void RenderLightGizmo(GameObject gameObject, GizmoType gizmoType) {
		if (gameObject.CompareTag("SpawnPoint")) {
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(gameObject.transform.position, 0.5f);
		}
	}
}
