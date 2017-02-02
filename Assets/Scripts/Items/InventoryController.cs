<<<<<<< HEAD
﻿
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InventoryEvents;


/// <summary>
/// This class manages the inventory UI as well as a simple event system
/// to allow easy inventory updating.
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class manages 
>>>>>>> Implement limited, click-based UI
/// </summary>
public class InventoryController : MonoBehaviour, IPointerClickHandler
{

    /// <section>Prefab for an inventory slot.</section>
    [SerializeField] GameObject UISlot;

    /// <section>Item currently selected by the player.</section>
<<<<<<< HEAD
    private GameObject selected { get; set; }

    /// <section>Event for an item moving in the inventory.</section>
    public static event EventHandler<ItemMovedEventArgs> itemMoved;

    /// <section>Original parent of the currently selected item.</section>
    private Transform parent { get; set; }
=======
    public GameObject selected { get; set; }
>>>>>>> Implement limited, click-based UI

    /// <section>Initializes the inventory to the size of the currently active character.</section>
    public void Start()
    {
        for (int i = 0; i < PlayerController.activeCharacter.inv.maxItems; i++)
        {
            var newSlot = (GameObject)Instantiate(UISlot);
            if (i % 2 == 0)
                Destroy(newSlot.transform.GetChild(0).gameObject);
            newSlot.name = "Slot." + i.ToString();
<<<<<<< HEAD
            newSlot.transform.SetParent(transform.GetChild(0).transform);
=======
            newSlot.transform.SetParent(transform);
>>>>>>> Implement limited, click-based UI
        }
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
<<<<<<< HEAD
        if (target.CompareTag("UISlot")) selected = null;
        if (!target.CompareTag("UIItem")) return;
        selected = target;
        parent = selected.transform.parent;
=======
        if (!target.CompareTag("UIItem") && !target.CompareTag("UISlot")) return;
        selected = target;
>>>>>>> Implement limited, click-based UI
        selected.transform.SetParent(selected.GetComponentInParent<Canvas>().transform);
        target.GetComponent<Image>().raycastTarget = false;
    }

    /// <section>Drops the selected item in a UI slot.</section>
    /// <param name="target">Either a UI item or a UI slot to drop the currently selected item into.</param>
    private void DropItem(GameObject target)
    {
        var prevSelected = selected;
        selected.GetComponent<Image>().raycastTarget = true;
<<<<<<< HEAD
        if (target.CompareTag("UIItem") || target.CompareTag("UISlot"))
        {
            var newParent = (target.CompareTag("UIItem")) ? target.transform.parent : target.transform;
            prevSelected.transform.SetParent(newParent);
            SelectItem(target);
            OnItemMoved(prevSelected, parent, newParent);
        }
        else
        {
            OnItemMoved(prevSelected, parent);
            Destroy(prevSelected);
            selected = null;
        }

=======
        if (target.CompareTag("UIItem"))
        {
            prevSelected.transform.SetParent(target.transform.parent);
            SelectItem(target);
        }
        else if (target.CompareTag("UISlot"))
        {
            selected = null;
            prevSelected.transform.SetParent(target.transform);
        }
>>>>>>> Implement limited, click-based UI
        prevSelected.transform.position = prevSelected.transform.parent.transform.position;
    }

    /// <section>Selects an item from or drops an item into a UI slot on left click.</section>
    /// <param name="eventData">Data about the pointer event.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (selected)
            DropItem(target);
        else
            SelectItem(target);
    }
<<<<<<< HEAD

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
}
=======
}
>>>>>>> Implement limited, click-based UI
