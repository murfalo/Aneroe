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

	GameObject activeMenu;

    /// <section>Load in UI game objects.</section>
	public override void InternalSetup()
    {
        for (int i = 0; i < UI.transform.childCount; i++)
        {
            Transform t = UI.transform.GetChild(i);
            if (t.name.Equals("Inventory"))
                inventory = t.gameObject;
            else if (t.name.Equals("MainMenu"))
                mainMenu = t.gameObject;
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
			inventory.SetActive (!inventory.activeSelf);
			if (inventory.activeSelf)
				activeMenu = inventory;
			else
				activeMenu = null;
		} else if (eventArgs.WasPressed ("mainmenu")) {
			if (activeMenu != null && activeMenu != mainMenu) {
				activeMenu.SetActive (false);
				activeMenu = null;
			} else {
				mainMenu.SetActive (!mainMenu.activeSelf);
			}
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
}