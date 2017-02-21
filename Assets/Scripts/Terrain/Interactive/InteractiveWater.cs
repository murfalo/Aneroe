public class InteractiveWater : TileInteractive {

	new void Start () {
		base.Start();
		usableItemPrefabNames = new[] {"IcePotion"};
	}
}
