using UnityEngine;
using System.Collections;

public class Weapon : Item {

	public static float MAX_DURABILITY;

	// Properties
	public float speed = 1f;
	public float attack = 1f;
	public float defense = 1f;
	float durability;

	void Awake () {
		durability = MAX_DURABILITY;
	}
}
