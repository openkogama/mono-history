using System.Collections.Generic;
using ThemeAttributes;
using UnityEngine;

public abstract class CloudyThemeBase : Theme
{
	public static class SettingGroups
	{
		public const int all = 0;

		public const int dayNightCycleOn = 1;

		public const int dayNightCycleOff = 2;

		public const int realTimeOn = 4;

		public const int realTimeOff = 8;

		public const int fogEnabled = 16;

		public const int fogDisabled = 32;
	}

	[Header("Day/night-cycle", order = 2)]
	[SerializeField]
	[Header("Attributes", order = 1)]
	private BoolAttribute useDayNightCycle;

	[SerializeField]
	private BoolAttribute useServerTime;

	[SerializeField]
	private IntAttribute colorPreset;

	[SerializeField]
	private FloatAttribute gameStartTime;

	[SerializeField]
	private FloatAttribute cycleLength;

	[SerializeField]
	private FloatAttribute sunAltitude;

	[SerializeField]
	private FloatAttribute sunLongitude;

	[SerializeField]
	private FloatAttribute moonAltitude;

	[SerializeField]
	private FloatAttribute moonLongitude;

	[Header("Sky")]
	[SerializeField]
	private ColorAttribute topColor;

	[SerializeField]
	private ColorAttribute bottomColor;

	[SerializeField]
	[Header("Stars")]
	private ColorAttribute starsTint;

	[SerializeField]
	private FloatAttribute starsExtinction;

	[SerializeField]
	private FloatAttribute starsTwinkilingSpeed;

	[Header("Sun")]
	[SerializeField]
	private ColorAttribute sunTint;

	[SerializeField]
	private FloatAttribute sunSize;

	[SerializeField]
	private FloatAttribute sunHeight;

	[SerializeField]
	private FloatAttribute sunAxis;

	[SerializeField]
	private FloatAttribute sunLightContrast;

	[SerializeField]
	private FloatAttribute sunLightIntensity;

	[SerializeField]
	private FloatAttribute sunFlareBrightness;

	[Header("Moon")]
	[SerializeField]
	private ColorAttribute moonTint;

	[SerializeField]
	private FloatAttribute moonSize;

	[SerializeField]
	private FloatAttribute moonHeight;

	[SerializeField]
	private FloatAttribute moonAxis;

	[SerializeField]
	private FloatAttribute moonLightContrast;

	[SerializeField]
	private FloatAttribute moonLightIntensity;

	[SerializeField]
	private FloatAttribute moonFlareBrightness;

	[SerializeField]
	[Header("Clouds")]
	private FloatAttribute cloudsHeight;

	[SerializeField]
	private FloatAttribute cloudsOffset;

	[SerializeField]
	private FloatAttribute cloudsRotationSpeed;

	[SerializeField]
	[Header("Fog")]
	private BoolAttribute useFog;

	[SerializeField]
	private IntAttribute fogMode;

	[SerializeField]
	private FloatAttribute fogDensity;

	[SerializeField]
	[Header("Misc")]
	private FloatAttribute exposure;

	[SerializeField]
	[Header("Dependencies")]
	private ThemeSkybox skybox;

	[SerializeField]
	private DayNightCycle dayNightCycle;

	[SerializeField]
	private DayNightCycleController cycleControllerPrefab;

	public override List<RectTransform> Controllers
	{
		get
		{
			List<RectTransform> list = new List<RectTransform>(0);
			if (useDayNightCycle.Value && !useServerTime.Value)
			{
				DayNightCycleController dayNightCycleController = Object.Instantiate(cycleControllerPrefab);
				dayNightCycleController.Initialize(dayNightCycle);
				list.Add((RectTransform)dayNightCycleController.transform);
			}
			return list;
		}
	}

	public override void ThemeReset()
	{
		base.ThemeReset();
		dayNightCycle.Reset();
	}

