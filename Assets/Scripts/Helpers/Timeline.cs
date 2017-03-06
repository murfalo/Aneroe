using UnityEngine;
using System.Collections;

namespace Timelines {
	public enum TimelinePeriod {
		PrePast, // Just before the 1st main timeline
		Past, // 1st main timeline in game
		Present, // 2nd main timeline in game
		PostPresent, // Just after the 2nd main timeline
		Future // When you are just about to die
	}
}