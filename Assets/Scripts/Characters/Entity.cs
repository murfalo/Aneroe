using UnityEngine;
using System.Collections;

[System.Serializable]
public class Entity : MonoBehaviour {

	// Components
	Animator anim;
	Weapon activeWeapon;
	//Collider2D hurtbox;
	public Inventory inv;

	// Combat stats
	public float MAX_HEALTH = 10;
	public float speed = 1f;
	public float attack = 1f;
	public float defense = 1f;
	float health;

	// Collision stats
	public float characterRadius;

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
		activeWeapon = GetComponentInChildren<Weapon> ();
		inv = new Inventory();
		//pack = GetComponent<Backpack> ();
		//hurtbox = GetComponent<Collider2D> ();
	}

    /// <summary>Subscribe to the inventory controller events to handle UI events appropriately.</summary>
    void Start()
    {
        InventoryController.itemMoved += OnItemMoved;
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

    void FixedUpdate() {
		//print (gameObject.name + "  " + anim.GetInteger ("state") + "  " + anim.GetInteger ("dir"));
		int dir = anim.GetInteger ("dir") - 1;
		switch (anim.GetInteger ("state")) {
		case (int)CharacterState.Walking:
			//print (gameObject.name + "  " + (speed * Time.deltaTime * directionVectors[dir].x) + "  " + (speed * Time.deltaTime * directionVectors[dir].y));
			Vector3 move = speed * Time.fixedDeltaTime * directionVectors [dir];
			RaycastHit2D r = Physics2D.Raycast (transform.position, (Vector2)move, move.magnitude + characterRadius, 1 << LayerMask.NameToLayer ("Wall"));

			if (r.collider == null) {
				transform.Translate (move);
			}
				
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
		activeWeapon.StartAttack (GetDirection());
	}

	public void SetBlocking() {
		if (((CharacterState)anim.GetInteger ("state")) != CharacterState.Still)
			return;
		anim.SetInteger ("state", (int)CharacterState.Blocking);
		activeWeapon.StartBlock (GetDirection());
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
		if (health <= 0) {
			Kill ();
		}
		return false;
	}

	public void Kill() {
		anim.SetInteger ("state", (int)CharacterState.Dead);
		foreach (Collider2D col in GetComponents<Collider2D>()) {
			col.enabled = false;
		}
	}

	public void interact() {
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
		if (i != null) {
			inv.AddItem (i);
			i.gameObject.GetComponent<Renderer> ().enabled = false;
			i.gameObject.GetComponent<Collider2D> ().enabled = false;
		}

	}
}
