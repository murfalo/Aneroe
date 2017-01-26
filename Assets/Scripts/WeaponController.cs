using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	Character character;
	Animator anim;
	Weapon activeWeapon;

	void Awake () {
		character = GetComponentInParent<Character> ();
		anim = GetComponent <Animator> ();
	}

	void Update () {
	
	}

	public void StartAttack() {
		anim.SetBool ("attack", true);
		anim.SetInteger ("dir", character.GetDirection ());
	}

	public void StartBlock() {
		anim.SetBool ("block", true);
		anim.SetInteger ("dir", character.GetDirection ());
	}

	public void EndSwing() {
		anim.SetBool ("attack", false);
		anim.SetBool ("block", false);
	}

	public void SetActiveWeapon(Weapon w) {
		activeWeapon = w;
		anim.runtimeAnimatorController = w.weaponAnim;
	}

	// Called by opponent when colliding with them
	// Return damage done to opponent
	public float DamageToInflict() {
		return 1;
	}

	public bool IsAttacking() {
		return anim.GetBool ("swipe");
	}

	public Character GetCharacter() {
		return character;
	}

	void OnTriggerEnter2D(Collider2D other) {
		// If collided with opponent's hitbox
		if (other.gameObject.tag.Equals ("Hitboxes")) {
			WeaponController otherWC = other.gameObject.GetComponent<WeaponController> ();
			if (IsAttacking () && !otherWC.IsAttacking ()) {
				character.Damage (otherWC.DamageToInflict ());
			} else if (!IsAttacking () && otherWC.IsAttacking ()) {
				otherWC.GetCharacter ().Damage (DamageToInflict ());
			}
		}
	}
}
