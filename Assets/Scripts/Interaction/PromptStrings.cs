using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PromptStrings {

	public static Dictionary<string, string> prompts = new Dictionary<string, string> () {
		{ "introWasd", "Press W, A, S, and D to move." },
        { "introMouse", "Press Left Click to attack and Right Click to block." },
        { "introRunCrawl", "Hold Left Shift to sprint and Ctrl to sneak."},
        { "introInventoryUsage", "Press E to open the inventory."},
        { "introInventoryControls", "Mouse over an item to learn more information about it.  Press Left Click to pick up an item and Left Click once more to drop it."},
        { "closeInventory", "Notice the crafting menu below. Press E to exit the inventory."},
        { "introEquippedItem", "The yellow outline in the hotbar indicates which slot is equipped.  Press 1-7 to select a slot to equip." },
        { "introPromptSkip", "To skip past this dialogue or any other, press Enter."},
        { "introSwap", "Aged oaks block the way. Press Space to swap timelines!"},
        { "introPast", "Sunlight illuminates a vivid display of springtime to eyes unclouded by age." },
        { "introSaplings", "Fragile young saplings obstruct the path." },
        { "introItemMounds", "Patches of dirt are safe places to store items, whether momentary or for the difficult years to come." },
        { "introItemPickup", "A scroll! Perhaps there is valuable information contained within..." },
	};
}
