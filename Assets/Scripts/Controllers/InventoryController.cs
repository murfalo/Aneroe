using System;
using UnityEngine;
using UnityEngine.UI;
using InventoryEvents;
using System.Reflection;
using AneroeInputs;


/// <summary>
/// This class manages the inventory UI as well as a simple event system
/// to allow easy inventory updating.
/// </summary>
public class InventoryController : BaseController
{
    /// <section>Prefab for an inventory slot.</section>
    [SerializeField] private GameObject UISlot;

	/// <section>Prefab for an inventory item slot.</section>
	[SerializeField] private GameObject InvSlot;

    /// <section>Prefab for a physical (non-UI) item.</section>
    [SerializeField] private GameObject Item;

	private GameObject[] slots;

    /// <section>Item currently selected by the player.</section>
    private GameObject selected { get; set; }

    /// <section>Event for an item moving in the inventory.</section>
    public static event EventHandler<ItemMovedEventArgs> itemMoved;

    /// <section>Original parent of the currently selected item.</section>
    private Transform parent { get; set; }

    /// <section>Initializes the inventory to the size of the currently active character.</section>
    public override void ExternalSetup()
    {
		SceneController.timeSwapped += RefreshInventory<EventArgs>;
		SceneController.timeSwapped += RebindListener;
		SaveController.playerLoaded += RefreshInventory<EventArgs>;

		slots = new GameObject[PlayerController.activeCharacter.inv.maxItems];
		GameObject itemHolder = GameObject.Find ("Items");
		for (int i = 0; i < slots.Length; i++)
        {
            var newSlot = (GameObject)Instantiate(UISlot);
			if (i % 2 != 0) {
				GameObject invSlot = (GameObject)Instantiate (InvSlot);
				invSlot.transform.SetParent (newSlot.transform);
				GameObject item = (GameObject)Instantiate (Item);
				item.transform.SetParent (itemHolder.transform);
				invSlot.GetComponent<InventorySlot> ().SetUnsetItem (item.GetComponent<Item>());
			}
			
            newSlot.name = "Slot." + i.ToString();
            newSlot.transform.SetParent(UIController.inventory.transform);
			slots [i] = newSlot;
        }
    }

	public override void RemoveEventListeners() {
		SceneController.timeSwapped -= RefreshInventory<EventArgs>;
		SceneController.timeSwapped -= RebindListener;
		SaveController.playerLoaded -= RefreshInventory<EventArgs>;
	}

    /// <section>Causes the selected item to follow the mouse cursor.</section>
    public void Update()
    {
        if (selected)
            selected.transform.position = Input.mousePosition;
    }

    /// <section>Selects the target item from a UI slot.</section>
    /// <param name="target">Either a UI item or UI slot to select an item from.</param>
    private void SelectItem(GameObject target)
    {
        if (target.CompareTag("UISlot")) selected = null;
        if (!target.CompareTag("UIItem")) return;
        selected = target;
        parent = selected.transform.parent;
        selected.transform.SetParent(selected.GetComponentInParent<Canvas>().transform);
        target.GetComponent<Image>().raycastTarget = false;
    }

    /// <section>Moves the selected item in a UI slot.</section>
    /// <param name="target">Either a UI item or a UI slot to move the currently selected item into.</param>
    private void MoveItem(GameObject target)
    {
        var prevSelected = selected;
        selected.GetComponent<Image>().raycastTarget = true;
        if (target.CompareTag("UIItem") || target.CompareTag("UISlot"))
        {
            var newParent = (target.CompareTag("UIItem")) ? target.transform.parent : target.transform;
            prevSelected.transform.SetParent(newParent);
            SelectItem(target);
            OnItemMoved(prevSelected, parent, newParent);
        }
        else
        {
            DropItem(prevSelected);
            selected = null;
        }

        prevSelected.transform.position = prevSelected.transform.parent.position;
    }

	public void PickupItem(object source, InventoryEvents.ItemPickupEventArgs e)
	{
		var uiItem = slots [e.inventory.NextAvailableSlot ()];
		GameObject invItem = ((GameObject)Instantiate (InvSlot));
		invItem.transform.SetParent (uiItem.transform);
		invItem.GetComponent<InventorySlot> ().SetUnsetItem (e.item);
	}

    /// <section>Drops the selected item from the inventory.</section>
    /// <param name="item">The item to drop from the inventory.</param>
    private void DropItem(GameObject item)
    {
        var activeCharacter = PlayerController.activeCharacter;
		var newItem = item.GetComponent<InventorySlot>().GetItem();
        //newItem.transform.SetParent(GameObject.Find("Items").transform);
		newItem.DropItem(activeCharacter.GetInteractPosition());
        OnItemMoved(item, parent);
        Destroy(item);
    }

    /// <section>Selects an item from or drops an item into a UI slot on left click.</section>
    /// <param name="eventData">Data about the pointer event.</param>
    public void HandlePointerClick(GameObject target)
    {
        //Debug.Log(selected);
        if (selected)
            MoveItem(target);
        else
            SelectItem(target);
    }

    /// <section>Publishes the itemMoved event if an item has changed positions in the inventory.</section>
    private void OnItemMoved(GameObject go, Transform prevParent, Transform newParent = null)
    {
        int prevSlot, newSlot = -1;
        var item = go.GetComponent<Item>();
        Int32.TryParse(prevParent.name.Split('.')[1], out prevSlot);
        if (newParent)
            Int32.TryParse(newParent.name.Split('.')[1], out newSlot);
        if (itemMoved != null && newSlot != prevSlot)
            itemMoved(this, new InventoryEvents.ItemMovedEventArgs(item, prevSlot, newSlot));
    }

    /// <section>Refreshes the inventory using the active player's inventory data.</section>
    /// <param name="source">Publisher of event.</section>
    /// <param name=""></section>
    private void RefreshInventory<T>(object source, T eventArgs)
    {
        if (!typeof(T).IsAssignableFrom(typeof(EventArgs))) return;
        // Do things

    }

	private void RebindListener(object source, PlayerSwitchEventArgs e) {
		if (e.oldPlayer)
			e.oldPlayer.itemPickup -= PickupItem;
		e.newPlayer.itemPickup += PickupItem;
	}
}
