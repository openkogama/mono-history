using System.Collections;

public class MaterialDescription
{
	public enum MaterialSpecialProperty
	{
		Burning,
		Poisonous,
		Slippery,
		Bouncy,
		Destructable,
		Size
	}

	public static readonly MaterialDescription[] materialDescriptions = new MaterialDescription[60]
	{
		new MaterialDescription(TM._("Bright Red"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Red"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Dark Red"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Sand"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Light Purple Fabric"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Bright Blue"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Blue"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Dark Blue"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Caramel"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Purple Fabric"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Bright Green"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Green"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Dark Green"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Ceramic"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Dark Purple Fabric"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Yellow"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Bright Orange"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Orange"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Butter"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Sandstone"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Light Concrete"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Concrete"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Dark Concrete"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Black Concrete"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Khaki"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Ice"), TM._("The ice material is slippery and super fun. Players can use it for ice slides, parkour and crazy roller coasters."), MaterialSpecialProperty.Slippery),
		new MaterialDescription(TM._("Lava"), TM._("The lava material is very hot and will set any player on fire that touches it. This Material can be used for traps, parkour and deadly obstacles. It also glows in the dark."), default(MaterialSpecialProperty)),
		new MaterialDescription(TM._("Bouncy"), TM._("Jumping on the bouncy material will launch the player up in the sky. This is great for creating crazy platform games!"), MaterialSpecialProperty.Bouncy),
		new MaterialDescription(TM._("Poison"), TM._("When a player has touched this material he is poisoned! He will die unless he picks up a health pack immediately! This material glows in the dark making it deadly and beautiful."), MaterialSpecialProperty.Poisonous),
		new MaterialDescription(TM._("Parkour"), TM._("This material allows players to wall jump. Create games with this material if you want the players to do skillbased parkour moves.")),
		new MaterialDescription(TM._("Bricks"), TM._("The brick material is slightly rough which makes players slide less when walking on it.")),
		new MaterialDescription(TM._("Bright Wood"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Cobblestone"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Cement"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Camouflage"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Green Pavement"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Ancient Cobblestone"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Red Bricks"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Yellow Bricks"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Zigzag"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Metal Pattern"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Metal"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Mushroom"), TM._("This material is slightly slippery AND a little bouncy. This can be used to create challenging obstacles."), MaterialSpecialProperty.Bouncy),
		new MaterialDescription(TM._("Black Ice"), TM._("The Black Ice material is slippery and super fun. Players can use it for ice slides, parkour and crazy roller coasters."), MaterialSpecialProperty.Slippery),
		new MaterialDescription(TM._("Pink Fabric"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Red Grid"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Green Grid"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Circuit"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Grey Bricks"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Spotty"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Metal Scraps"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Slime"), TM._("Yuck. Slime! This material has a slippery surface and isn't easy to run on."), MaterialSpecialProperty.Bouncy, MaterialSpecialProperty.Slippery),
		new MaterialDescription(TM._("Wrapping Paper"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Dark Wood"), TM._("This standard material can be used to create games, cube models and avatars.")),
		new MaterialDescription(TM._("Super Bouncy"), TM._("This material is even more bouncy than the normal bouncy material! Be careful when you jump on this because you might not come back down!")),
		new MaterialDescription(TM._("Cloud"), TM._("This material has a bouncy surface. It also glows in the dark! This can be used for really cool sky effects.")),
		new MaterialDescription(TM._("Soft Destructible"), TM._("This material can easily be destroyed by weapons, explosives and when it's hit by players or vehicles. You can hide treasures for the players with this."), MaterialSpecialProperty.Destructable),
		new MaterialDescription(TM._("Medium Destructible"), TM._("This material can be destroyed by weapons, explosives and when it's hit by players or vehicles. Build a castle with this... And DESTROY it!"), MaterialSpecialProperty.Destructable),
		new MaterialDescription(TM._("Hard Destructible"), TM._("This material can be destroyed by weapons, explosives and when it's hit by players or vehicles. To destroy this takes real work. Use it to create robust yet destructible walls."), MaterialSpecialProperty.Destructable),
		new MaterialDescription(TM._("Cracked Ice"), TM._("Both destructible and very slippery this material is great for building ice castles!"), MaterialSpecialProperty.Destructable, MaterialSpecialProperty.Slippery)
	};

	private BitArray specialProperties = new BitArray(5);

	public string Name { get; private set; }

	public string Description { get; private set; }

	public BitArray SpecialProperties
	{
		get
		{
			return specialProperties;
		}
		private set
		{
			specialProperties = value;
		}
	}

	public MaterialDescription(string name, string description, params MaterialSpecialProperty[] specialProperties)
	{
		Name = name;
		Description = description;
		for (int i = 0; i < specialProperties.Length; i++)
		{
			this.specialProperties.Set((int)specialProperties[i], value: true);
		}
	}
}
