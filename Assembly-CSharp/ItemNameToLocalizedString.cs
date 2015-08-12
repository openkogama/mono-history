using System.Collections.Generic;

public class ItemNameToLocalizedString
{
	private static ItemNameToLocalizedString instance = new ItemNameToLocalizedString();

	private Dictionary<string, string> itemTextToTipID;

	public static ItemNameToLocalizedString Instance => instance;

	private ItemNameToLocalizedString()
	{
		Init();
		TM.LanguageChanged(Init);
	}

	public static string GetToolTipTextFromItemName(string itemName)
	{
		if (!Instance.itemTextToTipID.ContainsKey(itemName))
		{
			return itemName;
		}
		return Instance.itemTextToTipID[itemName];
	}

	private void Init()
	{
		itemTextToTipID = new Dictionary<string, string>
		{
			{
				"Flamethrower",
				TM._("FlameThrower")
			},
			{
				"Shotgun",
				TM._("Shotgun")
			},
			{
				"Health",
				TM._("Health")
			},
			{
				"CenterGun",
				TM._("Center Gun")
			},
			{
				"ImpulseGun",
				TM._("Impulse Gun")
			},
			{
				"Bazooka",
				TM._("Bazooka")
			},
			{
				"RailGun",
				TM._("Rail Gun")
			},
			{
				"Sword",
				TM._("Sword")
			},
			{
				"Mutant",
				TM._("Mutant pickup: kill non-mutants by touching them")
			},
			{
				"SoundEmitter",
				TM._("Sound Emitter")
			},
			{
				"PointLight",
				TM._("Light")
			},
			{
				"WaterPlane",
				TM._("Water Plane")
			},
			{
				"Explosives",
				TM._("Bomb")
			},
			{
				"Fire",
				TM._("Fire")
			},
			{
				"Smoke",
				TM._("Smoke")
			},
			{
				"TextMsg",
				TM._("Text Message")
			},
			{
				"Skybox",
				TM._("Skybox")
			},
			{
				"SpawnPointBlue",
				TM._("Start Point")
			},
			{
				"SpawnPointRed",
				TM._("Start Point")
			},
			{
				"SpawnPointGreen",
				TM._("Start Point")
			},
			{
				"SpawnPointYellow",
				TM._("Start Point")
			},
			{
				"Flag",
				TM._("Flag")
			},
			{
				"RandomBox",
				TM._("Randomizer")
			},
			{
				"ModelToggle",
				TM._("Object Enabler")
			},
			{
				"PulseBox",
				TM._("Pulse Box")
			},
			{
				"And",
				TM._("And Box")
			},
			{
				"PressurePlate",
				TM._("Pressure Plate")
			},
			{
				"TimeTrigger",
				TM._("Time Trigger")
			},
			{
				"ToggleBox",
				TM._("Toggle Box")
			},
			{
				"Negate",
				TM._("Negate Box")
			},
			{
				"RoundCube",
				TM._("Round Time")
			},
			{
				"OculusKillLimit",
				TM._("Oculus Kill Limit")
			},
			{
				"KillLimit",
				TM._("Kill Limit")
			}
		};
	}
}
