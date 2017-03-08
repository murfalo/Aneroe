using UnityEngine;
using System.Collections;

public class PoisonAroma : MonoBehaviour
{
	void OnTriggerStay2D(Collider2D col) {
		BossEntity boss = col.GetComponentInParent<BossEntity> ();
		if (boss != null) {
			boss.Damage (10, 1, .3f, 0f);
		}
	}
}

