using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AneroeInputs;
using System;
using UnityEngine.UI;

public class PromptController : BaseController 
{
	public static EventHandler<TextPromptEventArgs> textPrompted;

	public float promptDuration = 3;

	public static TextPrompt activePrompt;
	public List<KeyValuePair<TextPrompt,TextPromptEventArgs>> queuedPrompts;
	public static InputEventArgs lastInputs;
	float promptTimer;

	[SerializeField] private GameObject promptBox;
	[SerializeField] private Text promptBoxText;
	[SerializeField] private Image promptBoxImage;

	public override void InternalSetup() {
		queuedPrompts = new List<KeyValuePair<TextPrompt,TextPromptEventArgs>> ();
	}

	public override void ExternalSetup()
	{
		InputController.iEvent.inputed += ReceiveInput;
		textPrompted += UpdatePrompt;
	}

	public override void RemoveEventListeners()
	{
		InputController.iEvent.inputed -= ReceiveInput;
		textPrompted -= UpdatePrompt;
	}

	public void Update() {
		if (activePrompt != null) {
			promptTimer += Time.deltaTime;
			if (promptTimer > promptDuration) {
				activePrompt.ContinuePrompt ();
			} else {
				activePrompt.CheckToContinue ();
			}
		}
	}

	void ReceiveInput(object sender, InputEventArgs e) {
		lastInputs = e;
		if (activePrompt != null && e.WasPressed ("return")) {
			activePrompt.ContinuePrompt ();
		}
	}

	void UpdatePrompt(object sender, TextPromptEventArgs textE) {
		if (textE != null) {
			if (activePrompt == null) {
				activePrompt = (TextPrompt)sender;
				if (textE.overrideDuration != -1) {
					promptTimer = promptDuration - textE.overrideDuration;
				}
				promptBoxText.text = textE.text;
				promptBoxImage.sprite = textE.image;
				promptBox.SetActive (true);
			} else {
				queuedPrompts.Add (new KeyValuePair<TextPrompt, TextPromptEventArgs>((TextPrompt)sender,textE));
			}
		} else {
			activePrompt = null;
			promptBox.SetActive (false);
			if (queuedPrompts.Count > 0) {
				KeyValuePair<TextPrompt,TextPromptEventArgs> pair = queuedPrompts [0];
				queuedPrompts.RemoveAt (0);
				UpdatePrompt (pair.Key, pair.Value);
			}
		}
	}

	public void SendPromptEvent(TextPrompt textP, TextPromptEventArgs textE) {
		textPrompted (textP, textE);
	}

}

public class TextPromptEventArgs : InputEventArgs {
	public Sprite image;
	public string text;
	public float overrideDuration;
}