using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cutscenes 
{
	public static class CutsceneStrings
	{
		public static Dictionary<string, List<string[]>> scenes = new Dictionary<string, List<string[]>>() 
		{
			{"prologue", 
				new List<string[]> {
					new[] {"text", "It can't be much farther."},
					new[] {"moveToMarker", "0","Marker1"},
					new[] {"text",  "Curse this poison. If only I had a little more time."},
					new[] {"fade", "out", "1"},
					new[] {"loadScene","OpeningScene2_20"},
				}
			},
		};
	}
}

