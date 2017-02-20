using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CraftingController : MonoBehaviour {

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
            value.transform.SetParent(outputSlot);
        }
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
        if (_inputItems.Any(item => item == null) || _outputItem != null) return;
        var newItem = CraftingRecipes.CraftItem(GetItemName(_inputItems[0]), GetItemName(_inputItems[1]), GetItemName(_inputItems[2]));
        if (newItem == null) return;
        for (var i = 1; i < _inputItems.Length; i++)
            Destroy(_inputItems[i]);
        newItem = Instantiate(newItem);
        _inputItems[0].GetComponent<InventorySlot>().SetItem(newItem.GetComponent<Item>());
        _inputItems[0].GetComponent<Image>().sprite = newItem.GetComponent<SpriteRenderer>().sprite;
        _outputItem = _inputItems[0];
    }

    private string GetItemName(GameObject inputItem)
    {
        var itemComponent = inputItem.GetComponent<InventorySlot>();
        return (itemComponent != null) ? itemComponent.GetItem().prefabName : null;
    }
}
