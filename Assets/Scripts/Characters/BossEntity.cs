using UnityEngine;
using System.Collections;

public class BossEntity : AIEntity
{
	public string cutsceneName;
	int attempts = 0;

	// Damages character, returns true if character is at 0 health
	public override void Damage(float amount, int dirFrom, float stunTime, float stunVel)
	{
		if (amount < 5) {
			if (!PlayerController.activeCharacter.InAttack ())
				return;
			attempts++;
			if (attempts >= 3) {
				foreach (GameObject obj in SceneController.RetrieveSceneRootObjs()) {
					foreach (CutscenePrompt cP in obj.GetComponentsInChildren<CutscenePrompt>()) {
						if (cutsceneName.Equals (cP.name)) {
							cP.ManualActivate ();
							return;
						}
					}
				}
			}
		} else {
			base.Damage (amount, dirFrom, stunTime, stunVel);
		}
	}

}

