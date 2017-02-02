using System.Collections.Generic;

/// <summary>Wrapper class for a list used to manage the player's inventory.</summary>
public class Inventory
{

    /// <summary>Initializes a new inventory object!</summary>
    public Inventory()
    {
        level = 1;
        items = new List<Item>();
    }

    /// <summary>List of items in the player's inventory.</summary>
    public List<Item> items;

    /// <summary>
    /// Current level of the inventory.  Used in calculating the maximum number of items
    /// that can be stored by the inventory.
    /// </summary>
    /// <remarks>This variable could also be named numRows as that is what it represents.</remarks>
    private int level { get; set; }

    /// <summary>
    /// Number of items in available per level.  Used in calculating the maximum number of items
    /// that can be stored by the inventory.
    /// </summary>
    private const int LEVEL_ITEMS = 7;

    /// <summary>Calculates the maximum number of items an inventory of a level can hold.</summary>
    /// <param name="invLevel">Level of inventory to calculate the max items for.</param>
    public int maxItems
    {
        get
        {
            return level * LEVEL_ITEMS;
        }
    }

    /// <summary>Retrieves the Item object stored in the inventory's inventory at itemIndex.</summary> 
    /// <param name="itemIndex">Index to retrieve item from inventory inventory.</param>
    public Item GetItem(int itemIndex)
    {
        if (itemIndex > 0 && itemIndex < items.Count)
            return items[itemIndex];
        return null;
    }

    /// <summary>Sets an item in the inventory's inventory.</summary>
    /// <param name="itemIndex">Index of item in backback inventory to replace.</param>
    /// <param name="newItem">Item with which the item at itemIndex will be replaced.</param>
    public void SetItem(int itemIndex, Item newItem)
    {
        if (itemIndex > 0 && itemIndex < items.Count)
            items[itemIndex] = newItem;
    }

    /// <summary>Adds an item to the end of the inventory's inventory.</summary>
    /// <param name="newItem">Item to add to the end of the inventory's inventory.</summary>
    public void AddItem(Item newItem)
    {
        if (items.Count < maxItems)
            items.Add(newItem);
    }

    /// <summary>Removes an item form the inventory's inventory.</summary>
    /// <param name="itemIndex">Index of the item to be removed.</param>
    public void RemoveItem(int itemIndex)
    {
        if (itemIndex > 0 && itemIndex < items.Count)
            items[itemIndex] = null;
    }

    /// <summary>Gets the inventory's current level.</summary>
    public int GetLevel() { return level; }

    /// <summary>Sets the inventory's level, thus increasing its capacity.</summary>
    /// <param name="newLevel">The level to set the inventory to.</param>
    public void SetLevel(int newLevel) { level = newLevel; }
}
