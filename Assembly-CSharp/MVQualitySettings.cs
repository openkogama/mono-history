using System;
using UnityEngine;

public class MVQualitySettings : MonoBehaviour
{
	public delegate void OnQualityLevedChanged(int level);

	public const int QualitySD = 0;

	public const int QualityHD = 1;

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

	public PostprocessFog postprocessFogEffect;

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

	private void QualityChanged(int level)
	{
		postprocessFogEffect = UnityEngine.Object.FindObjectOfType(typeof(PostprocessFog)) as PostprocessFog;
		switch (level)
		{
		}
	}

	private void OnEnable()
	{
		onQualityLevelChanged = (OnQualityLevedChanged)Delegate.Combine(onQualityLevelChanged, new OnQualityLevedChanged(QualityChanged));
	}

	private void OnDisable()
	{
		onQualityLevelChanged = (OnQualityLevedChanged)Delegate.Remove(onQualityLevelChanged, new OnQualityLevedChanged(QualityChanged));
	}

	public void Start()
	{
		CurrentLevel = 0;
		QualityChanged(CurrentLevel);
	}
}
