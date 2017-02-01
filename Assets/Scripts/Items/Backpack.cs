using System.Collections.Generic;

/// <summary>Wrapper class for a list used to manage the player's inventory.</summary>
public class Backpack {
    /// <summary>Initializes a new backpack object!</summary>
    public Backpack()
    {
        inventory = new List<Item>();
    }

    /// <summary>List of items in the player's inventory.</summary>
    private List<Item> inventory;

    /// <summary>
    /// Number of items in a row on screen.  Used in calculating the maximum number of items
    /// that can be stored by the backpack.
    /// </summary>
    private const int ROW_SIZE = 7;

    /// <summary>
    /// Current level of the backpack.  Used in calculating the maximum number of items
    /// that can be stored by the backpack.
    /// </summary>
    /// <remarks>This variable could also be named numRows as that is what it represents.</remarks>
    private int level = 1;

    /// <summary>The maximum number of items that can be stored in this backpack.</summary>
    private int maxItems
    {
        get {
            return ROW_SIZE * level;
        }
    }
    
    /// <summary>Retrieves the Item object stored in the backpack's inventory at itemIndex.</summary> 
    /// <param name="itemIndex">Index to retrieve item from backpack inventory.</param>
    public Item GetItem(int itemIndex)
    {
        if (itemIndex > 0 && itemIndex < inventory.Count)
            return inventory[itemIndex];
        return null;
    }

    /// <summary>Sets an item in the backpack's inventory.</summary>
    /// <param name="itemIndex">Index of item in backback inventory to replace.</param>
    /// <param name="newItem">Item with which the item at itemIndex will be replaced.</param>
    public void SetItem(int itemIndex, Item newItem)
    {
        if (itemIndex > 0 && itemIndex < inventory.Count)
            inventory[itemIndex] = newItem;
    }

    /// <summary>Adds an item to the end of the backpack's inventory.</summary>
    /// <param name="newItem">Item to add to the end of the backpack's inventory.</summary>
    public void AddItem(Item newItem)
    {
        if (inventory.Count < maxItems)
            inventory.Add(newItem);
    }

    /// <summary>Removes an item form the backpack's inventory.</summary>
    /// <param name="itemIndex">Index of the item to be removed.</param>
    public void RemoveItem(int itemIndex)
    {
        if (itemIndex > 0 && itemIndex < inventory.Count)
            inventory[itemIndex] = null;
    }

    /// <summary>Gets the backpack's current level.</summary>
    public int GetLevel() { return level; }

    /// <summary>Sets the backpack's level, thus increasing its capacity.</summary>
    /// <param name="newLevel">The level to set the backpack to.</param>
    public void SetLevel(int newLevel) { level = newLevel; }
}
