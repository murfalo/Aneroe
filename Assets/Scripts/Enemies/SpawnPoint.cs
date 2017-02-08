using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
	// indices into enemy Entity array stored in SpawnerInfo of parent object
	public int[] enemyIDs;

	// offsets from transform of this object indicating where to spawn Entity of id from enemyIDs array
	public Vector2[] spawnOffsets;
}

