using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class manages 
/// </summary>
public class InventoryController : MonoBehaviour, IPointerClickHandler
{

    /// <section>Prefab for an inventory slot.</section>
    [SerializeField] GameObject UISlot;

    /// <section>Item currently selected by the player.</section>
    private GameObject selected { get; set; }

    /// <section>Original parent of the currently selected item.</section>
    private Transform parent { get; set; }

    /// <section>Initializes the inventory to the size of the currently active character.</section>
    public void Start()
    {
        for (int i = 0; i < PlayerController.activeCharacter.inv.maxItems; i++)
        {
            var newSlot = (GameObject)Instantiate(UISlot);
            if (i % 2 == 0)
                Destroy(newSlot.transform.GetChild(0).gameObject);
            newSlot.name = "Slot." + i.ToString();
            newSlot.transform.SetParent(transform.GetChild(0).transform);
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
        if (!target.CompareTag("UIItem")) return;
        selected = target;
        parent = selected.transform.parent;
        selected.transform.SetParent(selected.GetComponentInParent<Canvas>().transform);
        target.GetComponent<Image>().raycastTarget = false;
    }

    /// <section>Drops the selected item in a UI slot.</section>
    /// <param name="target">Either a UI item or a UI slot to drop the currently selected item into.</param>
    private void DropItem(GameObject target)
    {
        var prevSelected = selected;
        selected.GetComponent<Image>().raycastTarget = true;
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
        else
        {
            prevSelected.transform.SetParent(parent);
            selected = null;
        }

        Debug.Log(target);

        prevSelected.transform.position = prevSelected.transform.parent.transform.position;
    }

    /// <section>Selects an item from or drops an item into a UI slot on left click.</section>
    /// <param name="eventData">Data about the pointer event.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        var target = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log(selected);
        if (selected)
            DropItem(target);
        else
            SelectItem(target);
    }
}
