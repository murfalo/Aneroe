using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PromptStrings {

	public static Dictionary<string, string> prompts = new Dictionary<string, string> () {
		// For prologue
		{ "almostThere", "It can't be much farther." },
		{ "curseTheDisease", "Curse this poison. If only I had a little more time." },
		// For opening scene 2_20
		{ "introWasd", "Press W, A, S, and D to move." },
        { "introMouse", "Press Left Click to use an item and Right Click for secondary use." },
		{ "introSword", "You have a sword equiped. The primary use of the sword attacks and the secondary use is Block."},
        { "introRunCrawl", "Hold Left Shift to sprint and Ctrl to sneak."},
        { "introInventoryUsage", "Press E to open the inventory."},
        { "introInventoryControls", "Mouse over an item to learn more information about it.  Press Left Click to pick up an item and Left Click once more to drop it."},
        { "closeInventory", "Notice the crafting menu below. Press E to exit the inventory."},
        { "introEquippedItem", "The yellow outline in the hotbar indicates which slot is equipped.  Press 1-7 to select a slot to equip." },
        { "introPromptSkip", "To skip past this dialogue or any other, press Enter."},
        { "introSwap", "Aged oaks block the way. Press Space to swap timelines!"},
        { "introPast", "Sunlight illuminates a vivid display of springtime to eyes unclouded by age." },
        { "introSaplings", "Fragile young saplings obstruct the path." },
		{ "introItemMounds", "Patches of dirt are safe places to bury items. Use an item on the dirt to bury it."},
		{ "introBuryItem", "Bury the flask for your future self." },
		{ "introPickupItem","Swap to the present timeline to retrieve the item."},
        { "introItemPickup", "Perhaps there is valuable information contained within the scroll..." },
		{ "introReset","If you eveer get stuck, press the escape button to reset to the last checkpoint."},
		{ "crossWater", "There looks to be some valuables across the water. I wonder if there's a way to cross..."},
	};
}