	protected override void InitializeAttributes()
	{
		useDayNightCycle.Initialize(Settings, "useDayNightCycle", 0, ToggleDayNightCycle);
		useServerTime.Initialize(Settings, "useServerTime", 1, ToggleDayNightCycleUseServerTime);
		colorPreset.Initialize(Settings, "colorPresetIndex", 1, (int i) =>
		{
			dayNightCycle.ColorPreset = i;
		});
		gameStartTime.Initialize(Settings, "gameStartTime", 9, (float f) =>
		{
			dayNightCycle.InitialTimeOfDay = f;
		});
		cycleLength.Initialize(Settings, "cycleLength", 9, (float f) =>
		{
			dayNightCycle.CycleLength = f;
		});
		topColor.Initialize(Settings, "topColor", 2, (Color c) =>
		{
			skybox.TopColor = c;
		});
		bottomColor.Initialize(Settings, "bottomColor", 2, (Color c) =>
		{
			skybox.BottomColor = c;
		});
		starsTint.Initialize(Settings, "starsTint", 2, (Color c) =>
		{
			skybox.StarsTint = c;
		});
		starsExtinction.Initialize(Settings, "starsExtinction", 0, (float f) =>
		{
			skybox.StarsExtinction = f;
		});
		starsTwinkilingSpeed.Initialize(Settings, "starsTwinklingSpeed", 0, (float f) =>
		{
			skybox.StarsTwinklingSpeed = f;
		});
		sunTint.Initialize(Settings, "sunTint", 2, (Color c) =>
		{
			skybox.SunTint = c;
			skybox.RecalculateSunLight();
		});
		sunSize.Initialize(Settings, "sunSize", 0, (float f) =>
		{
			skybox.SunSize = f;
		});
		sunHeight.Initialize(Settings, "sunHeight", 2, (float f) =>
		{
			skybox.SunHeight = f;
			skybox.RecalculateSunLight();
		});
		sunAxis.Initialize(Settings, "sunAxis", 2, (float f) =>
		{
			skybox.SunAxisDegrees = f;
		});
		sunAltitude.Initialize(Settings, "sunAltitude", 1, (float f) =>
		{
			dayNightCycle.SunAltitude = f;
		});
		sunLongitude.Initialize(Settings, "sunLongitude", 1, (float f) =>
		{
			dayNightCycle.SunLongitude = f;
		});
		sunLightContrast.Initialize(Settings, "sunLightContrast", 2, (float f) =>
		{
			skybox.SunLightContrast = f;
			skybox.RecalculateSunLight();
		});
		sunLightIntensity.Initialize(Settings, "sunLightIntensity", 2, (float f) =>
		{
			skybox.SunLightIntensity = f;
			skybox.RecalculateSunLight();
		});
		sunFlareBrightness.Initialize(Settings, "sunFlareBrightness", 0, (float f) =>
		{
			skybox.SunFlareBrightness = f;
		});
		moonTint.Initialize(Settings, "moonTint", 2, (Color c) =>
		{
			skybox.MoonTint = c;
			skybox.RecalculateMoonLight();
		});
		moonSize.Initialize(Settings, "moonSize", 0, (float f) =>
		{
			skybox.MoonSize = f;
		});
		moonHeight.Initialize(Settings, "moonHeight", 2, (float f) =>
		{
			skybox.MoonHeight = f;
			skybox.RecalculateMoonLight();
		});
		moonAxis.Initialize(Settings, "moonAxis", 2, (float f) =>
		{
			skybox.MoonAxisDegrees = f;
		});
		moonAltitude.Initialize(Settings, "moonAltitude", 1, (float f) =>
		{
			dayNightCycle.MoonAltitude = f;
		});
		moonLongitude.Initialize(Settings, "moonLongitude", 1, (float f) =>
		{
			dayNightCycle.MoonLongitude = f;
		});
		moonLightContrast.Initialize(Settings, "moonLightContrast", 2, (float f) =>
		{
			skybox.MoonLightContrast = f;
			skybox.RecalculateMoonLight();
		});
		moonLightIntensity.Initialize(Settings, "moonLightIntensity", 2, (float f) =>
		{
			skybox.MoonLightIntensity = f;
			skybox.RecalculateMoonLight();
		});
		moonFlareBrightness.Initialize(Settings, "moonFlareBrightness", 0, (float f) =>
		{
			skybox.MoonFlareBrightness = f;
		});
		cloudsHeight.Initialize(Settings, "cloudsHeight", 0, (float f) =>
		{
			skybox.CloudsHeight = f;
		});
		cloudsOffset.Initialize(Settings, "cloudsOffset", 0, (float f) =>
		{
			skybox.CloudsOffset = f;
		});
		cloudsRotationSpeed.Initialize(Settings, "cloudsRotationSpeed", 0, (float f) =>
		{
			skybox.CloudsRotationSpeed = f;
		});
		useFog.Initialize(Settings, "useFog", 0, ToggleFog);
		fogMode.Initialize(Settings, "fogMode", 16, (int i) =>
		{
			skybox.FogMode = (FogMode)i;
		});
		fogDensity.Initialize(Settings, "fogDensity", 16, (float f) =>
		{
			skybox.FogDensity = f;
		});
		exposure.Initialize(Settings, "exposure", 0, (float f) =>
		{
			skybox.Exposure = f;
		});
	}

	protected override void InitializeComponents()
	{
		skybox.Initialize(this);
		dayNightCycle.Initialize();
	}

	private void ToggleDayNightCycle(bool b)
	{
		dayNightCycle.enabled = b;
		if (b)
		{
			Settings.DisableAttributeGroups(2);
			Settings.EnableAttributeGroups(1);
		}
		else
		{
			Settings.DisableAttributeGroups(1);
			Settings.EnableAttributeGroups(2);
		}
	}

	private void ToggleDayNightCycleUseServerTime(bool b)
	{
		dayNightCycle.UseServerTime = b;
		if (b)
		{
			Settings.DisableAttributeGroups(8);
			Settings.EnableAttributeGroups(4);
		}
		else
		{
			Settings.DisableAttributeGroups(4);
			Settings.EnableAttributeGroups(8);
		}
	}

	private void ToggleFog(bool b)
	{
		skybox.FogEnabled = b;
		if (b)
		{
			Settings.DisableAttributeGroups(32);
			Settings.EnableAttributeGroups(16);
		}
		else
		{
			Settings.DisableAttributeGroups(16);
			Settings.EnableAttributeGroups(32);
		}
	}
}
