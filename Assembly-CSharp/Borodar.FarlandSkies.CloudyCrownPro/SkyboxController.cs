using Borodar.FarlandSkies.CloudyCrownPro.Helpers;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro;

[ExecuteInEditMode]
public class SkyboxController : Singleton<SkyboxController>
{
	public enum Resolution
	{
		low,
		high
	}

	[SerializeField]
	private TextureSelector textureSelector;

	[SerializeField]
	private Skybox skybox;

	public Material SkyboxMaterial;

	[Tooltip("Color at the top pole of skybox sphere")]
	[SerializeField]
	private Color _topColor = new Color(0.247f, 0.318f, 0.561f);

	[Tooltip("Color at the bottom pole of skybox sphere")]
	[SerializeField]
	private Color _bottomColor = new Color(0.773f, 0.455f, 0.682f);

	[SerializeField]
	private Color _starsTint = Color.gray;

	[SerializeField]
	[Tooltip("Reduction in stars apparent brightness closer to the horizon")]
	[Range(0f, 10f)]
	private float _starsExtinction = 2f;

	[SerializeField]
	[Tooltip("Variation in stars apparent brightness caused by the atmospheric turbulence")]
	[Range(0f, 25f)]
	private float _starsTwinklingSpeed = 4f;

	[SerializeField]
	private Light _sunLight;

	[SerializeField]
	private Color _sunTint = Color.gray;

	[SerializeField]
	[Range(0.1f, 3f)]
	private float _sunSize = 1f;

	[SerializeField]
	private bool _sunFlare = true;

	[SerializeField]
	[Range(0.01f, 2f)]
	[Tooltip("Actual flare brightness depends on sun tint alpha, and this property is just a coefficient for that value")]
	private float _sunFlareBrightness = 0.3f;

	[SerializeField]
	private Light _moonLight;

	[SerializeField]
	private Color _moonTint = Color.gray;

	[SerializeField]
	[Range(0.1f, 3f)]
	private float _moonSize = 1f;

	[SerializeField]
	private bool _moonFlare = true;

	[SerializeField]
	[Range(0.01f, 2f)]
	[Tooltip("Actual flare brightness depends on moon tint alpha, and this property is just a coefficient for that value")]
	private float _moonFlareBrightness = 0.3f;

	[Range(-0.75f, 0.75f)]
	[Tooltip("Height of the clouds relative to the horizon")]
	[SerializeField]
	private float _cloudsHeight;

	[Tooltip("Distance between the cloud waves")]
	[Range(0f, 1f)]
	[SerializeField]
	private float _cloudsOffset = 0.2f;

	[Range(-50f, 50f)]
	[Tooltip("Rotation of the clouds around the positive y axis")]
	[SerializeField]
	private float _cloudsRotationSpeed = 1f;

	[SerializeField]
	[Range(0f, 10f)]
	[Tooltip("Adjusts the brightness of the skybox")]
	private float _exposure = 1f;

	[Tooltip("Keep fog color in sync with the sky middle color automatically")]
	[SerializeField]
	private bool _adjustFogColor;

	private LensFlare _sunFlareComponent;

	private LensFlare _moonFlareComponent;

	public Color TopColor
	{
		get
		{
			return _topColor;
		}
		set
		{
			_topColor = value;
			SkyboxMaterial.SetColor("_TopColor", _topColor);
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
			SkyboxMaterial.SetColor("_BottomColor", _bottomColor);
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
			SkyboxMaterial.SetColor("_StarsTint", _starsTint);
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
			SkyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
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
			SkyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
		}
	}

	public Light SunLight
	{
		get
		{
			return _sunLight;
		}
		set
		{
			_sunLight = value;
		}
	}

	public Color SunTint
	{
		get
		{
			return _sunTint;
		}
		set
		{
			_sunTint = value;
			SkyboxMaterial.SetColor("_SunTint", _sunTint);
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
			SkyboxMaterial.SetFloat("_SunSize", _sunSize);
		}
	}

	public bool SunFlare
	{
		get
		{
			return _sunFlare;
		}
		set
		{
			_sunFlare = value;
			if ((bool)_sunFlareComponent)
			{
				_sunFlareComponent.enabled = value;
			}
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
		}
	}

