using System;
using MV.Common;
using UnityEngine;

public class WaterPlaneManager : MonoBehaviour
{
	private const float avatarHeight = 2.1f;

	[SerializeField]
	private Water water;

	[SerializeField]
	private Transform underwaterCameraPlane;

	private Renderer underwaterCameraPlaneRenderer;

	private AudioLowPassFilter lowPassFilter;

	private AudioReverbFilter reverbFilter;

	private bool audioHD;

	private MVWaterPlane waterPlaneLogicCube;

	private SkyboxManager skyboxManager;

	private float localAvatarOxygen = 100f;

	private AvatarModifierPackage.AvatarModifier[] additionalUnderWaterModifiers;

	public bool IsActive => waterPlaneLogicCube != null;

	private Color HorizonColor
	{
		get
		{
			return water.Renderer.material.GetColor("_HorizonColor");
		}
		set
		{
			water.Renderer.material.SetColor("_HorizonColor", value);
		}
	}

	public Color WaterColor
	{
		get
		{
			return water.Renderer.material.GetColor("_RefrColor");
		}
		set
		{
			underwaterCameraPlaneRenderer.material.SetColor("_Color", value);
			water.Renderer.material.SetColor("_RefrColor", value);
		}
	}

	public void AddWaterPlaneLogicCube(MVWaterPlane logicCube)
	{
		if (waterPlaneLogicCube != null)
		{
			DebugLogHandler.ReportError("Added water plane to manager twice.", string.Empty, LogType.Error);
		}
		waterPlaneLogicCube = logicCube;
		transform.SetParent(logicCube.Transform, worldPositionStays: false);
		underwaterCameraPlane.transform.parent = Camera.main.transform;
		underwaterCameraPlane.localPosition = new Vector3(0f, 0f, Camera.main.nearClipPlane + 0.01f);
		underwaterCameraPlane.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		underwaterCameraPlane.gameObject.SetActive(value: true);
		water.gameObject.SetActive(value: true);
	}

	public void RemoveWaterPlaneLogicCube(MVWaterPlane wp)
	{
		waterPlaneLogicCube = null;
		transform.parent = null;
		underwaterCameraPlane.transform.parent = transform;
		underwaterCameraPlane.gameObject.SetActive(value: false);
		water.gameObject.SetActive(value: false);
		enabled = true;
		gameObject.SetActive(value: true);
	}

	public float ComputeAvatarWaterProximity(Vector3 position)
	{
		if (waterPlaneLogicCube == null)
		{
			return 0f;
		}
		return Mathf.Clamp01((transform.position.y - position.y) / 2.1f);
	}

