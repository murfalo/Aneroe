using AneroeInputs;
using UnityEngine;

public class UIController : BaseController
{
    /// <section>The game object representing the user interface.</section>
    [SerializeField] private GameObject UI;

    /// <section>The game object representing the inventory.</section>
    public static GameObject inventory;

    /// <section>The game object representing the main menu.</section>
    public static GameObject mainMenu;

	/// <section>The game object representing the crafting menu.</section>
	public static GameObject crafting;

	GameObject activeMenu;

    /// <section>Load in UI game objects.</section>
	public override void InternalSetup()
    {
        for (int i = 0; i < UI.transform.childCount; i++)
        {
            Transform t = UI.transform.GetChild(i);
			switch (t.name)
			{
				case "Inventory": inventory = t.gameObject; break;
				case "MainMenu": mainMenu = t.gameObject; break;
				case "Crafting": crafting = t.gameObject; break;
			}
        }
    }

    public override void ExternalSetup()
    {
       InputController.iEvent.inputed += new InputEventHandler(ReceiveInput);
    }

	public override void RemoveEventListeners() {
		InputController.iEvent.inputed -= new InputEventHandler(ReceiveInput);
	}

    public void ReceiveInput(object source, InputEventArgs eventArgs)
	{
		if (eventArgs.WasPressed ("inventory")) {
			if (activeMenu == null || activeMenu == inventory)
				ToggleInventory();
		} else if (eventArgs.WasPressed ("mainmenu")) {
			// If possible, deactivate other menu instead of activate main menu
			if (activeMenu != null && activeMenu != mainMenu) {
				ToggleInventory();
			} else {
				mainMenu.SetActive (!mainMenu.activeSelf);
			}
			// Update active menu
			if (mainMenu.activeSelf)
				activeMenu = mainMenu;
			else
				activeMenu = null;
		}

		if (activeMenu) {
			InputController.mode = InputInfo.InputMode.UI;
		} else {
			InputController.mode = InputInfo.InputMode.Free;
		}
    }

	public void ToggleInventory()
	{
        inventory.SetActive(!inventory.activeSelf);
        crafting.SetActive(!crafting.activeSelf);
		activeMenu = (activeMenu != null) ? null : inventory;
	}
}