	public float MoonSize
	{
		get
		{
			return _moonSize;
		}
		set
		{
			_moonSize = value;
			SkyboxMaterial.SetFloat("_MoonSize", _moonSize);
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
			SkyboxMaterial.SetColor("_MoonTint", _moonTint);
		}
	}

	public Light MoonLight
	{
		get
		{
			return _moonLight;
		}
		set
		{
			_moonLight = value;
		}
	}

	public bool MoonFlare
	{
		get
		{
			return _moonFlare;
		}
		set
		{
			_moonFlare = value;
			if ((bool)_moonFlareComponent)
			{
				_moonFlareComponent.enabled = value;
			}
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
			SkyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
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
			SkyboxMaterial.SetFloat("_CloudsOffset", _cloudsOffset);
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
			SkyboxMaterial.SetFloat("_CloudsRotationSpeed", _cloudsRotationSpeed);
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
			SkyboxMaterial.SetFloat("_Exposure", _exposure);
		}
	}

	public bool AdjustFogColor
	{
		get
		{
			return _adjustFogColor;
		}
		set
		{
			_adjustFogColor = value;
			if (_adjustFogColor)
			{
				RenderSettings.fogColor = BottomColor;
			}
		}
	}

	public void Initialize()
	{
		gameObject.SetActive(value: true);
		SkyboxMaterial = textureSelector.SetTexturesAndGetMaterial();
		skybox.material = SkyboxMaterial;
		if (_sunLight != null)
		{
			_sunFlareComponent = _sunLight.GetComponent<LensFlare>();
		}
		else
		{
			Debug.LogWarning("SkyboxController: Sun light object is not assigned.");
		}
		if (_moonLight != null)
		{
			_moonFlareComponent = _moonLight.GetComponent<LensFlare>();
		}
		else
		{
			Debug.LogWarning("SkyboxController: Moon light object is not assigned.");
		}
		if (SkyboxMaterial != null)
		{
			UpdateSkyboxProperties();
		}
		else
		{
			Debug.LogWarning("SkyboxController: Skybox material is not assigned.");
		}
	}

	protected void OnValidate()
	{
		UpdateSkyboxProperties();
	}

	protected void Update()
	{
		if (!(SkyboxMaterial == null))
		{
			SkyboxMaterial.SetMatrix("sunMatrix", _sunLight.transform.worldToLocalMatrix);
			SkyboxMaterial.SetMatrix("moonMatrix", _moonLight.transform.worldToLocalMatrix);
			if (_adjustFogColor)
			{
				RenderSettings.fogColor = _bottomColor;
			}
			if (_sunFlare && (bool)_sunFlareComponent)
			{
				_sunFlareComponent.brightness = _sunTint.a * _sunFlareBrightness;
			}
			if (_moonFlare && (bool)_moonFlareComponent)
			{
				_moonFlareComponent.brightness = _moonTint.a * _moonFlareBrightness;
			}
		}
	}

	private void UpdateSkyboxProperties()
	{
		if (!(SkyboxMaterial == null))
		{
			SkyboxMaterial.SetColor("_TopColor", _topColor);
			SkyboxMaterial.SetColor("_BottomColor", _bottomColor);
			SkyboxMaterial.SetColor("_StarsTint", _starsTint);
			SkyboxMaterial.SetFloat("_StarsExtinction", _starsExtinction);
			SkyboxMaterial.SetFloat("_StarsTwinklingSpeed", _starsTwinklingSpeed);
			SkyboxMaterial.SetFloat("_SunSize", _sunSize);
			SkyboxMaterial.SetColor("_SunTint", _sunTint);
			if ((bool)_sunFlareComponent)
			{
				_sunFlareComponent.enabled = _sunFlare;
			}
			SkyboxMaterial.SetFloat("_MoonSize", _moonSize);
			SkyboxMaterial.SetColor("_MoonTint", _moonTint);
			if ((bool)_moonFlareComponent)
			{
				_moonFlareComponent.enabled = _moonFlare;
			}
			SkyboxMaterial.SetFloat("_CloudsHeight", _cloudsHeight);
			SkyboxMaterial.SetFloat("_CloudsOffset", _cloudsOffset);
			SkyboxMaterial.SetFloat("_CloudsRotationSpeed", _cloudsRotationSpeed);
			SkyboxMaterial.SetFloat("_Exposure", _exposure);
		}
	}
}
