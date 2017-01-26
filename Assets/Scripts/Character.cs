using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	// Components
	Animator anim;
	Backpack pack;
	WeaponController weaponC;
	Collider2D hurtbox;

	// Combat stats
	public float MAX_HEALTH = 10;
	public float speed = 1f;
	public float attack = 1f;
	public float defense = 1f;
	float health;

	// State (used for animation also)
	public enum CharacterState { 
		Dead = 0, 
		Immobile, 
		Still, 
		Walking,  
		Attacking, 
		Blocking,
		Digging,
	};
	// Character alternates step animation; this toggles which one
	public bool oddStep;

	public float stunTimer;

	// Convention used for directions, but not using the enum for simplicity
	public static Vector2[] directionVectors = new Vector2[4] {
		new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0)
	};
	/*public enum CharacterDir {
		Up = 1,
		Right,
		Down,
		Left
	};*/

	void Awake () {
		anim = GetComponent<Animator> ();
		anim.SetInteger ("state", (int)CharacterState.Still);
		anim.SetInteger ("dir", 1);
		oddStep = false;
		health = MAX_HEALTH;
		pack = GetComponent<Backpack> ();
		weaponC = GetComponentInChildren<WeaponController> ();
		hurtbox = GetComponent<Collider2D> ();
	}

	void Start() {
		weaponC.SetActiveWeapon (GetComponentInChildren<Weapon> ());
	}

	void FixedUpdate() {
		//print (gameObject.name + "  " + anim.GetInteger ("state") + "  " + anim.GetInteger ("dir"));
		int dir = anim.GetInteger ("dir") - 1;
		switch (anim.GetInteger ("state")) {
		case (int)CharacterState.Walking:
			//print (gameObject.name + "  " + (speed * Time.deltaTime * directionVectors[dir].x) + "  " + (speed * Time.deltaTime * directionVectors[dir].y));
			transform.Translate (speed * Time.deltaTime * directionVectors[dir]);
			break;
		default:
			//print (gameObject.name + "  " + new Vector2 (0, 0));
			break;
		}
	}

	// Sets animation state to still
	public void SetStill() {
		anim.SetInteger ("state", (int)CharacterState.Still);
	}

	// Whether the character can begin an action
	public bool CanAct() {
		return anim.GetInteger ("state") == (int)CharacterState.Still;
	}

	// Sets animation state for walking
	public void StartWalk() {
		// Alternate the step this walk cycle executes with
		oddStep = !oddStep;
		anim.SetInteger ("state", (int)CharacterState.Walking);
		anim.SetBool ("oddStep", oddStep);
	}

	// Sets animation state for attacking
	public void SetAttacking() {
		if (((CharacterState)anim.GetInteger ("state")) != CharacterState.Still)
			return;
		anim.SetInteger ("state", (int)CharacterState.Attacking);
		weaponC.StartAttack ();
	}

	public void SetBlocking() {
		if (((CharacterState)anim.GetInteger ("state")) != CharacterState.Still)
			return;
		anim.SetInteger ("state", (int)CharacterState.Blocking);
		weaponC.StartBlock ();
	}

	public CharacterState GetState() {
		return (CharacterState)anim.GetInteger ("state");
	}

	public void SetDirection(int direction) {
		anim.SetInteger ("dir", direction);
	}

	public int GetDirection() {
		return anim.GetInteger ("dir");
	}

	// Damages character, returns true if character is at 0 health
	public bool Damage(float amount) {
		health -= amount;
		return health <= 0;
	}
}
