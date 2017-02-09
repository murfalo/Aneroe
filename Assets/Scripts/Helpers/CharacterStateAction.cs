using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateAction : IComparable<CharacterStateAction> {
	public Entity.CharacterState state;

	public CharacterStateAction(Entity.CharacterState s) {
		state = s;
	}

	public int CompareTo(CharacterStateAction other) {
		if (this.state < other.state)
			return -1;
		else if (this.state == other.state)
			return 0;
		return 1;
	}
}
