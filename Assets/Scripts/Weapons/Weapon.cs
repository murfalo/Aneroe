﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Item {

	public static float MAX_DURABILITY;
	Animator anim;

	// Properties
	public float speed = 1f;
	public float attack = 1f;
	public float defense = 1f;
	//float durability;

	// targets hit by current attack
	// a target can only get hit once per attack
	List<Entity> damageQueue;
	List<Entity> targetsHit;

	protected override void Awake () {
		base.Awake ();
		anim = GetComponent<Animator> ();
		//durability = MAX_DURABILITY;

		targetsHit = new List<Entity> ();
		damageQueue = new List<Entity> ();
		owner = GetComponentInParent<Entity> ();

	}

	public void StartAttack(int dir) {
		anim.SetBool ("attack", true);
		anim.SetInteger ("dir", dir);
		LayerSprite (dir);
	}

	public void StartBlock(int dir) {
		anim.SetBool ("block", true);
		anim.SetInteger ("dir", dir);
		LayerSprite (dir);
	}

	// Layer the sprite renderer so that the weapon is either in front the user or behind the user
	void LayerSprite(int dir) {
		if (dir == (int)Entity.Dir.Down)
			sRend.sortingOrder = owner.GetComponent<SpriteRenderer> ().sortingOrder + 1;
		else 
			sRend.sortingOrder = owner.GetComponent<SpriteRenderer> ().sortingOrder - 1;
	}

	// Called by animator
	public void EndSwing() {
		anim.SetBool ("attack", false);
		anim.SetBool ("block", false);
		targetsHit.Clear ();
	}

	public bool IsAttacking() {
		return anim.GetBool ("attack");
	}

	public bool CanDamage(Entity e) {
		return !targetsHit.Contains (e);
	}

	public void AddToDamageQueue(Entity e) {
		damageQueue.Add (e);
	}

	public void AddToDamaged(Entity e) {
		targetsHit.Add (e);
	}

	// Called by opponent when colliding with them
	// Return damage done to opponent
	public float DamageToInflict() {
		return 1;
	}

	public float GetRange() {
		return .75f;
	}

	void OnTriggerEnter2D(Collider2D other) {
		
		// If collided with opponent's hitbox (weapon)
		if (other.gameObject.tag.Equals ("Hitbox")) {
			Weapon otherW = other.GetComponent<Weapon> ();
			if (IsAttacking () && !otherW.IsAttacking ()) {
				Entity entity = GetEntity ();
				if (otherW.CanDamage(entity)) {
					//AddToDamageQueue (entity);
					entity.Damage (otherW.DamageToInflict ());
					otherW.AddToDamaged(entity);
					//print ("Damaging: " + entity.name);
				}
			} else if (!IsAttacking () && otherW.IsAttacking ()) {
				Entity enemy = otherW.GetEntity ();
				if (!targetsHit.Contains (enemy)) {
					//AddToDamageQueue (enemy);
					enemy.Damage (DamageToInflict ());
					targetsHit.Add (enemy);
					//print ("Damaging: " + enemy.name);
				}
			}
		} else if (other.gameObject.tag.Equals ("Character")) {
			// If collided with opponent
			if (IsAttacking ()) {
				Entity enemy = other.GetComponentInParent<Entity>();
				// Don't hurt ourselves
				if (enemy.Equals (GetEntity ()))
					return;
				if (enemy.InAttack ())
					return;
				if (!targetsHit.Contains (enemy)) {
					//AddToDamageQueue (enemy);
					enemy.Damage (DamageToInflict ());
					targetsHit.Add (enemy);
					//print ("Damaging: " + enemy.name + "  " + other.name);
				}
			}
		} else if (other.gameObject.tag.Equals ("Wall")) {
			TileBreakable tb = other.gameObject.GetComponent<TileBreakable> ();
			if (tb != null) { 
				tb.UseItem (this); 
			} 
		}
	}
}
