using UnityEngine;
using System.Collections;

public class PlantBud : Item
{
	public GameObject plantToPlant;

	public override bool Use() {
		Vector3 plantPos = PlayerController.activeCharacter.GetInteractPosition ();

		// Check if plant won't collide with anything else
		BoxCollider2D col = plantToPlant.GetComponent<BoxCollider2D> ();
		RaycastHit2D[] hits = Physics2D.BoxCastAll (col.bounds.center, col.bounds.extents, 0, new Vector2 (0, 0), 0);
		int defaultLayer = LayerMask.NameToLayer ("Default");
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider.gameObject.layer != defaultLayer)
				return false;
		}

		// THIS ASSUMES ONLY ONE SCENE IS IN PLAY AND NEEDS TO CHANGE WHEN WE INCORPORATE SAVING WHEN MULTIPLE SCENES ARE ACTIVE
		GameObject sceneRoot = SceneController.RetrieveSceneRootObjs () [0];

		TerrainController terrainC = sceneRoot.GetComponentInChildren<TerrainController> ();
		InteractivePlant newPlant = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Terrain/" + prefabName)).GetComponent<InteractivePlant>();
		newPlant.transform.SetParent (terrainC.manMadeItemHolder.transform);
		newPlant.transform.position = plantPos;
		newPlant.Plant ();
		return true;
	}
}

