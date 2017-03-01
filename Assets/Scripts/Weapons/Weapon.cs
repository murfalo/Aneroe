using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SaveData;

public class Weapon : Item {

	public static float MAX_DURABILITY;
	Animator anim;

	// Properties
	public string[] statNames;
	public float[] statValues;

	StatInfo stats;

	/*public float speed = 1f;
	public float attack = 1f;
	public float defense = 1f;
	float durability;*/

	// targets hit by current attack
	// a target can only get hit once per attack
	//List<Entity> damageQueue;
	List<Entity> targetsHit;

	public override void Setup () {
		base.Setup ();
		anim = GetComponent<Animator> ();
		//durability = MAX_DURABILITY;

		targetsHit = new List<Entity> ();
		//damageQueue = new List<Entity> ();
		owner = GetComponentInParent<Entity> ();

		stats = new StatInfo ();
		for (int i = 0; i < statNames.Length; i++) {
			stats.SetStat (statNames [i], statValues [i]);
		}
	}

	public void StartAttack(int dir) {
		anim.SetBool ("attack", true);
		// SIMILAR ISSUE WITH PLAYER ENTITY ANIMATIONS. NEED TO COMPARE LOCAL STATE TO CURRENT STATE OF ANIM
		anim.SetTime (0);
		SetWeaponDir (dir);
	}

	public void StartBlock(int dir) {
		anim.SetBool ("block", true);
		// SIMILAR ISSUE WITH PLAYER ENTITY ANIMATIONS. NEED TO COMPARE LOCAL STATE TO CURRENT STATE OF ANIM
		anim.SetTime (0);
		SetWeaponDir (dir);
	}

	public void SetWeaponDir(int dir) {
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

	public override void EquipItem(bool equip) {
		sRend.enabled = equip;
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

	public bool IsBlocking() {
		return anim.GetBool ("block");
	}

	public bool HasHitbox() {
		return GetComponent<Collider2D> ().enabled;
	}

	public bool CanDamage(Entity e) {
		return !targetsHit.Contains (e);
	}

	//public void AddToDamageQueue(Entity e) {
	//	damageQueue.Add (e);
	//}

	public void AddToDamaged(Entity e) {
		targetsHit.Add (e);
	}

	public override void PickupItem (Entity e)
	{
		base.PickupItem (e);
		anim.SetBool ("inInv", true);
	}

	public override void DropItem (Vector3 pos)
	{
		base.DropItem (pos);
		anim.SetBool ("inInv", false);
	}

	// Called by opponent when colliding with them
	// Return damage done to opponent
	public void DealDamage(Entity damageE) {
		// Replace with properties of weapon eventually
		int damage = 1;
		float stunTime = .1f;
		float stunVel = 5f;
		damageE.Damage (damage, GetEntity ().GetDirection (), stunTime, stunVel);
	}

	public float GetRange() {
		return stats.GetStat("range");
	}

	void OnTriggerEnter2D(Collider2D other) {

		// If interactive tile
		if (((1 << other.gameObject.layer) & LayerMask.GetMask (new string[] { "InteractiveBlock", "InteractivePass" })) > 0) {
			Tile tile = other.GetComponentInParent<Tile> ();
			if (tile != null && tile.CanUseItem (this))
				GetEntity ().TriggerItemUse (tile);
		}
			
		// If collided with opponent's hitbox (weapon)
		if (other.gameObject.tag.Equals ("Hitbox")) {
			Weapon otherW = other.GetComponent<Weapon> ();
			if (IsAttacking () && !otherW.IsAttacking ()) {
				Entity entity = GetEntity ();
				if (otherW.CanDamage (entity)) {
					otherW.DealDamage (entity);//entity.Damage (otherW.DamageToInflict ());
					otherW.AddToDamaged (entity);
					//print ("Damaging: " + entity.name);
				}
			} else if (!IsAttacking () && otherW.IsAttacking ()) {
				Entity enemy = otherW.GetEntity ();
				if (!targetsHit.Contains (enemy)) {
					DealDamage (enemy);//enemy.Damage (DamageToInflict ());
					targetsHit.Add (enemy);
					//print ("Damaging: " + enemy.name);
				}
			} else if (!otherW.IsBlocking()) {
				TradeBlows (otherW);
			}
		} else if (other.gameObject.tag.Equals ("Character")) {
			// If collided with opponent
			if (IsAttacking ()) {
				Entity enemy = other.GetComponentInParent<Entity>();
				// Don't hurt ourselves
				if (enemy.Equals (GetEntity ()))
					return;
				if (!CanDamage (enemy))
					return;
				if (!enemy.InAttack() || enemy.IsIdleAttack()) {
					// Enemy but they are in lingering attack state, so still hit them
					DealDamage (enemy);
					AddToDamaged (enemy);
				}
			}
		}
	}

	void TradeBlows(Weapon otherW) {
		// Both are attacking, so trade blows
		Entity entity = GetEntity ();
		Entity enemy = otherW.GetEntity ();
		if (otherW.CanDamage (entity)) {
			otherW.DealDamage (entity);
			otherW.AddToDamaged (entity);
		}
		if (CanDamage (enemy)) {
			DealDamage (enemy);
			AddToDamaged (enemy);
		}
	}

	public override Hashtable Save ()
	{
		Hashtable wsd = base.Save();
		// Save statInfo now
		wsd.Add("statLevels", stats.GetStats());
		return wsd;
	}

	public override void Load (Hashtable wsd)
	{
		Setup ();
		base.Load (wsd);
		stats = new StatInfo((Dictionary<string, float>)wsd["statLevels"]);
	}
}
