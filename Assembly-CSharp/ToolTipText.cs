using System.Collections.Generic;
using Localize;

public class ToolTipText
{
	private static ToolTipText instance = new ToolTipText();

	private Dictionary<string, TextSlotIndex> itemTextToTipID = new Dictionary<string, TextSlotIndex>
	{
		{
			"Flamethrower",
			TextSlotIndex.FlameThrowerPickup
		},
		{
			"Shotgun",
			TextSlotIndex.ShotgunPickup
		},
		{
			"Health",
			TextSlotIndex.HealthPickup
		},
		{
			"CenterGun",
			TextSlotIndex.CenterGunPickup
		},
		{
			"ImpulseGun",
			TextSlotIndex.ImpulseGunPickup
		},
		{
			"Bazooka",
			TextSlotIndex.BazookaPickup
		},
		{
			"RailGun",
			TextSlotIndex.RailGunPickup
		},
		{
			"Sword",
			TextSlotIndex.SwordPickup
		},
		{
			"Mutant",
			TextSlotIndex.MutantPickup
		},
		{
			"SoundEmitter",
			TextSlotIndex.SoundEmitterBox
		},
		{
			"PointLight",
			TextSlotIndex.Light
		},
		{
			"WaterPlane",
			TextSlotIndex.WaterPlaneBox
		},
		{
			"Explosives",
			TextSlotIndex.Bomb
		},
		{
			"Fire",
			TextSlotIndex.FireBox
		},
		{
			"Smoke",
			TextSlotIndex.Smoke
		},
		{
			"TextMsg",
			TextSlotIndex.TextMessage
		},
		{
			"Skybox",
			TextSlotIndex.Skybox
		},
		{
			"SpawnPointBlue",
			TextSlotIndex.StartPoint
		},
		{
			"SpawnPointRed",
			TextSlotIndex.StartPoint
		},
		{
			"SpawnPointGreen",
			TextSlotIndex.StartPoint
		},
		{
			"SpawnPointYellow",
			TextSlotIndex.StartPoint
		},
		{
			"Flag",
			TextSlotIndex.Flag
		},
		{
			"RandomBox",
			TextSlotIndex.RandomBox
		},
		{
			"ModelToggle",
			TextSlotIndex.ModelToggle
		},
		{
			"PulseBox",
			TextSlotIndex.PulseBox
		},
		{
			"And",
			TextSlotIndex.AndBox
		},
		{
			"PressurePlate",
			TextSlotIndex.PressurePlate
		},
		{
			"TimeTrigger",
			TextSlotIndex.TimeTrigger
		},
		{
			"ToggleBox",
			TextSlotIndex.ToggleBox
		},
		{
			"Negate",
			TextSlotIndex.NegateBox
		}
	};

	public static ToolTipText Instance => instance;

	private ToolTipText()
	{
	}

	public string GetToolTipText(TextSlotIndex id)
	{
		return Localization.Instance.GetText(id);
	}

	public string GetToolTipTextFromItemName(string itemName)
	{
		if (!itemTextToTipID.ContainsKey(itemName))
		{
			return itemName;
		}
		return GetToolTipText(itemTextToTipID[itemName]);
	}
}
