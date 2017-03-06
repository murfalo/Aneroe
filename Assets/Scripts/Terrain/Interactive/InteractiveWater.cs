public class InteractiveWater : TileInteractive {

	new void Awake() {
		base.Awake();
		usableItemPrefabNames = new[] {"IcePotion"};
	}
}
