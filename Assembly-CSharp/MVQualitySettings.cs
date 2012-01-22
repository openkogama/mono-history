using System;
using UnityEngine;

public class MVQualitySettings : MonoBehaviour
{
	public delegate void OnQualityLevedChanged(QualityLevel level);

	private static LodData[] lodSettingsFastest = new LodData[2]
	{
		new LodData(0f, isVisible: true, MeshSetting.OriginalMesh, shadows: true),
		new LodData(300f, isVisible: false, MeshSetting.OriginalMesh, shadows: true)
	};

	private static LodData[] lodSettingsFast = new LodData[2]
	{
		new LodData(0f, isVisible: true, MeshSetting.OriginalMesh, shadows: true),
		new LodData(300f, isVisible: false, MeshSetting.OriginalMesh, shadows: true)
	};

	private static LodData[] lodSettingsSimple = new LodData[2]
	{
		new LodData(0f, isVisible: true, MeshSetting.OriginalMesh, shadows: true),
		new LodData(300f, isVisible: false, MeshSetting.OriginalMesh, shadows: true)
	};

	private static LodData[] lodSettingsGood = new LodData[2]
	{
		new LodData(0f, isVisible: true, MeshSetting.OriginalMesh, shadows: true),
		new LodData(300f, isVisible: false, MeshSetting.OriginalMesh, shadows: true)
	};

	private static LodData[] lodSettingsBeautiful = new LodData[2]
	{
		new LodData(0f, isVisible: true, MeshSetting.OriginalMesh, shadows: true),
		new LodData(500f, isVisible: false, MeshSetting.OriginalMesh, shadows: true)
	};

	private static LodData[] lodSettingsFantastic = new LodData[2]
	{
		new LodData(0f, isVisible: true, MeshSetting.OriginalMesh, shadows: true),
		new LodData(300f, isVisible: false, MeshSetting.OriginalMesh, shadows: true)
	};

	private static LodData[][] lodSettings = new LodData[6][] { lodSettingsFastest, lodSettingsFast, lodSettingsSimple, lodSettingsGood, lodSettingsBeautiful, lodSettingsFantastic };

	public PostprocessFog postprocessFogEffect;

	public static OnQualityLevedChanged onQualityLevelChanged;

	public static LodData[] CurrentLodData
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return lodSettings[QualitySettings.currentLevel];
		}
	}

	public static QualityLevel CurrentLevel
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return QualitySettings.currentLevel;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (value != QualitySettings.currentLevel)
			{
				QualitySettings.currentLevel = value;
				if (onQualityLevelChanged != null)
				{
					onQualityLevelChanged(value);
				}
			}
		}
	}

	private void QualityChanged(QualityLevel level)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected I4, but got Unknown
		postprocessFogEffect = Object.FindObjectOfType(typeof(PostprocessFog)) as PostprocessFog;
		switch ((int)level)
		{
		case 0:
		case 1:
		case 2:
			Shader.globalMaximumLOD = 100;
			break;
		case 3:
		case 4:
		case 5:
			Shader.globalMaximumLOD = 500;
			break;
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		CurrentLevel = (QualityLevel)2;
		QualityChanged(CurrentLevel);
	}
}
