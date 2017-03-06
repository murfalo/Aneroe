using UnityEngine;
using System.Collections;

public class SceneLoadDefaults : MonoBehaviour
{
	// Current scene load defaults to be used
	public static string[] currentObjName;
	public static string[] currentObjGroup;
	public static Vector3[] currentObjStartPos;

	// Prefab name, custom grouping (for easier locating), and start position
	// Unique to each scene
	public string[] objName;
	public string[] objGroup;
	public Vector3[] objStartPos;

	void Awake() {
		// Upon loading, it is the current scene to apply its defaults, so it replaces static field values
		currentObjName = objName;
		currentObjGroup = objGroup;
		currentObjStartPos = objStartPos;
	}
}
