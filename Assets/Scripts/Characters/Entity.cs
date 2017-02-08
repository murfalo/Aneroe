using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Entity : MonoBehaviour {

	// Components
	protected Animator anim;
	protected Weapon activeWeapon;
	protected EntityController controller;
	//Collider2D hurtbox;

	// Combat stats
	public StatInfo stats;
	// Inspector editable stats
	public float MAX_HEALTH = 10;
	public float speed = 1f;
	public float attack = 1f;
	public float defense = 1f;

	// Collision stats
	public float characterRadius;

	// State (used for animation also) 
	public enum CharacterState { 
		Dead,
		Immobile, 
		Still, 
		Walking,
		Attacking, 
		Blocking,
		Digging,
	};

	// Internal states
	public float ATTACK_SPEED_FACTOR = .5f;
	public float BLOCK_SPEED_FACTOR = 0f;
	public float RUN_SPEED_FACTOR = 1.5f;
	public float NORMAL_SPEED_FACTOR = 1f;
	protected float speedFactor;
	protected bool slideDirs;
	protected int primaryDir;
	protected int secondaryDir;

	protected float stunTimer;
	// Character alternates step animation; this toggles which one
	protected bool oddStep;

	// Convention used for directions, but not using the enum for simplicity
	public static Vector2[] directionVectors = new Vector2[4] {
		new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0)
	};
	public static float secondaryDirFactor = .5f;

	public enum Dir {
		Up = 1,
		Right,
		Down,
		Left
	};

	public virtual void Setup () {
		anim = GetComponent<Animator> ();
		activeWeapon = GetComponentInChildren<Weapon> ();

		anim.SetInteger ("state", (int)CharacterState.Still);
		anim.SetInteger ("dir", (int)Dir.Down);
		oddStep = false;
		slideDirs = false;
		speedFactor = NORMAL_SPEED_FACTOR;
		stats = new StatInfo (new Dictionary<string, float>() {
			{"health",MAX_HEALTH},
			{"attack",attack},
			{"defense",defense},
			{"speed",speed}
		});
	}

    public void DoFixedUpdate() {
		if (stunTimer > 0) {
			stunTimer -= Time.fixedDeltaTime;
			if (stunTimer <= 0) {
				GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
				stunTimer = 0;
			}
		}
		//print (gameObject.name + "  " + (CharacterState)anim.GetInteger ("state"));
		//print (gameObject.name + "  " + (CharacterState)anim.GetInteger ("state") + "  " + anim.GetInteger ("dir") + "  " + speedFactor);
		switch (anim.GetInteger ("state")) {
		case (int)CharacterState.Walking:
			ExecuteWalk ();
			break;
		case (int)CharacterState.Attacking:
		case (int)CharacterState.Blocking:
			if (primaryDir > 0)
				ExecuteWalk ();
			activeWeapon.ProcessDamageQueue ();
			break;
		default:
			//print (gameObject.name + "  " + new Vector2 (0, 0));
			break;
		}
	}

	void ExecuteWalk() {
		int dir = primaryDir - 1;
		if (dir < 0)
			dir = anim.GetInteger ("dir") - 1;
		Vector3 dirVector = directionVectors [dir];
		if (secondaryDir > 0) {
			dirVector = Vector3.Normalize(dirVector + secondaryDirFactor * (Vector3)directionVectors [secondaryDir - 1]);
		}
		Vector3 move = speedFactor * speed * Time.fixedDeltaTime * dirVector;
		RaycastHit2D r = Physics2D.Raycast (transform.position, (Vector2)move, move.magnitude + characterRadius, 1 << LayerMask.NameToLayer ("Wall"));

		if (r.collider == null) {
			transform.Translate (move);
		}
	}

	public void Quicken(bool active) {
		if (!CanActOutOfMovement())
			return;
		if (active) {
			speedFactor = RUN_SPEED_FACTOR;
		} else if (Mathf.Abs(speedFactor - RUN_SPEED_FACTOR) < .001) {
			speedFactor = NORMAL_SPEED_FACTOR;
		}
	}
		
	public void EndWeaponUseAnim() {
		anim.SetInteger ("state", (int)CharacterState.Still);
		speedFactor = NORMAL_SPEED_FACTOR;
	}

	public void EndMovementAnim() {
		if (!CanActOutOfMovement())
			return;
		anim.SetInteger ("state", (int)CharacterState.Still);
	}

	protected bool CanActOutOfMovement() {
		return GetState() <= CharacterState.Walking && GetState() != CharacterState.Immobile;
	}

	public bool InAttack() {
		return GetState() == CharacterState.Attacking || GetState() == CharacterState.Blocking;
	}

	public bool CanSwitchFrom() {
		return anim.GetInteger ("state") <= (int)CharacterState.Still;
	}
		
	// Sets animation state for walking
	public void TryWalk() {
		if ((CharacterState)anim.GetInteger ("state") != CharacterState.Still)
			return;
		speedFactor = NORMAL_SPEED_FACTOR;
		// Alternate the step this walk cycle executes with
		oddStep = !oddStep;
		anim.SetTime (0);
		anim.SetInteger ("state", (int)CharacterState.Walking);
		anim.SetBool ("oddStep", oddStep);
	}

	// Sets animation state for attacking
	public void TryAttacking() {
		if (!CanActOutOfMovement())
			return;
		anim.SetTime (0);
		anim.SetInteger ("state", (int)CharacterState.Attacking);
		activeWeapon.StartAttack (GetDirection());
		speedFactor = ATTACK_SPEED_FACTOR;
	}

	public void TryBlocking() {
		if (!CanActOutOfMovement())
			return;
		anim.SetTime (0);
		anim.SetInteger ("state", (int)CharacterState.Blocking);
		activeWeapon.StartBlock (GetDirection());
		speedFactor = BLOCK_SPEED_FACTOR;
	}

	public CharacterState GetState() {
		return (CharacterState)anim.GetInteger ("state");
	}

	public void SetDirections(bool[] directions) {
		if (!CanActOutOfMovement ()) {
			if (InAttack ()) {
				slideDirs = true;
			} else
				return;
		} else {
			slideDirs = false;
		}

		// If primary direction isn't still held, reset primaryDir;
		if (primaryDir > 0 && !directions [primaryDir - 1]) {
			primaryDir = 0;
			if (secondaryDir > 0 && directions [secondaryDir - 1])
				primaryDir = secondaryDir;
			secondaryDir = 0;
		}
		if (secondaryDir > 0 && !directions [secondaryDir - 1]) {
			secondaryDir = 0;
		}
		for (int i = 0; i < directions.Length; i++) {
			if (directions [i]) {
				if (primaryDir == 0)
					primaryDir = i + 1;
				else if (secondaryDir == 0 && (primaryDir + i + 1) % 2 != 0)
					secondaryDir = i + 1;
			}
		}
		if (primaryDir > 0 && !slideDirs)
			anim.SetInteger ("dir", primaryDir);
	}

	public int GetDirection() {
		return anim.GetInteger ("dir");
	}

	// Damages character, returns true if character is at 0 health
	public bool Damage(float amount) {
		stats.ChangeStat("health",-amount);
		if (stats.GetStat("health") <= 0) {
			Kill ();
		}
		stunTimer = .2f;
		GetComponent<SpriteRenderer> ().color = new Color (1, 0, 0, 1);
		return false;
	}

	public void Kill() {
		anim.SetInteger ("state", (int)CharacterState.Dead);
		controller.RespondToEntityAction (this, "die");
		foreach (Collider2D col in GetComponents<Collider2D>()) {
			col.enabled = false;
		}
	}

}
