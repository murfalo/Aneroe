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

	protected float ATTACK_SPEED_FACTOR = .5f;
	protected float BLOCK_SPEED_FACTOR = 0f;
	protected float RUN_SPEED_FACTOR = 2f;
	protected float NORMAL_SPEED_FACTOR = 1f;

	protected float speedFactor;
	protected int primaryDir;
	protected int secondaryDir;
	protected float stunTimer;
	// Character alternates step animation; this toggles which one
	protected bool oddStep;

	// Convention used for directions, but not using the enum for simplicity
	public static Vector2[] directionVectors = new Vector2[] {
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
		activeWeapon.Setup ();

		stunTimer = 0;
		anim.SetInteger ("state", (int)CharacterState.Still);
		anim.SetInteger ("dir", (int)Dir.Down);
		oddStep = false;
		speedFactor = NORMAL_SPEED_FACTOR;
		stats = new StatInfo (new Dictionary<string, float>() {
			{"health",MAX_HEALTH},
			{"attack",attack},
			{"defense",defense},
			{"speed",speed}
		});
	}

	public virtual void DoFixedUpdate() {
		//print (gameObject.name + "  " + (CharacterState)anim.GetInteger ("state"));
		//print (gameObject.name + "  " + (CharacterState)anim.GetInteger ("state") + "  " + anim.GetInteger ("dir") + "  " + speedFactor);

		// Timer updates
		if (stunTimer > 0 && DecrementTimer (stunTimer, out stunTimer)) {
			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
			stunTimer = 0;
		}

		// State-based updates
		switch (anim.GetInteger ("state")) {
		case (int)CharacterState.Walking:
			ExecuteWalk ();
			break;
		case (int)CharacterState.Attacking:
		case (int)CharacterState.Blocking:
			if (primaryDir > 0)
				ExecuteWalk ();
			break;
		default:
			//print (gameObject.name + "  " + new Vector2 (0, 0));
			break;
		}
	}

	protected bool DecrementTimer(float value, out float timer)
	{
		timer = value -= Time.fixedDeltaTime;
		return timer <= 0;
	}

	void ExecuteWalk() {
		int dir = primaryDir - 1;
		if (dir < 0)
			dir = GetDirection() - 1;
		Vector3 dirVector = directionVectors [dir];
		if (secondaryDir > 0) {
			dirVector = Vector3.Normalize(dirVector + secondaryDirFactor * (Vector3)directionVectors [secondaryDir - 1]);
		}
		Vector3 move = speedFactor * speed * Time.fixedDeltaTime * dirVector;

		// BAD COLLISION CODE. WE NEED TO RESTRUCTURE COLLISIONS ENTIRELY. MORE TO COME
		/*RaycastHit2D[] hits = Physics2D.RaycastAll (transform.position, (Vector2)move, move.magnitude + characterRadius, 1 << LayerMask.NameToLayer ("Wall"));
		bool noCollisions = true;
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null && !hit.collider.GetComponentInParent<Entity> ().Equals (this)) {
				noCollisions = false;
				break;
			}
		}
		if (noCollisions)
			transform.Translate (move);*/
		
		transform.Translate (move);
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
		
	public virtual void EndWeaponUseAnim() {
		anim.SetInteger ("state", (int)CharacterState.Still);
		speedFactor = NORMAL_SPEED_FACTOR;
	}

	public virtual void EndMovementAnim() {
		if (!CanActOutOfMovement())
			return;
		anim.SetInteger ("state", (int)CharacterState.Still);
	}

	protected bool CanActOutOfMovement() {
		if (GetState () > CharacterState.Walking || GetState () == CharacterState.Immobile) {
			return false;
		}
		if (stunTimer > 0)
			return false;
		return true;
	}

	public bool InAttack() {
		return GetState() == CharacterState.Attacking || GetState() == CharacterState.Blocking;
	}

	public bool CanSwitchFrom() {
		return GetState() <= CharacterState.Still;
	}
		
	// Sets animation state for walking
	public virtual void TryWalk() {
		if (GetState() != CharacterState.Still)
			return;			
		// Alternate the step this walk cycle executes with
		oddStep = !oddStep;
		anim.SetTime (0);
		anim.SetInteger ("state", (int)CharacterState.Walking);
		anim.SetBool ("oddStep", oddStep);
		speedFactor = NORMAL_SPEED_FACTOR;
	}

	// Sets animation state for attacking
	public virtual void TryAttacking() {
		if (!CanActOutOfMovement ())
			return;			
		anim.SetTime (0);
		anim.SetInteger ("state", (int)CharacterState.Attacking);
		activeWeapon.StartAttack (GetDirection());
		speedFactor = ATTACK_SPEED_FACTOR;
	}

	public virtual void TryBlocking() {
		if (!CanActOutOfMovement ())
			return;			
		anim.SetTime (0);
		anim.SetInteger ("state", (int)CharacterState.Blocking);
		activeWeapon.StartBlock (GetDirection());
		speedFactor = BLOCK_SPEED_FACTOR;
	}

	public CharacterState GetState() {
		return (CharacterState)anim.GetInteger ("state");
	}


	public int GetDirection() {
		return anim.GetInteger ("dir");
	}
		
	// Damages character, returns true if character is at 0 health
	public void Damage(float amount) {
		stats.ChangeStat("health",-amount);
		if (stats.GetStat ("health") <= 0) {
			Kill ();
		} else {
			stunTimer = .2f;
			GetComponent<SpriteRenderer> ().color = new Color (1, 0, 0, 1);
		}
	}

	public void Kill() {
		anim.SetInteger ("state", (int)CharacterState.Dead);
		controller.RespondToEntityAction (this, "die");
		foreach (Collider2D col in GetComponents<Collider2D>()) {
			col.enabled = false;
		}
	}

}