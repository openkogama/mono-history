using System;
using UnityEngine;

public class ThemeSkybox : ThemeComponent
{
	[Serializable]
	private class PlatformSpecificMaterial
	{
		[SerializeField]
		private Material skyboxMaterialAndroid;

		[SerializeField]
		private Material skyboxMaterialStandalone;

		[SerializeField]
		private Material skyboxMaterialWebGL;

		public static implicit operator Material(PlatformSpecificMaterial m)
		{
			return m.skyboxMaterialStandalone;
		}
	}

	[SerializeField]
	private PlatformSpecificMaterial skyboxMaterialSerialized;

	private Material skyboxMaterial;

	[SerializeField]
	[Tooltip("Color at the top pole of skybox sphere")]
	private Color _topColor = new Color(0.247f, 0.318f, 0.561f);

	[Tooltip("Color at the bottom pole of skybox sphere")]
	[SerializeField]
	private Color _bottomColor = new Color(0.773f, 0.455f, 0.682f);

	[SerializeField]
	private bool fogEnabled;

	[Range(0f, 0.5f)]
	[SerializeField]
	private float fogDensity;

	[SerializeField]
	private float fogStartDist = 10f;

	[SerializeField]
	private float fogEndDist = 100f;

	[SerializeField]
	private Color _starsTint = Color.gray;

	[Range(0f, 10f)]
	[Tooltip("Reduction in stars apparent brightness closer to the horizon")]
	[SerializeField]
	private float _starsExtinction = 2f;

	[Range(0f, 25f)]
	[Tooltip("Variation in stars apparent brightness caused by the atmospheric turbulence")]
	[SerializeField]
	private float _starsTwinklingSpeed = 4f;

	[SerializeField]
	private FlareLight _sun;

	[SerializeField]
	private Color _sunTint = Color.gray;

	[Range(0.1f, 3f)]
	[SerializeField]
	private float _sunSize = 1f;

	[Range(0.01f, 2f)]
	[Tooltip("Actual flare brightness depends on sun tint alpha, and this property is just a coefficient for that value")]
	[SerializeField]
	private float _sunFlareBrightness = 0.3f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _sunLightContrast = 0.5f;

	[SerializeField]
	[Range(0f, 8f)]
	private float _sunLightIntensity = 1f;

	[SerializeField]
	private FlareLight _moon;

	[SerializeField]
	private Color _moonTint = Color.gray;

	[SerializeField]
	[Range(0.1f, 3f)]
	private float _moonSize = 1f;

	[Range(0.01f, 2f)]
	[Tooltip("Actual flare brightness depends on moon tint alpha, and this property is just a coefficient for that value")]
	[SerializeField]
	private float _moonFlareBrightness = 0.3f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _moonLightContrast = 0.5f;

	[SerializeField]
	[Range(0f, 8f)]
	private float _moonLightIntensity = 1f;

