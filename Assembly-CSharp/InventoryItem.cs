using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
	public class ItemDescription
	{
		private string name;

		private string explanation;

		private string inspirational;

		private Vector3 cameraPreviewerOffset = new Vector3(0f, 0f, 0f);

		public Vector3 CameraPreviewerOffset => cameraPreviewerOffset;

		public string Name => name;

		public string Description => explanation + "\n\n" + inspirational;

		public ItemDescription(string name, string explanation, string inspirational)
		{
			this.name = name;
			this.explanation = explanation;
			this.inspirational = inspirational;
		}

		public ItemDescription(string name, string explanation, string inspirational, Vector3 cameraPreviewerOffset)
		{
			this.name = name;
			this.explanation = explanation;
			this.inspirational = inspirational;
			this.cameraPreviewerOffset = cameraPreviewerOffset;
		}
	}

	public readonly int itemID;

	public readonly int itemCategoryID;

	public readonly int itemTypeID;

	public readonly byte[] data;

	public readonly bool resellable;

	public readonly int priceGold;

	public readonly bool purchased;

	public readonly int authorProfileID;

	public readonly int originalItemID;

	public readonly bool isDeleted;

	public readonly bool isDefaultInvItem;

	public int shopInventoryID;

	public string name;

	public string description;

	public int slotPosition;

	public static readonly Dictionary<MVWorldObjectDocumentationType, ItemDescription> localItemDescriptionOverride = new Dictionary<MVWorldObjectDocumentationType, ItemDescription>
	{
		{
			MVWorldObjectDocumentationType.HealthPack,
			new ItemDescription(TM._("Health pack"), TM._("The health pack fully heals a player."), TM._("Place this after difficult parts of your game to refill players health."), new Vector3(0f, 0f, 0f))
		},
		{
			MVWorldObjectDocumentationType.Centergun,
			new ItemDescription(TM._("Machine gun"), TM._("A fully automatic rifle capable of firing a large amount of bullets towards the target."), TM._("This rifle is at its best in close to mid-range combat."))
		},
		{
			MVWorldObjectDocumentationType.ImpulseGun,
			new ItemDescription(TM._("Impulse gun"), TM._("A gun generating a powerful force wave, pushing anything or anyone away."), TM._("This gun has a very high recoil. By shooting straight down into the ground you can do super jumps with it."))
		},
		{
			MVWorldObjectDocumentationType.Bazooka,
			new ItemDescription(TM._("Bazooka"), TM._("A rocket launcher with devastating firing power."), TM._("The bazooka is incredibly powerful when shot towards groups of players."))
		},
		{
			MVWorldObjectDocumentationType.Railgun,
			new ItemDescription(TM._("Rail gun"), TM._("The sniper rifle uses a charged coil to fire metal slugs surpassing the speed of sound."), TM._("Devastating when used at long range. You need to fully charge your shot to fire!"))
		},
		{
			MVWorldObjectDocumentationType.Sword,
			new ItemDescription(TM._("Sword"), TM._("A steel blade designed for melee combat."), TM._("Sneak up on your enemies to catch them off guard. Don't bring a sword to a gun fight!"))
		},
		{
			MVWorldObjectDocumentationType.Mutant,
			new ItemDescription(TM._("Mutanto"), TM._("Infuses the player with mutagenic powers. The player will be invincible, super fast, and kill anything it touches."), TM._("Become a super hero, or super villain, by turning into a mutagenic freak, capable of decimating any enemy you touch!"))
		},
		{
			MVWorldObjectDocumentationType.Flamethrower,
			new ItemDescription(TM._("Flamethrower"), TM._("A classic! Burn everything within reach."), TM._("Light your enemies on fire with this close range weapon."))
		},
		{
			MVWorldObjectDocumentationType.Shotgun,
			new ItemDescription(TM._("Shotgun"), TM._("A gun modeled after the classic hunting rifle, firing a cluster of pellets."), TM._("Use this in close quarters to defeat any foe unfortunate enough to cross your path!"))
		},
		{
			MVWorldObjectDocumentationType.Star,
			new ItemDescription(TM._("Game objective: Star"), TM._("The player/team who first collects all the stars, wins!"), TM._("Collect all these sparkly trinkets to win games and unlock new paths."))
		},
		{
			MVWorldObjectDocumentationType.GrowthPill,
			new ItemDescription(TM._("Growth Pill"), TM._("Transforms a player into a giant!"), TM._("Become a giant KoGaMian, capable of slapping other players."))
		},
		{
			MVWorldObjectDocumentationType.MousePill,
			new ItemDescription(TM._("Mouse Pill"), TM._("Shrinks a player to the size of a mouse."), TM._("Become a miniature version of yourself, capable of entering any crevice."))
		},
		{
			MVWorldObjectDocumentationType.MouseGun,
			new ItemDescription(TM._("Mouse Gun"), TM._("A pistol that shrinks the target."), TM._("Turn enemy players into helpless little rodents, or help your friends navigate narrow areas."))
		},
		{
			MVWorldObjectDocumentationType.Colossus,
			new ItemDescription(MVAvatarLocal.GodzillaMode.screenName, TM._("Players can enter this object and transform into ") + MVAvatarLocal.GodzillaMode.screenName, TM._("Become the all powerful ") + MVAvatarLocal.GodzillaMode.screenName + TM._(", capable of frying any foe within your sights!"))
		},
		{
			MVWorldObjectDocumentationType.ThrowingStar,
			new ItemDescription(TM._("Shuriken"), TM._("A classic ninja shuriken."), TM._("Become one with the shadows using this classic shuriken."))
		},
		{
			MVWorldObjectDocumentationType.MultiThrowingStar,
			new ItemDescription(TM._("Multi Shuriken"), TM._("A handful of ninja shurikens."), TM._("Throw a line of shurikens, and strike several foes at once."))
		},
		{
			MVWorldObjectDocumentationType.CubeGun,
			new ItemDescription(TM._("Cube Gun"), TM._("Shoots cubes that stick to surfaces."), TM._("Build awesome towers, thick castle walls, incredible bridges or something entirely different. Your imagination is the limit!"))
		},
		{
			MVWorldObjectDocumentationType.Coin,
			new ItemDescription(TM._("Coin"), TM._("Players pick these up to pay for things."), TM._("Reward players with coins used to unlock new and interesting experiences within your game."))
		},
		{
			MVWorldObjectDocumentationType.CoinChest,
			new ItemDescription(TM._("Coin Chest"), TM._("Contains lots of coins, which can be used to pay for things."), TM._("Use this to reward players for reaching certain points, or for finding secrets in your game."))
		},
		{
			MVWorldObjectDocumentationType.DoubleSixShooter,
			new ItemDescription(TM._("Dual Revolvers"), TM._("A pair of high powered revolvers."), TM._("Most effective in mid to long range combat."))
		},
		{
			MVWorldObjectDocumentationType.GrowthGun,
			new ItemDescription(TM._("Growth Gun"), TM._("Transforms the target into a giant."), TM._("Make your enemies bigger targets, or empower friends by turning them into giants."))
		},
		{
			MVWorldObjectDocumentationType.SixShooter,
			new ItemDescription(TM._("Revolver"), TM._("A high powered revolver."), TM._("Most effective in mid to long range combat."))
		},
		{
			MVWorldObjectDocumentationType.NinjaRun,
			new ItemDescription(TM._("Lightning Speed"), TM._("Infuses the player with lightning speed."), TM._("This enables players to cross long gaps and climb steep walls, at high speed."))
		},
		{
			MVWorldObjectDocumentationType.Oculus,
			new ItemDescription(TM._("Oculus"), TM._("A customizable monster from another dimension. Hunts and kills nearby players."), TM._("These hideous creatures only have one vulnerability: Their eye! Jump on it to bounce high!"), new Vector3(0f, 0f, 0.7f))
		},
		{
			MVWorldObjectDocumentationType.Teleporter,
			new ItemDescription(TM._("Teleporter"), TM._("A pair of connected teleporters, capable of instantly transporting players between them."), TM._("Useful for separating different parts of a level, or to travel large distances quickly."), new Vector3(0.2f, -0.3f, -1f))
		},
		{
			MVWorldObjectDocumentationType.Hovercraft,
			new ItemDescription(TM._("Hovercraft"), TM._("A customizable hovercraft."), TM._("Enables players to traverse great distances over both land and water."), new Vector3(0f, 0f, -0.3f))
		},
		{
			MVWorldObjectDocumentationType.HamsterBall,
			new ItemDescription(TM._("Hamster Ball"), TM._("A giant hamster ball."), TM._("This bouncy ball offers a hilarious way of traveling. It can also float in the water!"), new Vector3(-0.3f, 0.8f, 0f))
		},
		{
			MVWorldObjectDocumentationType.FireSentryTower,
			new ItemDescription(TM._("Fire Sentry Tower"), TM._("A sentry tower, shooting fire at players who come too close. The sentry can be temporarily disabled by shooting at the orb part of it."), TM._("Fire sentries serve as stationary guards, presenting a challenge to anyone trying to pass."), new Vector3(0.5f, -0.9f, -1f))
		},
		{
			MVWorldObjectDocumentationType.MovingPlatform,
			new ItemDescription(TM._("Platform"), TM._("Create elevators or platforms with this customizable moving object!"), TM._("Useful when bridging the gap between stories in a building or separate platforms."))
		},
		{
			MVWorldObjectDocumentationType.HorizontalRotator,
			new ItemDescription(TM._("Horizontal Rotator"), TM._("A custom cube model, that rotates horizontally at a configurable speed."), TM._("Can be used as a challenging way of jumping between platforms, or as a method of smacking players mid-air."))
		},
		{
			MVWorldObjectDocumentationType.VerticalRotator,
			new ItemDescription(TM._("Vertical Rotator"), TM._("A custom cube model, that rotates vertically at a configurable speed."), TM._("Useful for pushing players off platforms, or to create beautiful windmills."))
		},
		{
			MVWorldObjectDocumentationType.BigJetpack,
			new ItemDescription(TM._("Dragonfly Jetpack"), TM._("A big customizable jetpack."), TM._("This jetpack offers unprecedented agility and speed, compared to it's sibling the Firefly Jetpack"), new Vector3(0f, -1f, 0.2f))
		},
		{
			MVWorldObjectDocumentationType.Ghost,
			new ItemDescription(TM._("Ghost"), TM._("An unkillable ghost which damages players on touch."), TM._("This scary monster can move through walls"))
		},
		{
			MVWorldObjectDocumentationType.FrostSentryTower,
			new ItemDescription(TM._("Frost Sentry Tower"), TM._("A sentry tower, shooting a beam of ice at players who come too close. The sentry can be temporarily disabled by shooting at the orb part of it."), TM._("Frost sentries serve as stationary guards, freezing players making them slip and slide as though walking on ice."), new Vector3(0.5f, -0.9f, -1f))
		},
		{
			MVWorldObjectDocumentationType.SmallJetpack,
			new ItemDescription(TM._("Firefly Jetpack"), TM._("A small customizable jetpack."), TM._("Soar to the skies with this lightweight jetpack."), new Vector3(0f, -0.7f, -0.5f))
		},
		{
			MVWorldObjectDocumentationType.PointLight,
			new ItemDescription(TM._("Light Cube"), TM._("A colored light, that can be controlled by logic signals."), TM._("Light up the dark areas of your game, or draw attention to a specific area."))
		},
		{
			MVWorldObjectDocumentationType.SpawnPointBlue,
			new ItemDescription(TM._("Blue Team Spawn Point"), TM._("Spawn point for the blue team. Place several to randomize spawn location."), string.Empty)
		},
		{
			MVWorldObjectDocumentationType.SpawnPointRed,
			new ItemDescription(TM._("Red Team Spawn Point"), TM._("Spawn point for the red team. Place several to randomize spawn location."), string.Empty)
		},
		{
			MVWorldObjectDocumentationType.SpawnPointGreen,
			new ItemDescription(TM._("Green Team Spawn Point"), TM._("Spawn point for the green team. Place several to randomize spawn location."), string.Empty)
		},
		{
			MVWorldObjectDocumentationType.SpawnPointYellow,
			new ItemDescription(TM._("Yellow Team Spawn Point"), TM._("Spawn point for the yellow team. Place several to randomize spawn location."), string.Empty)
		},
		{
			MVWorldObjectDocumentationType.Flag,
			new ItemDescription(TM._("Game objective: Flag"), TM._("The player who reaches the flag first, wins!"), TM._("Flags are a great way to give players an objective, purpose, or a sense of competition."))
		},
		{
			MVWorldObjectDocumentationType.Explosives,
			new ItemDescription(TM._("Explosive"), TM._("A bundle of dynamite that will explode when it receives a signal from a link."), TM._("Use this by connecting it to a pressure plate or lever."))
		},
		{
			MVWorldObjectDocumentationType.Fire,
			new ItemDescription(TM._("Fire"), TM._("This burns anyone who gets too close. The Fire Cube can be controlled by connecting other logic cubes such as the pressure plate."), TM._("You can pair this with the Smoke Cube to create a realistic effect!"))
		},
		{
			MVWorldObjectDocumentationType.Smoke,
			new ItemDescription(TM._("Smoke"), TM._("Emits a cloud of smoke. The smoke can be controlled by connecting other logic cubes such as the pressure plate."), TM._("Great cube if you want to obstruct the player's view. Can also be paired with the Fire Cube for a cool effect!"))
		},
		{
			MVWorldObjectDocumentationType.Text,
			new ItemDescription(TM._("Text"), TM._("An object enabling you to display a text in your game."), TM._("Display a game title, help text, or a piece of story through text."))
		},
		{
			MVWorldObjectDocumentationType.Skybox,
			new ItemDescription(TM._("Skybox Cube"), TM._("Manage the look of your game by setting fog, sky, and ambient lighting."), TM._("Adding this to your project will make it looks awesome! Give it a go!"))
		},
		{
			MVWorldObjectDocumentationType.WaterPlane,
			new ItemDescription(TM._("Water Cube"), TM._("Adjust the level and color of your games water."), TM._("WARNING: KoGaMians cannot breathe under water!"))
		},
		{
			MVWorldObjectDocumentationType.SoundEmitter,
			new ItemDescription(TM._("Speaker"), TM._("Add sounds to your game. There's a wide collection of sounds to choose from."), TM._("This is great for creating the atmosphere that suits your game."))
		},
		{
			MVWorldObjectDocumentationType.Checkpoint,
			new ItemDescription(TM._("Checkpoint"), TM._("A player who reaches this will respawn here and get fully healed."), TM._("This is a must have for any level with progression, especially parkour games! Make sure to rotate the checkpoint, so respawned players face the correct direction"))
		},
		{
			MVWorldObjectDocumentationType.OculusKillWinCondition,
			new ItemDescription(TM._("Game objective: Eliminate the Oculus"), TM._("Set a Oculus kill limit for your game. This object will not be usable if you haven't first purchased the Oculus."), TM._("The team or player that reaches the kill limit wins the game!"))
		},
		{
			MVWorldObjectDocumentationType.WindTurbine,
			new ItemDescription(TM._("Wind Turbine"), TM._("A big fan, which blows players and vehicles away."), TM._("You can stack several wind turbines on top of each other for a mega boost."))
		},
		{
			MVWorldObjectDocumentationType.RoundCube,
			new ItemDescription(TM._("Round time"), TM._("Set a time limit for a round. After the time is up, the round is restarted. Optionally a winner is determined based on avatar altitude."), TM._("Round cubes are a great if your game needs to be reset once in a while."))
		},
		{
			MVWorldObjectDocumentationType.PlayerKillWinCondition,
			new ItemDescription(TM._("Game objective: Death match"), TM._("Set a player kill limit for your game."), TM._("Add this to create a death match game. If you have multiple teams you can even create a team death match game!"))
		},
		{
			MVWorldObjectDocumentationType.CameraSettings,
			new ItemDescription(TM._("Camera Cube"), TM._("An object specifying the distance between the camera and your avatar."), TM._("This is a great way to control the look and feel of your game"))
		},
		{
			MVWorldObjectDocumentationType.TimeTrigger,
			new ItemDescription(TM._("Delay Cube"), TM._("When receiving a signal from another logic object it will delay for x seconds before sending a signal for y seconds."), TM._("You can put these in sequence to create really long waiting times."))
		},
		{
			MVWorldObjectDocumentationType.ToggleBox,
			new ItemDescription(TM._("Toggle Cube"), TM._("Switches between ON and OFF each time it receives a new signal"), TM._("Do you want to open and close the door with the same button? Then this is what you want."))
		},
		{
			MVWorldObjectDocumentationType.Negate,
			new ItemDescription(TM._("Negate Cube"), TM._("A logic object which sends the opposite of its input."), TM._("Inverting a signal open up many possibilities."))
		},
		{
			MVWorldObjectDocumentationType.And,
			new ItemDescription(TM._("And Cube"), TM._("The And Cube takes several inputs, and sends a signal when all of them are ON."), TM._("If you want players to stand on multiple pressure plates to open a door this is for you."))
		},
		{
			MVWorldObjectDocumentationType.PressurePlate,
			new ItemDescription(TM._("Pressure Plate"), TM._("Whenever a player steps on the plate, it sends a signal."), TM._("Connect this to an toggle box for a simple door, or to explosives for a land mine."))
		},
		{
			MVWorldObjectDocumentationType.ModelToggle,
			new ItemDescription(TM._("Cube Model Hider"), TM._("Hides connected cube model when powered."), TM._("This is most commonly used to enable/disable door models."))
		},
		{
			MVWorldObjectDocumentationType.PulseBox,
			new ItemDescription(TM._("Pulse Cube"), TM._("This logic cube sends signals in intervals. This means that it'll be disabled for a couple of seconds, then enabled for a couple of seconds, and so forth."), TM._("This is really neat when you want to create something that flickers, like a light."))
		},
		{
			MVWorldObjectDocumentationType.RandomBox,
			new ItemDescription(TM._("Random Cube"), TM._("Powers one connected object at random."), TM._("This is useful if you want to create unique experiences each round"))
		},
		{
			MVWorldObjectDocumentationType.CountingCube,
			new ItemDescription(TM._("Counting Cube"), TM._("Set a count. Each time the cube receives a signal it counts down. When it reaches 0 it sends a signal"), TM._("This looks super cool and can be used as a count down display!"))
		},
		{
			MVWorldObjectDocumentationType.ShootableButton,
			new ItemDescription(TM._("Target cube"), TM._("Sends a signal when shot."), TM._("Can be used to open doors or secret areas from a distance by shooting."))
		},
		{
			MVWorldObjectDocumentationType.Lever,
			new ItemDescription(TM._("Lever"), TM._("Sends a signal when pulled."), TM._("Open doors, trigger explosives. You can do a lot of things with a lever!"))
		},
		{
			MVWorldObjectDocumentationType.CollectTheItem,
			new ItemDescription(TM._("Collect And Drop"), TM._("A pair consisting of a pickup and a drop off area. Sends a signal when someone brings the pickup into the drop off area."), TM._("Can be used for everything from locked doors (bring the key to the door), to pizza deliveries (bring the pizza to the helicopter)."))
		},
		{
			MVWorldObjectDocumentationType.HealRay,
			new ItemDescription(TM._("Heal Ray"), TM._("Shoots a ray that heals people, repairs vehicles, and removes poison and fire."), TM._("Don't let your friends die, shoot them!"))
		},
		{
			MVWorldObjectDocumentationType.GlobalSoundEmitter,
			new ItemDescription(TM._("Global Speaker"), TM._("Adds a global sound to the game that can be heard from anywhere."), TM._("This can be used to make a better ambiance for the game."))
		},
		{
			MVWorldObjectDocumentationType.FinishLine,
			new ItemDescription(TM._("Game objective: Finish Line"), TM._("Records the time it took for a player to reach it, without resetting the round."), TM._("Try it with a round cube to determine the winner after a set period of time."))
		}
	};

	public InventoryItem()
	{
	}

	public InventoryItem(Dictionary<byte, object> data)
	{
		itemID = (int)data[40];
		itemCategoryID = (int)data[151];
		itemTypeID = (int)data[41];
		name = (string)data[42];
		this.data = (byte[])data[43];
		slotPosition = (int)data[45];
		resellable = (bool)data[139];
		authorProfileID = (int)data[138];
		originalItemID = (int)data[140];
		priceGold = (int)data[69];
		isDefaultInvItem = false;
	}

	public InventoryItem(int itemID, Dictionary<object, object> itemData)
	{
		this.itemID = itemID;
		itemCategoryID = (int)itemData[(byte)116];
		itemTypeID = (int)itemData[(byte)15];
		name = (string)itemData[(byte)10];
		description = (string)itemData[(byte)107];
		resellable = (bool)itemData[(byte)104];
		priceGold = (int)itemData[(byte)76];
		shopInventoryID = (int)itemData[(byte)108];
		authorProfileID = (int)itemData[(byte)106];
		originalItemID = (int)itemData[(byte)110];
		isDeleted = (bool)itemData[(byte)109];
		if (!isDeleted)
		{
			data = (byte[])itemData[(byte)11];
		}
		isDefaultInvItem = (bool)itemData[(byte)141];
	}

	public InventoryItem(ShopItem itemToCopy)
	{
		itemCategoryID = itemToCopy.itemCategoryID;
		itemTypeID = itemToCopy.itemTypeID;
		name = itemToCopy.name;
		description = itemToCopy.description;
		resellable = itemToCopy.resellable;
		priceGold = itemToCopy.priceGold;
		data = itemToCopy.data;
		itemID = itemToCopy.itemID;
		isDeleted = false;
		isDefaultInvItem = false;
	}

	public void ApplyLocalDescriptionOverride(MVWorldObjectDocumentationType t)
	{
		if (localItemDescriptionOverride.ContainsKey(t))
		{
			ItemDescription itemDescription = localItemDescriptionOverride[t];
			name = itemDescription.Name;
			description = itemDescription.Description;
		}
	}
}
