using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UIEvents;

public class CraftingController : BaseController {

    /// <summary>Accessors for items in the crafting input bar.</summary>
    private GameObject[] _inputItems
    {
        get
        {
            var inputBar = UIController.Crafting.transform.GetChild(0);
            var inputItems = new GameObject[inputBar.childCount];
            for (int i = 0; i < inputBar.childCount; i++)
                if (inputBar.GetChild(i).childCount > 0)
                    inputItems[i] = inputBar.GetChild(i).GetChild(0).gameObject;
            return inputItems;
        }
    }
		
    /// <summary> Setter for the crafting output slot.</summary>
    private GameObject _outputItem
    {
        get
        {
            var outputSlot = UIController.Crafting.transform.GetChild(2).GetChild(0);
            return (outputSlot.childCount > 0) ? outputSlot.GetChild(0).gameObject : null;
        }
        set
        {
            var outputSlot = UIController.Crafting.transform.GetChild(2).GetChild(0);
            if (outputSlot.childCount > 0)
                Destroy(outputSlot.GetChild(0).gameObject);
            value.transform.SetParent(outputSlot, false);
        }
    }

	private GameObject _craftButton;

	public override void InternalSetup() {
		_craftButton = UIController.Crafting.transform.GetChild(1).GetChild(0).gameObject;
	}

	void Update() {
		CheckIfCraftable ();
	}

    /// <summary>Drops all items in crafting slots.</summary>
    public void DropItems()
    {
        foreach (var item in _inputItems)
            UIController.DropItem(item);
        UIController.DropItem(_outputItem);
    }

    /// <summary>Crafts a single item using the input items and places it in the output slot.</summary>
    public void CraftItem()
    {
        //if (_inputItems.Any(item => item == null) || _outputItem != null) return;
		if (_outputItem != null) return;
		var newItem = CraftingRecipes.CraftItem(GetItemName(_inputItems[0]), GetItemName(_inputItems[1]), GetItemName(_inputItems[2]));
        if (newItem == null) return;

		bool reuse = true;
		for (var i = 0; i < _inputItems.Length; i++) {
			if (!reuse)
				Destroy (_inputItems [i]);
			else if (_inputItems[i] != null) {
				_inputItems [i].GetComponent<InventorySlot> ().SetUnsetItem (newItem.GetComponent<Item> ());
				_outputItem = _inputItems [i];
				reuse = false;
			}
		}
    }

    private string GetItemName(GameObject inputItem)
    {
		if (inputItem == null)
			return "";
        var itemComponent = inputItem.GetComponent<InventorySlot>();
        return (itemComponent != null) ? itemComponent.GetItem().prefabName : null;
    }

	public void CheckIfCraftable() {
		if (CraftingRecipes.CanCraft (GetItemName (_inputItems [0]), GetItemName (_inputItems [1]), GetItemName (_inputItems [2]))) {
			_craftButton.GetComponent<Image> ().CrossFadeAlpha (1, 0, true);
		} else {
			_craftButton.GetComponent<Image> ().CrossFadeAlpha (.25f, 0, true);
		}
	}
}
