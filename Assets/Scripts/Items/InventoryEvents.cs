using System;
using UnityEngine;

/// <section>
/// This namespace defines the event arguments for all events relating to
/// changes in the player's inventory.
/// </section>
namespace InventoryEvents
{
    /// <section>Information used when an item moves in the inventory.</section>
    public class ItemMovedEventArgs : EventArgs
    {
        /// <section>Creates a new instance of ItemMovedEventArgs.</section>
        /// <param name="item">The item being moved.</param>
        /// <param name="prevSlot">The identifier of item's old slot.</param>
        /// <param name="newSlot">The identifier of item's new slot.</param>
        public ItemMovedEventArgs(Item item, int prevSlot, int? newSlot = null)
        {
            this.item = item;
            this.prevSlot = prevSlot;
            this.newSlot = (newSlot != null && newSlot < 0) ? null : newSlot;
        }

        public Item item { get; }
        public int prevSlot { get; }
        public int? newSlot { get; }
    }
}