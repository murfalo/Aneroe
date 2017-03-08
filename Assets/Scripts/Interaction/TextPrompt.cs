using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPrompt : MonoBehaviour {

	public string[] imagePaths;
	public string[] stringKeys;
	public float overridePromptDuration = -1;
	public bool noTimer;

	protected Sprite[] images;
	protected string[] stringPrompts;

	protected int promptIndex;

	protected virtual void Awake() {
		stringPrompts = new string[stringKeys.Length];
		for (int i = 0; i < stringKeys.Length; i++) {
			stringPrompts[i] = PromptStrings.prompts [stringKeys[i]];
		}
		if (imagePaths.Length > 0) {
			images = new Sprite[imagePaths.Length];
			for (int i = 0; i < imagePaths.Length; i++) {
				if (imagePaths [i] != "")
					images [i] = Resources.Load<Sprite> ("Sprites/TextPrompts/" + imagePaths [i]);
				else
					images [i] = null;
			}
		} else
			images = new Sprite[stringKeys.Length];
		
		promptIndex = -1;
	}

	public virtual void BeginPrompt() {
		if (promptIndex != -1)
			return;
		promptIndex++;
		SendPromptEvent (false);
	}

	public virtual void ContinuePrompt() {
		if (promptIndex < stringPrompts.Length - 1) {
			promptIndex++;
			SendPromptEvent (true);
		} else {
			PromptController.textPrompted (this, null);
		}
	}

	public virtual void CheckToContinue() {}

	public void SendPromptEvent(bool overrideEvent) {
		//	PromptController.activePrompt = null;
		TextPromptEventArgs textE = new TextPromptEventArgs ();
		textE.text = stringPrompts [promptIndex];
		textE.image = images [promptIndex];
		textE.overrideDuration = overridePromptDuration;
		textE.textSpeaker = "???";
		textE.chainPrompt = overrideEvent;
		PromptController.textPrompted (this, textE);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent<Entity> () == PlayerController.activeCharacter) {
			BeginPrompt ();
		}
	}

	public Hashtable Save()
	{
		return new Hashtable () {
			{ "promptIndex",promptIndex },
		};
	}

	public void Load(Hashtable info)
	{
		promptIndex = (int)info ["promptIndex"];
	}
}
