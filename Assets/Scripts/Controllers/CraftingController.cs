using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (var item in _inputItems)
            Debug.Log(item);
    }
}
