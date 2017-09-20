using System.Linq;
using UnityEngine;

public class MVQualitySettings : MonoBehaviour
{
	public delegate void OnQualityLevedChanged(int level);

	public const int QualitySD = 0;

	public const int QualityHD = 1;

	public const int QualitySDAndroid = 2;

	private static LodData[] lodSettingsFastest = new LodData[2]
	{
		new LodData(0f, isVisible: true, shadows: true),
		new LodData(300f, isVisible: false, shadows: true)
	};

	private static LodData[] lodSettingsFast = new LodData[2]
	{
		new LodData(0f, isVisible: true, shadows: true),
		new LodData(300f, isVisible: false, shadows: true)
	};

	private static LodData[] lodSettingsSimple = new LodData[2]
	{
		new LodData(0f, isVisible: true, shadows: true),
		new LodData(300f, isVisible: false, shadows: true)
	};

	private static LodData[] lodSettingsGood = new LodData[2]
	{
		new LodData(0f, isVisible: true, shadows: true),
		new LodData(300f, isVisible: false, shadows: true)
	};

	private static LodData[] lodSettingsBeautiful = new LodData[2]
	{
		new LodData(0f, isVisible: true, shadows: true),
		new LodData(500f, isVisible: false, shadows: true)
	};

	private static LodData[] lodSettingsFantastic = new LodData[2]
	{
		new LodData(0f, isVisible: true, shadows: true),
		new LodData(300f, isVisible: false, shadows: true)
	};

	private static LodData[][] lodSettings = new LodData[6][] { lodSettingsFastest, lodSettingsFast, lodSettingsSimple, lodSettingsGood, lodSettingsBeautiful, lodSettingsFantastic };

	public static OnQualityLevedChanged onQualityLevelChanged;

	public static LodData[] CurrentLodData => lodSettings[QualitySettings.GetQualityLevel()];

	public static int CurrentLevel
	{
		get
		{
			return QualitySettings.GetQualityLevel();
		}
		set
		{
			if (value != QualitySettings.GetQualityLevel())
			{
				QualitySettings.SetQualityLevel(value, applyExpensiveChanges: true);
				if (onQualityLevelChanged != null)
				{
					onQualityLevelChanged(value);
				}
			}
		}
	}

	public void Start()
	{
		string[] names = QualitySettings.names;
		string text = string.Empty;
		if (names.Length != 3)
		{
			text += "QualitySettings have been changed:";
		}
		if (!names.Contains("SD"))
		{
			text += " SD Missing, ";
		}
		if (!names.Contains("SDAndroid"))
		{
			text += " SDAndroid Missing, ";
		}
		if (!names.Contains("HD"))
		{
			text += " HD Missing, ";
		}
		if (text != string.Empty)
		{
			Debug.Log("Error: " + text);
			Debug.LogError("QualitySettings not correct! Missing 1 or several quality levels.");
		}
		CurrentLevel = 0;
	}
}
