using System;
using UnityEngine;

namespace UIEvents
{
    /// <summary>Information used when an item is selected in the UI.</summary>
    public class ItemSelectedEventArgs : EventArgs
    {
        /// <param name="oldSelected">The previously selected GameObject.</param>
        /// <param name="newSelected">The newly selected GameObject.</param>
        public ItemSelectedEventArgs(GameObject oldSelected, GameObject newSelected)
        {
            this.oldSelected = oldSelected;
            this.newSelected = newSelected;
        }

        public GameObject oldSelected;
        public GameObject newSelected;
    }

    /// <summary>Information used when an item moves in the inventory.</summary>
    public class ItemMovedEventArgs : EventArgs
    {
        /// <summary>Creates a new instance of ItemMovedEventArgs.</summary>
        /// <param name="item">The item being moved.</param>
        /// <param name="prevSlot">The identifier of item's old slot.</param>
        /// <param name="newSlot">The identifier of item's new slot.</param>
        public ItemMovedEventArgs(Item item, int prevSlot, int? newSlot = null)
        {
            this.item = item;
            this.prevSlot = prevSlot;
            this.newSlot = (newSlot != null && newSlot < 0) ? null : newSlot;
        }

		public Item item;
		public int prevSlot;
		public int? newSlot;
    }

    public class ItemInteractEventArgs : EventArgs
    {
        public ItemInteractEventArgs(Item i, Inventory inv, bool added, int oldIndex = -1)
        {
            this.item = i;
            this.inventory = inv;
            this.addedToInv = added;
            this.oldIndex = oldIndex;
        }

        public Item item;
        public Inventory inventory;
        public bool addedToInv;
        public int oldIndex;
    }
}