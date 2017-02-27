using UnityEngine;
using System.Collections;

public class TallObject : MonoBehaviour
{
	SpriteRenderer sRend;

	void Awake() {
		sRend = GetComponentInParent<SpriteRenderer> ();
	}

	public virtual void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.layer == LayerMask.NameToLayer ("TallObject")) {
			SpriteRenderController.AddToOverlappings (sRend);
			// Temporary fix to prevent multiple instances of sprite renderer added to controller
			if (col.GetComponentInParent<Entity> () == null)
				SpriteRenderController.AddToOverlappings (col.GetComponentInParent<SpriteRenderer> ());
		}
	}
}

