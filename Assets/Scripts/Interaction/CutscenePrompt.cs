using UnityEngine;
using System.Collections;
using AneroeInputs;

public class CutscenePrompt : MonoBehaviour
{
	public string cutsceneName;
	bool started = false, finished = false;

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (!started) {
			if (other.GetComponent<Entity> () == PlayerController.activeCharacter) {
				started = true;
				CutsceneController.BeginCutscene (this, cutsceneName);
			}
		}
	}

	// Triggered by cutscene controller only after the scene is finished
	public void Finish() {
		finished = true;
	}

	public Hashtable Save()
	{
		// Only prevent the scene from replaying if it finishes.
		return new Hashtable () {
			{ "finished",finished },
		};
	}

	public void Load(Hashtable info)
	{
		started = (bool)info ["finished"];
		finished = started;
		print (cutsceneName + "  " + started);
	}
}

