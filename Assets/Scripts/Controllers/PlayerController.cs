using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using AneroeInputs;
using UnityEngine.UI;

public class PlayerController : EntityController
{

    public List<GameObject> characterPrefabs;
    public static PlayerEntity activeCharacter;

    PlayerEntity[] characters;
    int characterIndex;
    string[] directions;

	public override void InternalSetup()
    {
        actionResponses = new Dictionary<string, System.Action<Entity>>() {
            {"die",RestartGame}
        };
        GameObject obj = new GameObject();
        obj.name = "PlayerHolder";
        characters = new PlayerEntity[characterPrefabs.Count];
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            characters[i] = ((GameObject)GameObject.Instantiate(characterPrefabs[i], obj.transform)).GetComponent<PlayerEntity>();
            characters[i].Setup();
        }
        characterIndex = 0;
        activeCharacter = characters[0];
        directions = new string[4] { "up", "right", "down", "left" };
    }

    public override void ExternalSetup()
    {
        InputController.iEvent.inputed += new InputEventHandler(ReceiveInput);
		SaveController.playerLoaded += Load;
		SaveController.playerSaving += Save;
    }

    void FixedUpdate()
    {
        if (activeCharacter != null)
            activeCharacter.DoFixedUpdate();
    }

    public void ReceiveInput(object sender, InputEventArgs e)
    {
        // Inputs prioritized as such (by order of check):
        // Attacking, Walking, Switching character
        activeCharacter.Quicken(e.IsHeld("quicken"));

        // See if a direction was input and log it
        bool dirChosen = false;
        bool[] dirActive = new bool[4];
        bool[] dirTapped = new bool[4];
        for (int i = 0; i < directions.Length; i++)
        {
            dirTapped[i] = e.WasPressed(directions[i]);
            dirActive[i] = dirTapped[i] || e.IsHeld(directions[i]);
            dirChosen = dirChosen || dirActive[i] || dirTapped[i];
        }
        activeCharacter.SetDirections(dirActive, dirTapped);

        if (e.IsHeld("attack"))
        {
            activeCharacter.TryAttacking();
        }
        else if (e.IsHeld("defend"))
        {
            activeCharacter.TryBlocking();
        }
        else if (e.WasPressed("interact"))
        {
            activeCharacter.TryInteracting();
        }
        else if (e.WasPressed("switch character") && activeCharacter.CanSwitchFrom())
        {
            characterIndex = (characterIndex + 1) % characters.Length;
            activeCharacter = characters[characterIndex];
            GameObject.Find("Control").GetComponent<SceneController>().ChangeActiveCharacter();
        }
        else if (dirChosen)
        {
            activeCharacter.TryWalk();
        }
    }

    void RestartGame(Entity e)
    {
        GameObject.Find("Control").GetComponent<SceneController>().ReloadBaseScene();
    }

	public void Load(object sender, System.EventArgs e) {
		for (int i = 0; i < characters.Length; i++) {
			PlayerEntity player = characters [i];
			EntitySaveData esd;
			SaveController.GetValue (SaveKeys.players [i], out esd);
			player.transform.position = new Vector3(esd.posX, esd.posY, 0);
			player.stats = new StatInfo(esd.statLevels);
		}
	}

	public void Save(object sender, System.EventArgs e) {
		for (int i = 0; i < characters.Length; i++) {
			PlayerEntity player = characters [i];
			EntitySaveData esd = new EntitySaveData ();
			esd.posX = player.transform.position.x;
			esd.posY = player.transform.position.y;
			esd.statLevels = player.stats.GetStats();
			SaveController.SetValue (SaveKeys.players [i], esd);
		}
	}
}