	protected void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		additionalUnderWaterModifiers = new AvatarModifierPackage.AvatarModifier[2]
		{
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, UnderwaterModifierCallback),
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, UnderwaterJumpPowerModifierCallback)
		};
	}

	protected void Start()
	{
		this.skyboxManager = MVGameControllerBase.SkyboxManager;
		SkyboxManager skyboxManager = this.skyboxManager;
		skyboxManager.OnSkyboxColorChanged = (SkyboxManager.SkyboxColorChangedDelegate)Delegate.Combine(skyboxManager.OnSkyboxColorChanged, new SkyboxManager.SkyboxColorChangedDelegate(HandleSkyboxColorChanged));
		underwaterCameraPlane.gameObject.SetActive(value: false);
		underwaterCameraPlaneRenderer = underwaterCameraPlane.GetComponent<Renderer>();
		water.gameObject.SetActive(value: false);
		lowPassFilter = Camera.main.GetComponent<AudioLowPassFilter>();
		reverbFilter = Camera.main.GetComponent<AudioReverbFilter>();
	}

	protected void OnEnable()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Combine(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(HandleQualityChanged));
	}

	protected void OnDisable()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Remove(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(HandleQualityChanged));
	}

	protected void Update()
	{
		if (waterPlaneLogicCube == null)
		{
			return;
		}
		UpdateUnderwaterCameraEffects();
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		if (avatarLocal == null)
		{
			return;
		}
		Vector3 position = avatarLocal.GameObject.transform.position;
		position.y = transform.position.y;
		water.transform.position = position;
		if (avatarLocal.InteractionDataHandlerBase.enabled)
		{
			MVInteractableBase component = avatarLocal.GameObject.GetComponent<MVInteractableBase>();
			if (component != null)
			{
				UpdateLocalAvatarModifers(avatarLocal, component);
				UpdateLocalAvatarOxygen(avatarLocal, component);
			}
		}
	}

	private void UpdateUnderwaterCameraEffects()
	{
		bool flag = Camera.main.transform.position.y < transform.position.y;
		underwaterCameraPlaneRenderer.enabled = flag;
		if (underwaterCameraPlaneRenderer.enabled)
		{
			underwaterCameraPlaneRenderer.material.SetFloat("_WaterY", transform.position.y);
		}
		if (lowPassFilter == null)
		{
			lowPassFilter = Camera.main.GetComponent<AudioLowPassFilter>();
		}
		if (flag && !lowPassFilter.enabled)
		{
			lowPassFilter.enabled = true;
			if (audioHD)
			{
				reverbFilter.enabled = true;
			}
		}
		else if (!flag && lowPassFilter.enabled)
		{
			lowPassFilter.enabled = false;
			reverbFilter.enabled = false;
		}
	}

	private void UpdateLocalAvatarModifers(MVAvatarLocal avatar, MVInteractableBase avatarInteractable)
	{
		if (avatar.GameObject.transform.position.y < transform.position.y)
		{
			avatarInteractable.AddModifier(AvatarModifierPackageType.Underwater, -1, additionalUnderWaterModifiers);
			if (waterPlaneLogicCube != null && waterPlaneLogicCube.Data.ContainsKey("avatarModifierPackageType"))
			{
				AvatarModifierPackageType avatarModifierPackageType = (AvatarModifierPackageType)(int)waterPlaneLogicCube.Data["avatarModifierPackageType"];
				if (avatarModifierPackageType != AvatarModifierPackageType.None)
				{
					AvatarModifierPackage package = AvatarModifierPackageFactory.GetPackage(avatarModifierPackageType);
					avatarInteractable.AddModifier(avatarModifierPackageType, package.id, package.avatarModifiers);
				}
			}
		}
		else if (avatarInteractable.HasModifier(AvatarModifierPackageType.Underwater))
		{
			avatarInteractable.RemoveModifier(AvatarModifierPackageType.Underwater);
		}
	}

	private void UpdateLocalAvatarOxygen(MVAvatarLocal avatar, MVInteractableBase avatarInteractable)
	{
		float num = ComputeAvatarWaterProximity(avatar.GameObject.transform.position);
		if (num >= 0.6f)
		{
			localAvatarOxygen = Mathf.Max(0f, localAvatarOxygen - Time.deltaTime * 5f);
		}
		else
		{
			localAvatarOxygen = Mathf.Min(100f, localAvatarOxygen + Time.deltaTime * 25f);
		}
		if (localAvatarOxygen <= 0f)
		{
			avatarInteractable.TakeDamage(5f * Time.deltaTime, null, PlayerKilledByType.Environmental);
		}
	}

	private float UnderwaterJumpPowerModifierCallback()
	{
		float num = ComputeAvatarWaterProximity(MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position);
		return (!((double)num > 0.7)) ? 1f : 2f;
	}

	private float UnderwaterModifierCallback()
	{
		float num = 1f - ComputeAvatarWaterProximity(MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform.position);
		return num * 0.3f + 0.7f;
	}

	private void HandleSkyboxColorChanged(Color newColor)
	{
		HorizonColor = newColor;
	}

	private void HandleQualityChanged(int level)
	{
		switch (level)
		{
		case 0:
		case 2:
			water.m_WaterMode = Water.WaterMode.Simple;
			audioHD = false;
			break;
		case 1:
			water.m_WaterMode = Water.WaterMode.Reflective;
			audioHD = true;
			break;
		}
	}
}
