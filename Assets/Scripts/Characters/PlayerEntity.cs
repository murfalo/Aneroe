using System;
using System.Collections;
using System.Collections.Generic;
using SaveData;
using UIEvents;
using UnityEngine;

public class PlayerEntity : Entity
{
    private readonly float DIRECTION_TAP_BUFFER = .2f;
    private readonly float LINK_ATTACKS_BUFFER = .1f;

    protected int combatDir;
    protected float combatDirTimer;
    protected float combatLinkTimer;

    public event EventHandler<ItemInteractEventArgs> itemInteracted;

    public Inventory inv;
	public string[] defaultItemPrefabNames;
	private Tile interactTile;

    // For use in queue comparisons
    private CharacterStateAction[] defaultActions;

    // Internal states
    private PriorityQueue<CharacterStateAction> queuedStateActions;

    // Position offsets from transform.position for interact detection in each of four directions
    private Vector2[] interactOffsets;
    private float interactRadius;
    private int interactLayerMask;

    public new Item activeItem
    {
        get
        {
            var activeInv = PlayerController.activeCharacter.inv;
            return activeInv.GetItem(activeInv.itemSlotEquipped);
        }
    }

    protected new bool ActiveItemOfType<T>(T typeOfObj)
    {
        return activeItem != null && activeItem.GetType().Equals(typeOfObj);
    }

    public override void Setup()
    {
        base.Setup();

        queuedStateActions = new PriorityQueue<CharacterStateAction>();
        defaultActions = new CharacterStateAction[Enum.GetNames(typeof(CharacterState)).Length];
        for (var i = 0; i < defaultActions.Length; i++)
            defaultActions[i] = new CharacterStateAction((CharacterState) i);
        var col = GetComponent<BoxCollider2D>();
        interactLayerMask = LayerMask.GetMask("InteractiveTile", "Item");
        interactRadius = .5f * ((col.size.x + col.size.y) / 2);
        interactOffsets = new Vector2[4]
        {
            new Vector2(0, 1.25f * (.5f * col.size.y)),
            new Vector2(1.5f * (.5f * col.size.x), 0),
            new Vector2(0, -1.75f * (.5f * col.size.y)),
            new Vector2(-1.5f * (.5f * col.size.x), 0)
        };
        combatDirTimer = 0;
        combatLinkTimer = 0;

        controller = GameObject.Find("Control").GetComponent<EntityController>();
        inv = new Inventory();
    }

    public override void DoFixedUpdate()
    {
        StateQueueUpdate();
        // Timer updates
        if (combatDirTimer > 0 && DecrementTimer(combatDirTimer, out combatDirTimer))
        {
            combatDir = 0;
            combatDirTimer = 0;
        }
        if (combatLinkTimer > 0 && DecrementTimer(combatLinkTimer, out combatLinkTimer)) combatLinkTimer = 0;

        base.DoFixedUpdate();
    }

    private void StateQueueUpdate()
    {
        CharacterStateAction action;
        var newQueue = new PriorityQueue<CharacterStateAction>();
        while ((action = queuedStateActions.Dequeue()) != default(CharacterStateAction))
            switch (action.state)
            {
                case CharacterState.Still:
                    if (CanActOutOfMovement() && primaryDir > 0 && CanSwitchCombatDirection())
                        anim.SetInteger("dir", primaryDir);
                    else newQueue.Enqueue(action);
                    break;
                case CharacterState.Walking:
                    // Alternate the step this walk cycle executes with
                    oddStep = !oddStep;
                    anim.SetTime(0);
                    anim.SetInteger("state", (int) CharacterState.Walking);
                    anim.SetBool("oddStep", oddStep);
                    speedFactor = NORMAL_SPEED_FACTOR;
                    break;
                case CharacterState.Attacking:
                    if (!CanActOutOfMovement() || !ActiveItemOfType(typeof(Weapon)))
                        break;
                    anim.SetTime(0);
                    anim.SetInteger("state", (int) CharacterState.Attacking);
                    ((Weapon) activeItem).StartAttack(SwitchToCombatDirection());
                    speedFactor = ATTACK_SPEED_FACTOR;
                    break;
                case CharacterState.Blocking:
                    if (!CanActOutOfMovement() || !ActiveItemOfType(typeof(Weapon)))
                        break;
                    anim.SetTime(0);
                    anim.SetInteger("state", (int) CharacterState.Blocking);
                    ((Weapon) activeItem).StartBlock(SwitchToCombatDirection());
                    speedFactor = BLOCK_SPEED_FACTOR;
                    break;
				case CharacterState.Digging:
					if (!CanActOutOfMovement ())
						break;
					TriggerItemUse ();
					break;
				default:
                    //print (gameObject.name + "  " + new Vector2 (0, 0));
                    break;
            }
        queuedStateActions = newQueue;
    }

