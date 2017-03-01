using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPrompt : TextPrompt {

	protected override void OnTriggerEnter2D(Collider2D other) {
		// Intentionally override to prevent a trigger activating this prompt.
		// It is a chain prompt, activated by the previous prompt linked to it.
	}
}
