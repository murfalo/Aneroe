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
        public ItemMovedEventArgs(GameObject item, int prevSlot, int newSlot)
        {
            this.item = item;
            this.newSlot = newSlot;
            this.prevSlot = prevSlot;
        }

        public GameObject item { get; }
        public int newSlot { get; }
        public int prevSlot { get; }
    }
}