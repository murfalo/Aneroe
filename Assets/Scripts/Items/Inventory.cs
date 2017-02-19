using System.Collections;
using System.Collections.Generic;
using SaveData;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Wrapper class for a list used to manage the player's inventory.</summary>
public class Inventory
{
    private const int MAX_HOTKEY_ITEMS = 10;

    /// <summary>Maximum possible level for the inventory.</summary>
    private const int MAX_LEVEL = 5;

    /// <summary>
    ///     Number of items in available per level.  Used in calculating the maximum number of items
    ///     that can be stored by the inventory.
    /// </summary>
    private const int LEVEL_ITEMS = 7;

    public const int MAX_POSSIBLE_ITEMS = MAX_LEVEL * LEVEL_ITEMS;

    /// <summary>hotkey items on player</summary>
    public Item[] hotkeyItems;

    /// <summary>List of items in the player's inventory.</summary>
    public List<Item[]> items;

	public int itemSlotEquipped;

    /// <summary>Initializes a new inventory object with a single row of items!</summary>
    public Inventory()
	{
		level = 4;
		itemSlotEquipped = 0;
		items = new List<Item[]>();
		for (var i = 0; i < level; i++)
			items.Add(new Item[LEVEL_ITEMS]);
		hotkeyItems = new Item[10];
    }

    /// <summary>
    ///     Current level of the inventory.  Used in calculating the maximum number of items
    ///     that can be stored by the inventory.
    /// </summary>
    /// <remarks>This variable could also be named numRows as that is what it represents.</remarks>
    private int level { get; set; }

    /// <summary>Calculates the maximum number of items an inventory of a level can hold.</summary>
    /// <param name="invLevel">Level of inventory to calculate the max items for.</param>
    public int maxItems
    {
        get { return level * LEVEL_ITEMS; }
    }

    /// <summary>Calculates the row and column that an item is stored in given an item's index.</summary>
    /// <param name="itemIndex">Index of item in inventory to replace.</param>
    /// <param name="rowIndex">Row index of item in inventory.</param>
    /// <param name="colIndex">Column index of item in inventory.</param>
    private void GetIndices(int itemIndex, out int rowIndex, out int colIndex)
    {
        rowIndex = itemIndex / LEVEL_ITEMS;
        colIndex = itemIndex % LEVEL_ITEMS;
    }

    /// <summary>Retrieves the Item object stored in the inventory's inventory at itemIndex.</summary>
    /// <param name="itemIndex">Index to retrieve item from inventory inventory.</param>
    public Item GetItem(int itemIndex)
    {
        int row, col;
		GetIndices(itemIndex, out row, out col);
        return row < items.Count ? items[row][col] : null;
    }

    /// <summary>Sets an item in the inventory's inventory.</summary>
    /// <param name="itemIndex">Index of item in inventory to replace.</param>
    /// <param name="newItem">Item with which the item at itemIndex will be replaced.</param>
    public void SetItem(int itemIndex, Item newItem)
    {
        int row, col;
        GetIndices(itemIndex, out row, out col);
        if (row < items.Count)
            items[row][col] = newItem;
    }

    /// <summary>Adds an item to the end of the inventory's inventory.</summary>
    /// <param name="newItem">Item to add to the end of the inventory's inventory.</summary>
    public void AddItem(Item newItem)
    {
		for (var j = 0; j < items.Count; j++) {
			for (var i = 0; i < LEVEL_ITEMS; i++) {
				if (items [j] [i] != null)
					continue;
				items [j] [i] = newItem;
				return;
			}
		}
    }

	public int SlotOf(Item item) {
		for (var i = 0; i < maxItems; i++)
			if (GetItem (i) == item)
				return i;
		return -1;
	}

    public bool IsFull()
    {
		return SlotOf(null) == -1;
    }

    /// <summary>Removes an item form the inventory's inventory.</summary>
    /// <param name="itemIndex">Index of the item to be removed.</param>
    public void RemoveItem(int itemIndex)
    {
        int row, col;
        GetIndices(itemIndex, out row, out col);
        if (row >= items.Count) return;
        items[row][col] = null;
    }

    /// <summary>Gets the inventory's current level.</summary>
    public int GetLevel()
    {
        return level;
    }

    /// <summary>Sets the inventory's level, thus increasing its capacity.</summary>
    /// <param name="newLevel">The level to set the inventory to.</param>
    public void SetLevel(int newLevel)
    {
        if (newLevel > 0 && newLevel < MAX_LEVEL)
            level = newLevel;
    }
}