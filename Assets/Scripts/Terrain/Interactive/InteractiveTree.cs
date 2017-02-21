public class InteractiveTree : TileInteractive {

	new void Start () {
		base.Start();
		if (isPast) usableItemTypes = new[] {typeof(Weapon)};
	}
}
