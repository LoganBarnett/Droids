using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;


public class StationSpawnGizmo : MonoBehaviour {
	[DrawGizmo(GizmoType.NotSelected | GizmoType.Pickable)]
	static void RenderLightGizmo(GameObject gameObject, GizmoType gizmoType) {
		if (gameObject.CompareTag("StationSpawn") && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(gameObject))) {
			Gizmos.color = Color.yellow;
			var prefab = (GameObject)GameObject.FindObjectsOfTypeIncludingAssets(typeof(GameObject)).First(g => ((GameObject)g).name == "Work Station");
			var modelTransform = prefab.transform.FindChild("Cube");
			Gizmos.DrawCube(gameObject.transform.position, modelTransform.localScale);
		}
	}
}

