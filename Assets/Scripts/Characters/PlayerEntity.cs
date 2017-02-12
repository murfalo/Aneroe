﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity {

	float DIRECTION_TAP_BUFFER = .2f;
	float LINK_ATTACKS_BUFFER = .1f;

	protected int combatDir;
	protected float combatDirTimer;
	protected float combatLinkTimer;
	public Inventory inv;

	// For use in queue comparisons
	CharacterStateAction[] defaultActions;

	// Internal states
	PriorityQueue<CharacterStateAction> queuedStateActions;

	public override void Setup() {
		base.Setup ();

		queuedStateActions = new PriorityQueue<CharacterStateAction> ();
		defaultActions = new CharacterStateAction[System.Enum.GetNames (typeof(CharacterState)).Length];
		for (int i = 0; i < defaultActions.Length; i++) {
			defaultActions[i] = new CharacterStateAction ((CharacterState)i);
		}

		combatDirTimer = 0;
		combatLinkTimer = 0;

		controller = GameObject.Find("Control").GetComponent<EntityController> ();
		inv = new Inventory();
		// Subscribe to the inventory controller events to handle UI events appropriately.
		InventoryController.itemMoved += OnItemMoved;
	}

	public override void DoFixedUpdate() {
		StateQueueUpdate ();
		// Timer updates
		if (combatDirTimer > 0 && DecrementTimer (combatDirTimer, out combatDirTimer)) {
			combatDir = 0;
			combatDirTimer = 0;
		} 
		if (combatLinkTimer > 0 && DecrementTimer (combatLinkTimer, out combatLinkTimer)) {
			combatLinkTimer = 0;
		}

		base.DoFixedUpdate ();
	}

	void StateQueueUpdate() {
		CharacterStateAction action;
		PriorityQueue<CharacterStateAction> newQueue = new PriorityQueue<CharacterStateAction>(); 
		while ((action = queuedStateActions.Dequeue()) != default(CharacterStateAction)) {
			switch (action.state) {
			case CharacterState.Still:
				if (CanActOutOfMovement () && primaryDir > 0 && CanSwitchCombatDirection()) {
					anim.SetInteger ("dir", primaryDir);
				} else {
					newQueue.Enqueue (action);
				}
				break;
			case CharacterState.Walking:		
				// Alternate the step this walk cycle executes with
				oddStep = !oddStep;
				anim.SetTime (0);
				anim.SetInteger ("state", (int)CharacterState.Walking);
				anim.SetBool ("oddStep", oddStep);
				speedFactor = NORMAL_SPEED_FACTOR;
				break;
			case CharacterState.Attacking:
				if (!CanActOutOfMovement ()) {
					break;
				}
				anim.SetTime (0);
				anim.SetInteger ("state", (int)CharacterState.Attacking);
				activeWeapon.StartAttack (SwitchToCombatDirection());
				speedFactor = ATTACK_SPEED_FACTOR;
				break;
			case CharacterState.Blocking:
				if (!CanActOutOfMovement ()) {
					break;
				}
				anim.SetTime (0);
				anim.SetInteger ("state", (int)CharacterState.Blocking);
				activeWeapon.StartBlock (SwitchToCombatDirection());
				speedFactor = BLOCK_SPEED_FACTOR;
				break;
			default:
				//print (gameObject.name + "  " + new Vector2 (0, 0));
				break;
			}
		}
		queuedStateActions = newQueue;
	}

	public override void EndWeaponUseAnim() {
		anim.SetInteger ("state", (int)CharacterState.Still);
		speedFactor = NORMAL_SPEED_FACTOR;
		combatLinkTimer = LINK_ATTACKS_BUFFER;
	}

	// Sets animation state for walking
	public override void TryWalk() {
		if (GetState() != CharacterState.Still)
			return;
		queuedStateActions.Enqueue (new CharacterStateAction(CharacterState.Walking));
	}

	// Sets animation state for attacking
	public override void TryAttacking() {
		if (!queuedStateActions.ContainsByCompare(defaultActions[(int)CharacterState.Attacking]))
			queuedStateActions.Enqueue (new CharacterStateAction(CharacterState.Attacking));
	}

	public override void TryBlocking() {
		if (!queuedStateActions.ContainsByCompare(defaultActions[(int)CharacterState.Blocking]))
			queuedStateActions.Enqueue (new CharacterStateAction(CharacterState.Blocking));
	}

	// Processes direction input of two kinds:
	// dirsDown: Was the direction pressed or held down?
	// dirsTapped: Was the direction just pressed?
	public void SetDirections(bool[] dirsDown, bool[] dirsTapped) {
		if (!CanActOutOfMovement () && !InAttack()) {
			return;
		}

		for (int i = 0; i < 4; i++) {
			if (dirsTapped [i] && combatDir != i + 1) {
				combatDirTimer = DIRECTION_TAP_BUFFER;
				combatDir = i + 1;
				break;
			}
		}

		// If primary direction isn't still held, reset primaryDir;
		if (primaryDir > 0 && !dirsDown [primaryDir - 1]) {
			primaryDir = 0;
			if (secondaryDir > 0 && dirsDown [secondaryDir - 1])
				primaryDir = secondaryDir;
			secondaryDir = 0;
		}
		if (secondaryDir > 0 && !dirsDown [secondaryDir - 1]) {
			secondaryDir = 0;
		}
		for (int i = 0; i < 4; i++) {
			if (dirsDown [i]) {
				if (primaryDir == 0)
					primaryDir = i + 1;
				else if (secondaryDir == 0 && (primaryDir + i + 1) % 2 != 0)
					secondaryDir = i + 1;
			}
		}

		if (!queuedStateActions.ContainsByCompare(defaultActions[(int)CharacterState.Still]))
			queuedStateActions.Enqueue (new CharacterStateAction(CharacterState.Still));
	}

	protected bool CanSwitchCombatDirection() {
		return combatLinkTimer == 0;
	}

	protected int SwitchToCombatDirection() {
		if (CanSwitchCombatDirection() && combatDir > 0) {
			anim.SetInteger ("dir", combatDir);
			return combatDir;
		}
		return GetDirection();
	}

	/// <summary>Event handler for the itemMoved event provided by ItemController.</summary>
	/// <param name="source">Originator of itemMoved event.</param>
	/// <param name="eventArgs">Useful context of the itemMoved event.</param>
	public void OnItemMoved(object source, InventoryEvents.ItemMovedEventArgs eventArgs)
	{
		inv.RemoveItem(eventArgs.prevSlot);
		if (eventArgs.newSlot != null)
			inv.SetItem((int)eventArgs.newSlot, eventArgs.item);
	}

	public void TryInteracting() {
		if (!CanActOutOfMovement())
			return;
		// right now, it only tries to place first item into ItemMound below
		Item i = inv.GetItem(0);
		Collider2D coll = Physics2D.OverlapCircle (transform.position, 0.01f, 1 << LayerMask.NameToLayer ("ItemMound"));
		if (i != null && coll != null && coll.gameObject.GetComponent<ItemMound> ().CanUseItem (i)) {
			coll.gameObject.GetComponent<ItemMound> ().UseItem (i);
			inv.RemoveItem (0);
			print("Putting Item into mound");
		} else if (coll != null) {
			inv.AddItem (coll.gameObject.GetComponent<ItemMound> ().RetrieveItem (this));
			print ("Taking Item out of mound");
		} else {
			print ("Miss");
		}
	}

	public void OnTriggerEnter2D (Collider2D coll) {
		// Attempting to pick up items off the ground
		Item i = coll.gameObject.GetComponent<Item> ();
		if (i != null && i.GetEntity() == null) {
			print (i.transform.parent.name);
			inv.AddItem (i);
			i.gameObject.GetComponent<Renderer> ().enabled = false;
			i.gameObject.GetComponent<Collider2D> ().enabled = false;
		}

	}
}