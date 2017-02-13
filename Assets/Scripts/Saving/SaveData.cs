using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveData 
{

	[System.Serializable]
	public struct EntitySaveData {
		public float posX;
		public float posY;
		public Dictionary<string, float> statLevels;
		public InvSaveData inv;
	}

	[System.Serializable]
	public class ItemSaveData {
		// Only used to find prefab to spawn item gameobject with
		public string prefabName;
		public int count;
		public bool smallItem;
	}

	[System.Serializable]
	public class WeaponSaveData : ItemSaveData {
		public Dictionary<string, float> statLevels;
	}

	[System.Serializable]
	public struct InvSaveData {
		public ItemSaveData[] hotkeyItems;
		public int level;
		// 1 dimensional flattened array 
		// (i*LEVEL_ITEMS + j) for i:level 
		public ItemSaveData[] items;
		public int itemSlotsUsed;
	}
}