	// Animator signals when an attack is over
    public override void EndWeaponUseAnim()
    {
        anim.SetInteger("state", (int) CharacterState.Still);
        speedFactor = NORMAL_SPEED_FACTOR;
        combatLinkTimer = LINK_ATTACKS_BUFFER;
    }

	// Animator signals when to use item in interaction animation
	public void TriggerItemUse()
	{
		if (interactTile.UseItem (activeItem)) {
			// If item was used up, get rid of it in inventory
			HandleItemRemove(activeItem);
		}
	}

    // Sets animation state for walking
    public override void TryWalk()
    {
        if (GetState() != CharacterState.Still)
            return;
        queuedStateActions.Enqueue(new CharacterStateAction(CharacterState.Walking));
    }

    public override void TryAttacking()
    {
		if (activeItem == null)
			return;
		CharacterState newState = default(CharacterState);
		Tile interactable;
		if (activeItem.GetType ().Equals (typeof(Weapon))) {
			newState = CharacterState.Attacking;
		} else if ((interactable = GetInteractableTile ()) != null) {
			if (interactable.CanUseItem (activeItem)) {
				newState = interactable.GetInteractState ();
				interactTile = interactable;
			}
		}
		if (newState != default(CharacterState) && !queuedStateActions.ContainsByCompare (defaultActions [(int)newState]))
			queuedStateActions.Enqueue (new CharacterStateAction (newState));
    }

    public override void TryBlocking()
	{
		if (activeItem == null)
			return;
        if (!queuedStateActions.ContainsByCompare(defaultActions[(int) CharacterState.Blocking]))
            queuedStateActions.Enqueue(new CharacterStateAction(CharacterState.Blocking));
    }

    // Processes direction input of two kinds:
    // dirsDown: Was the direction pressed or held down?
    // dirsTapped: Was the direction just pressed?
    public void SetDirections(bool[] dirsDown, bool[] dirsTapped)
    {
        if (!CanActOutOfMovement() && !InAttack()) return;

        for (var i = 0; i < 4; i++)
            if (dirsTapped[i] && combatDir != i + 1)
            {
                combatDirTimer = DIRECTION_TAP_BUFFER;
                combatDir = i + 1;
                break;
            }

        // If primary direction isn't still held, reset primaryDir;
        if (primaryDir > 0 && !dirsDown[primaryDir - 1])
        {
            primaryDir = 0;
            if (secondaryDir > 0 && dirsDown[secondaryDir - 1])
                primaryDir = secondaryDir;
            secondaryDir = 0;
        }
        if (secondaryDir > 0 && !dirsDown[secondaryDir - 1]) secondaryDir = 0;
        for (var i = 0; i < 4; i++)
            if (dirsDown[i])
                if (primaryDir == 0)
                    primaryDir = i + 1;
                else if (secondaryDir == 0 && (primaryDir + i + 1) % 2 != 0)
                    secondaryDir = i + 1;

        if (!queuedStateActions.ContainsByCompare(defaultActions[(int) CharacterState.Still]))
            queuedStateActions.Enqueue(new CharacterStateAction(CharacterState.Still));
    }

    protected bool CanSwitchCombatDirection()
    {
        return combatLinkTimer == 0;
    }

    protected int SwitchToCombatDirection()
    {
        if (CanSwitchCombatDirection() && combatDir > 0)
        {
            anim.SetInteger("dir", combatDir);
            return combatDir;
        }
        return GetDirection();
    }

    public void OnItemMoved(ItemMovedEventArgs eventArgs)
    {
        if (eventArgs.prevSlot >= 0)
            inv.RemoveItem(eventArgs.prevSlot);
        if (eventArgs.newSlot >= 0)
            inv.SetItem((int) eventArgs.newSlot, eventArgs.item);
    }

    public void ResetActiveItem()
    {
        // Unequip old item
        if (activeItem != null)
            activeItem.EquipItem(false);
        if (activeItem != null)
			activeItem.EquipItem(true);
    }

    public Vector2 GetInteractPosition()
    {
        return (Vector2) transform.position + interactOffsets[GetDirection() - 1];
    }