	[Tooltip("Height of the clouds relative to the horizon")]
	[SerializeField]
	[Range(-0.75f, 0.75f)]
	private float _cloudsHeight;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("Distance between the cloud waves")]
	private float _cloudsOffset = 0.2f;

	[Range(-50f, 50f)]
	[Tooltip("Rotation of the clouds around the positive y axis")]
	[SerializeField]
	private float _cloudsRotationSpeed = 1f;

	[Range(0f, 10f)]
	[Tooltip("Adjusts the brightness of the skybox")]
	[SerializeField]
	private float _exposure = 1f;

	[SerializeField]
	private AnimationCurve lightIntensityByEmitterHeight;

	private Material previousSkyboxMaterial;

	private CameraClearFlags previousClearFlags;

	private float sunAngle;

	private float moonAngle;

	public Material Material => skyboxMaterial;

	public Color TopColor
	{
		get
		{
			return _topColor;
		}
		set
		{
			_topColor = value;
			skyboxMaterial.SetColor("_TopColor", _topColor);
		}
	}

	public Color BottomColor
	{
		get
		{
			return _bottomColor;
		}
		set
		{
			_bottomColor = value;
			skyboxMaterial.SetColor("_BottomColor", _bottomColor);
			RecalculateFogColor();
			RenderSettings.ambientLight = _bottomColor;
		}
	}

	public bool FogEnabled
	{
		get
		{
			return fogEnabled;
		}
		set
		{
			fogEnabled = value;
			RenderSettings.fog = value;
		}
	}

	public float FogDensity
	{
		get
		{
			return fogDensity;
		}
		set
		{
			fogDensity = value;
			RenderSettings.fogDensity = value;
		}
	}

	public float FogStartDistance
	{
		get
		{
			return fogStartDist;
		}
		set
		{
			fogStartDist = value;
			RenderSettings.fogStartDistance = value;
		}
	}

	public float FogEndDistance
	{
		get
		{
			return fogEndDist;
		}
		set
		{
			fogEndDist = value;
			RenderSettings.fogEndDistance = value;
		}
	}

	public Color StarsTint
	{
		get
		{
			return _starsTint;
		}
		set
		{
			_starsTint = value;
			skyboxMaterial.SetColor("_StarsTint", _starsTint);
		}
	}

	public float StarsExtinction
	{
		get
		{
			return _starsExtinction;
		}
		set
		{
			_starsExtinction = value;
			skyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
		}
	}

	public float StarsTwinklingSpeed
	{
		get
		{
			return _starsTwinklingSpeed;
		}
		set
		{
			_starsTwinklingSpeed = value;
			skyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
		}
	}

	public Light SunLight => _sun.Light;

	public LensFlare SunFlare => _sun.LensFlare;

	public Color SunTint
	{
		get
		{
			return _sunTint;
		}
		set
		{
			_sunTint = value;
			skyboxMaterial.SetColor("_SunTint", _sunTint);
		}
	}

	public float SunSize
	{
		get
		{
			return _sunSize;
		}
		set
		{
			_sunSize = value;
			skyboxMaterial.SetFloat("_SunSize", _sunSize);
		}
	}

	public float SunLightContrast
	{
		get
		{
			return _sunLightContrast;
		}
		set
		{
			_sunLightContrast = value;
			RecalculateSunLight();
		}
	}

	public float SunLightIntensity
	{
		get
		{
			return _sunLightIntensity;
		}
		set
		{
			_sunLightIntensity = value;
			RecalculateSunLight();
		}
	}

	public float SunFlareBrightness
	{
		get
		{
			return _sunFlareBrightness;
		}
		set
		{
			_sunFlareBrightness = value;
			RecalculateSunLight();
		}
	}

	public float SunHeight
	{
		set
		{
			sunAngle = Mathf.Clamp(value * 90f, -90f, 90f);
			Vector3 localEulerAngles = _sun.transform.localEulerAngles;
			localEulerAngles.x = sunAngle;
			_sun.transform.localEulerAngles = localEulerAngles;
			skyboxMaterial.SetMatrix("sunMatrix", _sun.transform.worldToLocalMatrix);
		}
	}

	public float SunAxisDegrees
	{
		set
		{
			Vector3 localEulerAngles = _sun.transform.localEulerAngles;
			localEulerAngles.y = value;
			_sun.transform.localEulerAngles = localEulerAngles;
			skyboxMaterial.SetMatrix("sunMatrix", _sun.transform.worldToLocalMatrix);
		}
	}

	public Quaternion SunRotation
	{
		get
		{
			return _sun.transform.rotation;
		}
		set
		{
			_sun.transform.rotation = value;
			skyboxMaterial.SetMatrix("sunMatrix", _sun.transform.worldToLocalMatrix);
		}
	}

	public Light MoonLight => _moon.Light;

	public LensFlare MoonFlare => _moon.LensFlare;

	public float MoonSize
	{
		get
		{
			return _moonSize;
		}
		set
		{
			_moonSize = value;
			skyboxMaterial.SetFloat("_MoonSize", _moonSize);
		}
	}

	public Color MoonTint
	{
		get
		{
			return _moonTint;
		}
		set
		{
			_moonTint = value;
			skyboxMaterial.SetColor("_MoonTint", _moonTint);
		}
	}

	public float MoonLightContrast
	{
		get
		{
			return _moonLightContrast;
		}
		set
		{
			_moonLightContrast = value;
			RecalculateMoonLight();
		}
	}

	public float MoonLightIntensity
	{
		get
		{
			return _moonLightIntensity;
		}
		set
		{
			_moonLightIntensity = value;
			RecalculateMoonLight();
		}
	}

	public float MoonFlareBrightness
	{
		get
		{
			return _moonFlareBrightness;
		}
		set
		{
			_moonFlareBrightness = value;
			MoonFlare.brightness = MoonLight.intensity * value;
		}
	}

	public float MoonHeight
	{
		set
		{
			moonAngle = Mathf.Clamp(value * 90f, -90f, 90f);
			Vector3 localEulerAngles = _moon.transform.localEulerAngles;
			localEulerAngles.x = moonAngle;
			_moon.transform.localEulerAngles = localEulerAngles;
			skyboxMaterial.SetMatrix("moonMatrix", _moon.transform.worldToLocalMatrix);
		}
	}

	public float MoonAxisDegrees
	{
		set
		{
			Vector3 localEulerAngles = _moon.transform.localEulerAngles;
			localEulerAngles.y = value;
			_moon.transform.localEulerAngles = localEulerAngles;
			skyboxMaterial.SetMatrix("moonMatrix", _moon.transform.worldToLocalMatrix);
		}
	}

	public Quaternion MoonRotation
	{
		get
		{
			return _sun.transform.rotation;
		}
		set
		{
			_moon.transform.rotation = value;
			skyboxMaterial.SetMatrix("moonMatrix", _moon.transform.worldToLocalMatrix);
		}
	}

	public float CloudsHeight
	{
		get
		{
			return _cloudsHeight;
		}
		set
		{
			_cloudsHeight = value;
			skyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
			RecalculateMoonLight();
			RecalculateSunLight();
		}
	}

	public float CloudsOffset
	{
		get
		{
			return _cloudsOffset;
		}
		set
		{
			_cloudsOffset = value;
			skyboxMaterial.SetFloat("_CloudsOffset", _cloudsOffset);
		}
	}

	public float CloudsRotationSpeed
	{
		get
		{
			return _cloudsRotationSpeed;
		}
		set
		{
			_cloudsRotationSpeed = value;
			skyboxMaterial.SetFloat("_CloudsRotationSpeed", _cloudsRotationSpeed);
		}
	}

	public float Exposure
	{
		get
		{
			return _exposure;
		}
		set
		{
			_exposure = value;
			skyboxMaterial.SetFloat("_Exposure", _exposure);
			RenderSettings.ambientIntensity = _exposure;
			RecalculateFogColor();
		}
	}

	private Skybox Skybox => MVGameControllerBase.CameraController.Skybox;

	private Camera Camera => MVGameControllerBase.CameraController.MainCamera;

	protected void Awake()
	{
		skyboxMaterial = UnityEngine.Object.Instantiate((Material)skyboxMaterialSerialized);
	}

	public override void Activate()
	{
		previousSkyboxMaterial = Skybox.material;
		Skybox.material = skyboxMaterial;
		skyboxMaterial.SetMatrix("sunMatrix", _sun.Light.transform.worldToLocalMatrix);
		skyboxMaterial.SetMatrix("moonMatrix", _moon.Light.transform.worldToLocalMatrix);
		previousClearFlags = Camera.clearFlags;
		Camera.clearFlags = CameraClearFlags.Skybox;
		_sun.enabled = true;
		_moon.enabled = true;
		ApplyRenderSettings();
	}

	public override void Deactivate()
	{
		Skybox.material = previousSkyboxMaterial;
		Camera.clearFlags = previousClearFlags;
		_sun.enabled = false;
		_moon.enabled = false;
		SkyboxManager.ResetAmbientLight();
	}

	public void RecalculateSunLight()
	{
		SunLight.color = _sunTint * _sunLightContrast + Color.white * (1f - _sunLightContrast);
		float time = sunAngle / 90f - _cloudsHeight;
		SunLight.intensity = lightIntensityByEmitterHeight.Evaluate(time) * _sunLightIntensity;
		SunFlare.brightness = SunLight.intensity * _sunFlareBrightness;
		SunFlare.enabled = !Mathf.Approximately(SunFlare.brightness, 0f);
	}

	public void RecalculateMoonLight()
	{
		MoonLight.color = _moonTint * _moonLightContrast + Color.white * (1f - _moonLightContrast);
		float time = moonAngle / 90f - _cloudsHeight;
		MoonLight.intensity = lightIntensityByEmitterHeight.Evaluate(time) * _moonLightIntensity;
		MoonFlare.brightness = MoonLight.intensity * _moonFlareBrightness;
		MoonFlare.enabled = !Mathf.Approximately(MoonFlare.brightness, 0f);
	}

	public void RecalculateFogColor()
	{
		Color fogColor = _bottomColor * _exposure;
		fogColor.a = 1f;
		RenderSettings.fogColor = fogColor;
	}

	private void ApplyRenderSettings()
	{
		RenderSettings.fog = fogEnabled;
		RenderSettings.fogMode = FogMode.ExponentialSquared;
		RenderSettings.fogStartDistance = fogStartDist;
		RenderSettings.fogEndDistance = fogEndDist;
		RenderSettings.fogDensity = fogDensity;
		RecalculateFogColor();
		RenderSettings.ambientLight = _bottomColor;
		RenderSettings.ambientIntensity = _exposure;
	}
}
