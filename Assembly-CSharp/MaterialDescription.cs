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
		new MaterialDescription(TM._("Bright Red"), TM._("This is a bright red material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Red"), TM._("This is a red material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Dark Red"), TM._("This is a dark red material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Sand"), TM._("This is a sand material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Light Purple Fabric"), TM._("This is a light purple material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Bright Blue"), TM._("This is an bright blue material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Blue"), TM._("This is a blue material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Dark Blue"), TM._("This is a dark blue material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Caramel"), TM._("This is a caramel material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Purple Fabric"), TM._("This is a purple material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Bright Green"), TM._("This is a bright green material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Green"), TM._("This is a green material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Dark Green"), TM._("This is a dark green material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Ceramic"), TM._("This is a ceramic material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Dark Purple Fabric"), TM._("This is a dark gravel material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Yellow"), TM._("This is a yellow material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Bright Orange"), TM._("This is a bright orange material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Orange"), TM._("This is an orange material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Butter"), TM._("This is a butter material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Sandstone"), TM._("This is a sandstone material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Light Concrete"), TM._("This is a bright concrete material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Concrete"), TM._("This is a concrete material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Dark Concrete"), TM._("This is a dark concrete material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Black Concrete"), TM._("This is a black concrete material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Khaki"), TM._("This is a khaki material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Ice"), TM._("This is an ice material. This material is slippery and isn't easy to run on. Players can use it for ice slides, parkour, or even cool effects."), MaterialSpecialProperty.Slippery),
		new MaterialDescription(TM._("Lava"), TM._("This is a lava material. This material will slowly burn any player that comes in contact with it. This Material can be used for traps, parkour, or even cool effects. It is not affected by light."), default(MaterialSpecialProperty)),
		new MaterialDescription(TM._("Bouncy"), TM._("This is a bouncy material. This material will launch any player that jumps on it high into the air."), MaterialSpecialProperty.Bouncy),
		new MaterialDescription(TM._("Poison"), TM._("This is a poison material. This material will slowly kill any player that comes in contact with it. Players can use it for traps, parkour, or even cool effects. It is not affected by light."), MaterialSpecialProperty.Poisonous),
		new MaterialDescription(TM._("Parkour"), TM._("This is a parkour material. This material allows players to jump from wall to wall. Players can use this material for epic parkour games, and even cool effects.")),
		new MaterialDescription(TM._("Bricks"), TM._("This is a brick material. This material can be used to create games and cool visual effects. Makes you slide less when walking on it.")),
		new MaterialDescription(TM._("Bright Wood"), TM._("This is a bright wood material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Cobblestone"), TM._("This is a cobblestone material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Concrete"), TM._("This is a concrete material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Camouflage"), TM._("This is a military camouflage material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Green Pavement"), TM._("This is a green pavement material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Ancient Cobblestone"), TM._("This is an ancient cobblestone material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Red Bricks"), TM._("This is a red brick material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Yellow Bricks"), TM._("This is a yellow brick material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Zig-zag"), TM._("This is a zig-zag striped material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Metal Pattern"), TM._("This is a metal pattern material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Metal"), TM._("This is a metal material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Mushroom"), TM._("This is a mushroom material. This material will launch any player that jumps on it into the air. Players can use this material for parkour or even cool effects."), MaterialSpecialProperty.Bouncy),
		new MaterialDescription(TM._("Dark Ice"), TM._("This is a dark ice material. This material is slippery and isn't easy to run on. Players can use it for ice slides, parkour, or even cool effects"), MaterialSpecialProperty.Slippery),
		new MaterialDescription(TM._("Pink Fabric"), TM._("This is a pink fabric material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Red Grid"), TM._("This is a red grid material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Green Grid"), TM._("This is a green grid material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Circuit"), TM._("This is a circuit material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Grey Bricks"), TM._("This is a grey brick material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Spotty"), TM._("This is a spotty material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Metal Scraps"), TM._("This is a metal scraps material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Slime"), TM._("This is a slime material. This material has a slippery surface and isn't easy to run on. Players can use this material for parkour, or even cool effects."), MaterialSpecialProperty.Bouncy, MaterialSpecialProperty.Slippery),
		new MaterialDescription(TM._("Wrapping Paper"), TM._("This is a wrapping paper material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Dark Wood"), TM._("This is a dark wood material. This material can be used to create games and cool visual effects.")),
		new MaterialDescription(TM._("Super Bouncy"), TM._("This is a super bouncy material. This material will launch any player that jumps on it extremely high into the air.")),
		new MaterialDescription(TM._("Cloud"), TM._("This is a cloud material. This material has a bouncy surface. Players can use this material for clouds, snow, or even cool effects. It is not affected by light.")),
		new MaterialDescription(TM._("Soft Destructible"), TM._("This is a soft destructible material. This material can easily be destroyed by weapons, explosives and when it's hit by players or vehicles. Players can use this material for parkour, secret rooms, secret tunnels and more!"), MaterialSpecialProperty.Destructable),
		new MaterialDescription(TM._("Medium Destructible"), TM._("This is a medium destructible material. This material can be destroyed by weapons, explosives and when it's hit by players or vehicles. Players can use this material for parkour, secret rooms, secret tunnels and more!"), MaterialSpecialProperty.Destructable),
		new MaterialDescription(TM._("Hard Destructible"), TM._("This is a hard destructible material. This material can be destroyed by weapons, explosives and when it's hit by players or vehicles. But it's not easy. Players can use this material for parkour, secret rooms, secret tunnels and more!"), MaterialSpecialProperty.Destructable),
		new MaterialDescription(TM._("Cracked Ice"), TM._("This is a destructible, cracked ice material. This material has a slippery surface and can be destroyed by weapons, explosives and when it's hit by players and vehicles. Players can use this material forice slides, parkour, cool games, secret rooms and cool effects."), MaterialSpecialProperty.Destructable, MaterialSpecialProperty.Slippery)
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