	Tile GetInteractableTile() {
		Vector2 dir = directionVectors [GetDirection () - 1];
		float dist = Vector2.Distance(GetInteractPosition (),(Vector2)transform.position);
		RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, hurtbox.bounds.size, 0.0f, dir, dist, 1 << LayerMask.NameToLayer("InteractiveTile"));
		if (hits.Length > 0)
			return hits [0].collider.GetComponentInChildren<Tile> ();
		return null;
	}

	/*
    public void TryInteracting()
    {
        if (!CanActOutOfMovement())
            return;
        var cols = Physics2D.OverlapCircleAll(GetInteractPosition(), interactRadius, interactLayerMask);
        if (cols.Length == 0)
        {
        }
        else if (cols.Length == 1)
        {
            if (cols[0].gameObject.layer == LayerMask.NameToLayer("Item"))
                if (!inv.IsFull())
                {
					var i = cols[0].GetComponent<Item>();
					HandleItemPickup (i);
                }
        }
        else
        {
            //Change later to not just pick an item but display ui for options of choice
            if (cols[0].gameObject.layer == LayerMask.NameToLayer("Item"))
                if (!inv.IsFull())
                {
                    var i = cols[0].GetComponent<Item>();
					HandleItemPickup (i);
                }
        }
    }
    */

	void HandleItemPickup(Item i) {
		i.PickupItem (this);
		inv.AddItem (i);
		if (itemInteracted != null)
			itemInteracted(this, new ItemInteractEventArgs(i, inv, true));
	}

	void HandleItemRemove(Item i) {
		int oldIndex = inv.SlotOf (i);
		inv.RemoveItem (oldIndex);
		if (itemInteracted != null)
			itemInteracted(this, new ItemInteractEventArgs(i, inv, false, oldIndex));
	}

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer.Equals(LayerMask.NameToLayer("Item")))
        {
            // THIS HURTS ME SO MUCH. IT PICKS ITEMS UP AUTOMATICALLY. But leave it in.
			var i = coll.GetComponent<Item>();
			HandleItemPickup (i);

            // Attempting to pick up items off the ground
            /*if (topItemOnGround == null || topItemOnGround.GetComponent<SpriteRenderer>().sortingOrder < coll.GetComponent<SpriteRenderer>().sortingOrder) {
                topItemOnGround = coll.gameObject.GetComponent<Item> ();
            }*/
        }
    }

    public Hashtable Save()
    {
        var esd = new Hashtable();
		esd.Add("posX", transform.position.x);
		esd.Add("posY", transform.position.y);
		esd.Add("statLevels", stats.GetStats());
		esd.Add ("itemcount", inv.maxItems);
		for (int i = 0; i < inv.maxItems; i++) {
			Item item = inv.GetItem (i);
			if (item != null)
				esd.Add ("item" + i, item.Save ());
		}
        return esd;
    }

    public void Load(Hashtable esd)
    {
		transform.position = new Vector3((float)esd["posX"], (float)esd["posY"], 0);
		stats = new StatInfo((Dictionary<string, float>)esd["statLevels"]);
		controller.RespondToEntityAction(this, "health");

		inv = new Inventory();
		for (int i = 0; i < (int)esd["itemcount"]; i++) {
			if (!esd.ContainsKey ("item" + i))
				continue;
			Hashtable itemSave = (Hashtable)esd["item" + i];
			GameObject itemObj = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Items/" + (string)itemSave["prefabName"]));
			itemObj.transform.SetParent (transform);
			Item item = itemObj.GetComponentInChildren<Item> ();
			item.Load (itemSave);		
			// Configure item and inventory, but do NOT send event to inventory that this is being added. That would cause duplication
			item.PickupItem (this);
			inv.SetItem (i,item);
		}

        // Set active item
        ResetActiveItem();
    }

    public void LoadFirstTime()
    {
		foreach (string itemName in defaultItemPrefabNames) {
			var itemObj = Instantiate (Resources.Load<GameObject> ("Prefabs/Items/" + itemName));
			itemObj.transform.SetParent (transform);
			Item i = itemObj.GetComponentInChildren<Item> ();
			i.Setup();	
			// Configure item and inventory, but do NOT send event to inventory that this is being added. That would cause duplication
			i.PickupItem (this);
			inv.AddItem (i);
		}

		// Destroy old entity items
		//foreach (var item in GetComponentsInChildren<Item>()) Destroy(item.gameObject);
        // Set active item
        ResetActiveItem();
    }
}
