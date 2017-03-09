using UnityEngine;
using System.Collections;

public class PoisonAroma : MonoBehaviour
{
	float timer = 0;
	const float BUFFER = 1;
	bool damaged;

	void Update() {
		if (damaged) {
			timer += Time.deltaTime;
			if (timer > BUFFER) {
				timer = 0;
				damaged = false;
			}
		}
	}

	void OnTriggerStay2D(Collider2D col) {
		if (damaged)
			return;
		BossEntity boss = col.GetComponentInParent<BossEntity> ();
		if (boss != null) {
			boss.Damage (10, 1, .1f, 0f);
			damaged = true;
		}
	}
}

