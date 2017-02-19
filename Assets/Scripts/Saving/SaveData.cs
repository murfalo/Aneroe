using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveData 
{

	[System.Serializable]
	public struct EntitySaveData {
		public int entityID;
		public float posX;
		public float posY;
		public Dictionary<string, float> statLevels;
		public InvSaveData inv;
	}

	[System.Serializable]
	public class ItemSaveData {
		// Only used to find prefab to spawn item gameobject with
		public int itemID;
		public string prefabName;
		public int count;
		public bool smallItem;

		public override string ToString ()
		{
			return prefabName + "  " + count + "  " + smallItem;
		}
	}

	[System.Serializable]
	public class WeaponSaveData : ItemSaveData {
		public Dictionary<string, float> statLevels;

		public override string ToString ()
		{
			if (statLevels != null)
				return prefabName + "  " + count + "  " + smallItem + "  " + statLevels.Count;
			return prefabName + "  " + count + "  " + smallItem + "  " + 0;
		}
	}

	[System.Serializable]
	public struct InvSaveData {
		public Hashtable hotkeyItems;
		public int level;
		// 1 dimensional flattened array 
		// (i*LEVEL_ITEMS + j) for i:level 
		public Hashtable items;
		public int itemSlotsUsed;
	}
}
