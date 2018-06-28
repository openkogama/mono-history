using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GodzillaSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	public enum Sizes
	{
		S = 0,
		M = 1,
		L = 2,
		XL = 3,
		count = 4,
		none = count
	}

	public static class Strings
	{
		public const string size = "size";
	}

	private const int numOfSizes = 4;

	[Header("Settings")]
	[SerializeField]
	private string[] sizes = new string[4];

	[SerializeField]
	[Header("Dependencies")]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider sizeSlider;

	[SerializeField]
	private Text title;

	[SerializeField]
	private Text sizeLabel;

	public static AvatarModifierPackageType GetPackageType(Sizes s)
	{
		switch (s)
		{
		case Sizes.S:
			return AvatarModifierPackageType.GodzillaS;
		case Sizes.M:
			return AvatarModifierPackageType.GodzillaM;
		case Sizes.L:
			return AvatarModifierPackageType.GodzillaL;
		case Sizes.XL:
			return AvatarModifierPackageType.GodzillaXL;
		default:
			Debug.LogError(string.Concat("GodzillaSettings size: ", s, " not accounted for."));
			return AvatarModifierPackageType.None;
		}
	}

	private void OnValidate()
	{
		title.text = "Colossus settings";
		string[] array = new string[4]
		{
			TM._("Small"),
			TM._("Default"),
			TM._("Large"),
			TM._("Enormous")
		};
		for (int i = 0; i < 4; i++)
		{
			if (sizes[i].Length < 1)
			{
				sizes[i] = array[i];
			}
		}
		sizeSlider.Initialize("size", 1, 0, 3);
	}

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Colossus);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		int value = (int)data["size"];
		sizeSlider.Initialize("size", value, 0, 3);
	}

	public void OnSettingChanged(string key, object value)
	{
		int num = (int)(float)value;
		settingsBase.OnSettingChanged(key, num);
		switch (key)
		{
		case "size":
			sizeLabel.text = sizes[num];
			break;
		default:
			Debug.LogError("GodzillaSettings:OnSettingChanged - Unexpected key: " + key);
			break;
		}
	}
}
