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
            return inv.GetItem(inv.itemSlotEquipped);
        }
    }

	// Since activeItem variable gets overwritten, use this to figure out how to compare the variable to a type
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
        //interactLayerMask = LayerMask.GetMask("InteractiveBlock", "Interactive Pass", "Item");
        //interactRadius = .5f * ((col.size.x + col.size.y) / 2);

		// REPLACE WITH CODE THAT CALCULATES FROM EITHER PLAYER OR ACTIVE HITBOX ON ITEM
        interactOffsets = new Vector2[4]
        {
            2*new Vector2(0, 1.25f * (.5f * col.size.y)),
            2*new Vector2(1.5f * (.5f * col.size.x), 0),
            2*new Vector2(0, -1.75f * (.5f * col.size.y)),
            2*new Vector2(-1.5f * (.5f * col.size.x), 0)
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
						SetDir(primaryDir);
                    else newQueue.Enqueue(action);
                    break;
			case CharacterState.Walking:
					// Not able to trigger a walk
				if (GetState () != CharacterState.Still)
					break;
                    // Alternate the step this walk cycle executes with
                    oddStep = !oddStep;
					anim.SetInteger("state", (int) CharacterState.Walking);
                    anim.SetBool("oddStep", oddStep);
                    speedFactor = NORMAL_SPEED_FACTOR;
                    break;
                case CharacterState.Attacking:
                    if (!CanActOutOfMovement() || !ActiveItemOfType(typeof(Weapon)))
                        break;
					anim.SetInteger("state", (int) CharacterState.Attacking);
					// REPLACE WITH LOCAL STORAGE OF STATE TO PREVENT FUNKY ANIM STUFF
					anim.SetTime(0);
                    ((Weapon) activeItem).StartAttack(SwitchToCombatDirection());
                    speedFactor = ATTACK_SPEED_FACTOR;
                    break;
                case CharacterState.Blocking:
                    if (!CanActOutOfMovement() || !ActiveItemOfType(typeof(Weapon)))
                        break;
					anim.SetInteger("state", (int) CharacterState.Blocking);
					// REPLACE WITH LOCAL STORAGE OF STATE TO PREVENT FUNKY ANIM STUFF
					anim.SetTime(0);
                    ((Weapon) activeItem).StartBlock(SwitchToCombatDirection());
                    speedFactor = BLOCK_SPEED_FACTOR;
                    break;
				case CharacterState.Interacting:
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
	public override void TriggerItemUse(Tile tile = null)
	{
		if (tile != null)
			interactTile = tile;
		Item newItem;
		bool keepOldItem = interactTile.UseItem (activeItem, out newItem);
		if (newItem == null && activeItem != null) {
			// If item was used up, get rid of it in inventory
			HandleItemRemove(activeItem);
		} else if (newItem != null && newItem != activeItem) {
			if (activeItem != null && !keepOldItem)
				HandleItemRemove (activeItem);
			HandleItemPickup (newItem);
		}
	}

	public override bool IsIdleAttack() 
	{
		return InAttack() && activeItem != null && activeItem.GetType ().Equals (typeof(Weapon)) && !activeItem.GetComponent<Weapon> ().HasHitbox ();
	}

    // Sets animation state for walking
    public override void TryWalk()
	{
        if (GetState() != CharacterState.Still)
            return;
        queuedStateActions.Enqueue(new CharacterStateAction(CharacterState.Walking));
    }

	public void TryInteracting() 
	{
		// Don't use interaction state for weapon. It will use attacking state
		if (activeItem is Weapon)
			return;
		CharacterState newState = default(CharacterState);
		Tile interactable;
		if ((interactable = GetInteractableTile ()) != null) {
			if (interactable.CanUseItem (activeItem)) {
				newState = interactable.GetInteractState ();
				interactTile = interactable;
			}
		} else if (activeItem != null && activeItem.Use()) {
            HandleItemRemove(activeItem);
        }
	    if (newState != default(CharacterState) && !queuedStateActions.ContainsByCompare(defaultActions[(int) newState]))
	        queuedStateActions.Enqueue(new CharacterStateAction(newState));
	}

    public override void TryAttacking()
    {
		if (activeItem == null || !activeItem.GetType ().Equals (typeof(Weapon)))
			return;
		CharacterState newState = CharacterState.Attacking;
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

	public void TryItemEquip(int newSlot) {
		if (!CanActOutOfMovement ())
			return;
		// Unequip, choose new item, reequip
		if (activeItem)
			activeItem.EquipItem (false);
		inv.itemSlotEquipped = newSlot;
		if (activeItem)
			activeItem.EquipItem (true);
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

	// Overrides to use overwritten activeItem field
	public override void SetDir(int dir) {
		anim.SetInteger("dir", dir);
		if (activeItem != null && activeItem.GetType () == typeof(Weapon)) {
			((Weapon)activeItem).SetWeaponDir (dir);
		}
	}

    public void OnItemMoved(ItemMovedEventArgs eventArgs)
    {
        if (eventArgs.prevSlot >= 0)
            inv.RemoveItem(eventArgs.prevSlot);
        if (eventArgs.newSlot >= 0)
            inv.SetItem((int) eventArgs.newSlot, eventArgs.item);
    }
    
    public Vector2 GetInteractPosition()
    {
        return (Vector2) transform.position + interactOffsets[GetDirection() - 1];
    }

	private Tile GetInteractableTile() {
		var dir = directionVectors [GetDirection () - 1];
		var dist = Vector2.Distance(GetInteractPosition (),(Vector2)transform.position);
		var hits = Physics2D.BoxCastAll(transform.position, wallBox.bounds.size, 0.0f, dir, dist, LayerMask.GetMask("InteractiveBlock", "InteractivePass"));
        return hits.Length > 0 ? hits [0].collider.GetComponentInChildren<Tile> () : null;
	}

	void HandleItemPickup(Item i) {
		if (i.GetEntity () == null && !inv.IsFull()) {
			i.PickupItem (this);
			inv.AddItem (i);
			// Make sure if the item is picked up into active item slot, it is equipped
			if (activeItem)
				activeItem.EquipItem (true);
			if (itemInteracted != null)
				itemInteracted (this, new ItemInteractEventArgs (i, inv, true));
		}
	}

	void HandleItemRemove(Item i) {
		int oldIndex = inv.SlotOf (i);
		inv.RemoveItem (oldIndex);
		if (itemInteracted != null)
			itemInteracted(this, new ItemInteractEventArgs(i, inv, false, oldIndex));
	}

    public void OnTriggerEnter2D(Collider2D coll)
    {
		if ((LayerMask.GetMask (new string[] { "Weapon", "Item" }) & (1 << coll.gameObject.layer)) != 0) {
			// THIS HURTS ME SO MUCH. IT PICKS ITEMS UP AUTOMATICALLY. But leave it in.
			var i = coll.GetComponent<Item> ();
			HandleItemPickup (i);
		}
    }

    public Hashtable Save()
    {
        var esd = new Hashtable();
		esd.Add("posX", transform.position.x);
		esd.Add("posY", transform.position.y);
		esd.Add("statLevels", stats.GetStats());
		esd.Add ("itemcount", inv.maxItems);
		esd.Add ("indexequipped", inv.itemSlotEquipped);
		for (int i = 0; i < inv.maxItems; i++) {
			Item item = inv.GetItem (i);
			if (item != null)
				esd.Add ("item" + i, item.Save ());
		}
        return esd;
    }

    public void Load(Hashtable esd)
	{
		// Destroy old entity items
		foreach (var item in GetComponentsInChildren<Item>()) 
			Destroy(item.gameObject);

		transform.position = new Vector3((float)esd["posX"], (float)esd["posY"], 0);
		stats = new StatInfo((Dictionary<string, float>)esd["statLevels"]);
		controller.RespondToEntityAction(this, "health");

		inv = new Inventory();
		inv.itemSlotEquipped = (int)esd ["indexequipped"];
		for (int i = 0; i < (int)esd["itemcount"]; i++) {
			if (!esd.ContainsKey ("item" + i))
				continue;
			Hashtable itemSave = (Hashtable)esd["item" + i];
			GameObject itemObj = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Items/" + (string)itemSave["prefabName"]));
			itemObj.transform.SetParent (transform, false);
			itemObj.transform.localScale = new Vector3 (1, 1, 1);
			Item item = itemObj.GetComponentInChildren<Item> ();
			item.Load (itemSave);		
			// Configure item and inventory, but do NOT send event to inventory that this is being added. That would cause duplication
			item.PickupItem (this);
			inv.SetItem (i,item);
		}

		// Set active item
		if (activeItem != null)
			activeItem.EquipItem(true);
    }

    public void LoadFirstTime()
    {
		foreach (string itemName in defaultItemPrefabNames) {
			var itemObj = Instantiate (Resources.Load<GameObject> ("Prefabs/Items/" + itemName));
			itemObj.transform.SetParent (transform, false);
			itemObj.transform.localScale = new Vector3 (1, 1, 1);
			Item i = itemObj.GetComponentInChildren<Item> ();
			i.Setup();	
			// Configure item and inventory, but do NOT send event to inventory that this is being added. That would cause duplication
			i.PickupItem (this);
			inv.AddItem (i);
		}

        // Set active item
		if (activeItem != null)
			activeItem.EquipItem(true);
    }
}
