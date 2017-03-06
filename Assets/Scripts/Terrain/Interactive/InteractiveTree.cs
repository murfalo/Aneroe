public class InteractiveTree : TileInteractive {

	new void Awake () {
		base.Awake();
		if (isPast) usableItemTypes = new[] {typeof(Weapon)};
	}
}
