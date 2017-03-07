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
					new[] {"takeSteps","1","3"}, // number of steps, direction
					new[] {"wound","2"},
					new[] {"wait",".3"},
					new[] {"takeSteps","1","2"},
					new[] {"wound","2"},
					new[] {"wait",".3"},
					new[] {"takeSteps","1","2"},
					new[] {"wound","2"},
					new[] {"wait",".5"},
					new[] {"text",  "Curse this poison. If only I had a little more time."},
					//new[] {"overlapNextCommands"}, // Counterpart is new[] {"waitOnOverlapped"}, placed after acts that are simultaneous
					new[] {"fade", "out", ".5"},
					//new[] {"takeSteps","1","2"},
					//new[] {"wait",".3"},
					new[] {"loadScene","OpeningScene2_20"},
				}
			},
			{"stillAlive",
				new List<string[]> {
					new [] {"text","Sunlight illuminates a vivid display of springtime to eyes unclouded by age.","???"},
					new [] {"text","You have traveled back in time many years. The world is similar, but several things have not yet changed over the years.","???"},
					new [] {"turn","2"},
					new [] {"wait",".1"},
					new [] {"turn","4"},
					new [] {"wait",".1"},
					new [] {"turn","3"},
					new [] {"text","I'm alive!? This can't be."},
					new [] {"text","And I'm in the past. I even have my dagger!"},
					new [] {"text","I don't feel fully integrated into this timeframe though"},
					new [] {"wait",".5"},
					new [] {"timeSwap"},
					new [] {"wait",".5"},
					new [] {"turn","4"},
					new [] {"wait",".1"},
					new [] {"turn","2"},
					new [] {"wait",".1"},
					new [] {"turn","3"},
					new [] {"text","This is after the failed expedition."},
					new [] {"trackEntity", "true"},
					new [] {"takeSteps","2","1"},
					new [] {"text","I only gained access to these two timeframes, but it'll have to do."},
				}
			},
			{"berryBushSpotted",
				new List<string[]> {
					new [] {"camTo","marker1"},
					new [] {"text","Ice berries! I haven't seen those in a long time. Well, I have yet to not see them for a while, so?"},
					new [] {"camTo","marker2"},
					new [] {"text","I'm less excited to see that."},
				}
			},
			{"waterSpotted",
				new List<string[]> {
					new [] {"camTo","marker1"},
					new [] {"text","Wouldn't it be lovely if one could freeze that water and pass over it?","???"},
				}
			},
		};
	}
}

