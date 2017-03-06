using UnityEngine;
using System.Collections;

public class HealthPotion : Potion {

    public float healAmount; 

	public override bool Use() 
    {
        return PlayerController.activeCharacter.Heal(healAmount);
    }
}
