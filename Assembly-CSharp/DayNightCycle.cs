using System;
using Borodar.FarlandSkies.CloudyCrownPro.DotParams;
using ThemeTimers;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	[Header("Configuration")]
	[Range(0f, 100f)]
	[SerializeField]
	private float _sunrise = 25f;

	[Range(0f, 100f)]
	[SerializeField]
	private float _sunset = 85f;

	[Tooltip("Max angle between the horizon and the center of sun’s disk")]
	[SerializeField]
	private float _sunAltitude = 45f;

	[Tooltip("Angle between z-axis and the center of sun’s disk at sunrise")]
	[SerializeField]
	private float _sunLongitude;

	[Tooltip("A pair of angles that limit visible orbit of the sun")]
	[SerializeField]
	private Vector2 _sunOrbit = new Vector2(-20f, 200f);

	[Range(0f, 100f)]
	[SerializeField]
	private float _moonrise = 90f;

	[SerializeField]
	[Range(0f, 100f)]
	private float _moonset = 22.5f;

	[SerializeField]
	[Tooltip("Max angle between the horizon and the center of moon’s disk")]
	private float _moonAltitude = 45f;

	[Tooltip("Angle between z-axis and the center of moon’s disk at moonrise")]
	[SerializeField]
	private float _moonLongitude;

	[Tooltip("A pair of angles that limit visible orbit of the moon")]
	[SerializeField]
	private Vector2 _moonOrbit = new Vector2(-20f, 200f);

	[SerializeField]
	[Header("Dependencies")]
	private ThemeSkybox skybox;

	[SerializeField]
	private DayNightCycleColorPresets colorPresets;

	private float _sunDuration;

	private Vector3 _sunAttitudeVector;

	private float _moonDuration;

	private Vector3 _moonAttitudeVector;

	private bool useServerTime;

	private float cycleLength = 1f;

	private ITimer timer;

	private float cycleStartTime;

	private bool initialized;

	private bool isPaused;

	private DayNightCycleColorPresets.Preset activeColorPreset;

	private float CurrentStepTime => 0f;

	private SkyParamsList _skyParamsList => activeColorPreset.Sky;

	private CelestialParamsList _sunParamsList => activeColorPreset.Sun;

	private CelestialParamsList _moonParamsList => activeColorPreset.Moon;

	private StarsParamsList _starsParamsList => activeColorPreset.Stars;

	public bool IsPaused
	{
		get
		{
			return isPaused;
		}
		set
		{
			isPaused = value;
		}
	}

	public int ColorPreset
	{
		set
		{
			activeColorPreset = colorPresets[value];
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public bool UseServerTime
	{
		set
		{
			useServerTime = value;
			if (value)
			{
				timer = new SystemTimer();
			}
			else
			{
				timer = new Timer(cycleStartTime, cycleLength);
			}
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float InitialTimeOfDay
	{
		set
		{
			cycleStartTime = value % 100f;
			if (initialized)
			{
				timer = new Timer(cycleStartTime, cycleLength);
				Update(timer.Time);
				EditorAssert(!useServerTime);
			}
		}
	}

	public float TimeOfDay
	{
		get
		{
			return timer.Time;
		}
		set
		{
			timer = new Timer(value, cycleLength);
			EditorAssert(!useServerTime);
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float CycleLength
	{
		set
		{
			cycleLength = value;
			if (initialized)
			{
				timer = new Timer(timer.Time, cycleLength);
				Update(timer.Time);
				EditorAssert(!useServerTime);
			}
		}
	}

	public float SunriseTime
	{
		set
		{
			_sunrise = value % 100f;
			RecalcSunDuration();
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float SunsetTime
	{
		set
		{
			_sunset = value % 100f;
			RecalcSunDuration();
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float MoonriseTime
	{
		set
		{
			_moonrise = value % 100f;
			RecalcMoonDuration();
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float MoonsetTime
	{
		set
		{
			_moonset = value % 100f;
			RecalcMoonDuration();
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float SunAltitude
	{
		set
		{
			_sunAltitude = value;
			RecalcSunAltitudeVector();
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float SunLongitude
	{
		set
		{
			_sunLongitude = value;
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float MoonAltitude
	{
		set
		{
			_moonAltitude = value;
			RecalcMoonAltitudeVector();
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	public float MoonLongitude
	{
		set
		{
			_moonLongitude = value;
			if (initialized)
			{
				Update(timer.Time);
			}
		}
	}

	private SkyParam CurrentSkyParam { get; set; }

	private StarsParam CurrentStarsParam { get; set; }

	private CelestialParam CurrentSunParam { get; set; }

	private CelestialParam CurrentMoonParam { get; set; }

	private void EditorAssert(bool b)
	{
	}

	public void Initialize()
	{
		RecalcSunDuration();
		RecalcSunAltitudeVector();
		RecalcMoonDuration();
		RecalcMoonAltitudeVector();
		if (activeColorPreset == null)
		{
			activeColorPreset = colorPresets[0];
		}
		if (useServerTime)
		{
			timer = new SystemTimer();
		}
		else
		{
			timer = new Timer((CurrentStepTime + cycleStartTime) % 100f, cycleLength);
		}
		initialized = true;
	}

	public void Reset()
	{
		if (!useServerTime)
		{
			timer = new Timer(cycleStartTime, cycleLength);
		}
	}

	private void RecalcSunDuration()
	{
		_sunDuration = ((!(_sunrise < _sunset)) ? (100f - _sunrise + _sunset) : (_sunset - _sunrise));
	}

	private void RecalcSunAltitudeVector()
	{
		float f = _sunAltitude * ((float)Math.PI / 180f);
		_sunAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
	}

	private void RecalcMoonDuration()
	{
		_moonDuration = ((!(_moonrise < _moonset)) ? (100f - _moonrise + _moonset) : (_moonset - _moonrise));
	}

	private void RecalcMoonAltitudeVector()
	{
		float f = _moonAltitude * ((float)Math.PI / 180f);
		_moonAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
	}

	protected void Update()
	{
		if (!isPaused)
		{
			timer.Update();
			Update(timer.Time);
		}
	}

	private void Update(float timeOfDay)
	{
		CurrentSkyParam = _skyParamsList.GetParamPerTime(timeOfDay);
		skybox.TopColor = CurrentSkyParam.TopColor;
		skybox.BottomColor = CurrentSkyParam.BottomColor;
		CurrentStarsParam = _starsParamsList.GetParamPerTime(timeOfDay);
		skybox.StarsTint = CurrentStarsParam.TintColor;
		if (timeOfDay > _sunrise || timeOfDay < _sunset)
		{
			float num = ((!(_sunrise < timeOfDay)) ? (100f + timeOfDay - _sunrise) : (timeOfDay - _sunrise));
			float t = ((!(num < _sunDuration)) ? ((_sunDuration - num) / _sunDuration) : (num / _sunDuration));
			float angle = Mathf.Lerp(_sunOrbit.x, _sunOrbit.y, t);
			Quaternion sunRotation = Quaternion.AngleAxis(_sunLongitude - 180f, Vector3.up) * Quaternion.AngleAxis(angle, _sunAttitudeVector);
			sunRotation.eulerAngles = new Vector3(sunRotation.eulerAngles.x, sunRotation.eulerAngles.y, 0f);
			skybox.SunRotation = sunRotation;
		}
		CurrentSunParam = _sunParamsList.GetParamPerTime(timeOfDay);
		skybox.SunTint = CurrentSunParam.TintColor;
		skybox.SunLight.color = CurrentSunParam.LightColor;
		skybox.SunLight.intensity = CurrentSunParam.LightIntencity;
		skybox.SunFlare.brightness = skybox.SunLight.intensity * skybox.SunFlareBrightness;
		skybox.SunFlare.enabled = !Mathf.Approximately(skybox.SunFlare.brightness, 0f);
		if (timeOfDay > _moonrise || timeOfDay < _moonset)
		{
			float num2 = ((!(_moonrise < timeOfDay)) ? (100f + timeOfDay - _moonrise) : (timeOfDay - _moonrise));
			float t2 = ((!(num2 < _moonDuration)) ? ((_moonDuration - num2) / _moonDuration) : (num2 / _moonDuration));
			float angle2 = Mathf.Lerp(_moonOrbit.x, _moonOrbit.y, t2);
			Quaternion moonRotation = Quaternion.AngleAxis(_moonLongitude - 180f, Vector3.up) * Quaternion.AngleAxis(angle2, _moonAttitudeVector);
			moonRotation.eulerAngles = new Vector3(moonRotation.eulerAngles.x, moonRotation.eulerAngles.y, 0f);
			skybox.MoonRotation = moonRotation;
		}
		CurrentMoonParam = _moonParamsList.GetParamPerTime(timeOfDay);
		skybox.MoonTint = CurrentMoonParam.TintColor;
		skybox.MoonLight.color = CurrentMoonParam.LightColor;
		skybox.MoonLight.intensity = CurrentMoonParam.LightIntencity;
		skybox.MoonFlare.brightness = skybox.MoonLight.intensity * skybox.MoonFlareBrightness;
		skybox.MoonFlare.enabled = !Mathf.Approximately(skybox.MoonFlare.brightness, 0f);
	}

	protected void OnValidate()
	{
		_sunDuration = ((!(_sunrise < _sunset)) ? (100f - _sunrise + _sunset) : (_sunset - _sunrise));
		float f = _sunAltitude * ((float)Math.PI / 180f);
		_sunAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
		_moonDuration = ((!(_moonrise < _moonset)) ? (100f - _moonrise + _moonset) : (_moonset - _moonrise));
		f = _moonAltitude * ((float)Math.PI / 180f);
		_moonAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
	}
}
