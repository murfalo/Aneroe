using UnityEngine;
using System.Collections;

public class TerrainController : BaseController
{

	public static bool BuryItem(Item i,Vector3 pos) {
		RaycastHit2D[] hits = Physics2D.CircleCastAll (pos, 0.1f, new Vector3 (0, 1, 0), 0.1f,1 << LayerMask.NameToLayer("InteractiveTile"));
		for (int j = 0; j < hits.Length; j++) {
			ItemMound buryTile = hits [j].collider.gameObject.GetComponent<ItemMound> ();
			if (buryTile != null) {
				if (buryTile.UseItem(i)) {
					return true;
				}
			}
		}
		return false;
	}

	public static Item RetrieveItem(Vector3 pos) {
		RaycastHit2D[] hits = Physics2D.CircleCastAll (pos, 0.1f, new Vector3 (0, 1, 0), 0.1f,1 << LayerMask.NameToLayer("InteractiveTile"));
		for (int j = 0; j < hits.Length; j++) {
			ItemMound buryTile = hits [j].collider.gameObject.GetComponent<ItemMound> ();
			if (buryTile != null) {
				Item i = buryTile.RetrieveItem ();
				if (i) {
					return i;
				}
			}
		}
		return null;	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

