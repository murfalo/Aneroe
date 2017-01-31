using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	Entity entity;
	Animator anim;
	Weapon activeWeapon;

	void Awake () {
		entity = GetComponentInParent<Entity> ();
		anim = GetComponent <Animator> ();
	}

	void Update () {
	
	}

	public void StartAttack() {
		anim.SetBool ("attack", true);
		anim.SetInteger ("dir", entity.GetDirection ());
	}

	public void StartBlock() {
		anim.SetBool ("block", true);
		anim.SetInteger ("dir", entity.GetDirection ());
	}

	public void EndSwing() {
		anim.SetBool ("attack", false);
		anim.SetBool ("block", false);
	}

	public void SetActiveWeapon(Weapon w) {
		activeWeapon = w;
		anim.runtimeAnimatorController = activeWeapon.weaponAnim;
	}

	// Called by opponent when colliding with them
	// Return damage done to opponent
	public float DamageToInflict() {
		return 1;
	}

	public bool IsAttacking() {
		return anim.GetBool ("swipe");
	}

	public Entity GetEntity() {
		return entity;
	}

	void OnTriggerEnter2D(Collider2D other) {
		// If collided with opponent's hitbox
		if (other.gameObject.tag.Equals ("Hitboxes")) {
			WeaponController otherWC = other.gameObject.GetComponent<WeaponController> ();
			if (IsAttacking () && !otherWC.IsAttacking ()) {
				entity.Damage (otherWC.DamageToInflict ());
			} else if (!IsAttacking () && otherWC.IsAttacking ()) {
				otherWC.GetEntity ().Damage (DamageToInflict ());
			}
		}
	}
}
