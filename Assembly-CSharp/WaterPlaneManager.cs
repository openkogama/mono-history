using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class WaterPlaneManager : MonoBehaviour
{
	private static float avatarHeight = 2.1f;

	public Water water;

	public Transform underwaterPlane;

	private AudioLowPassFilter lowPassFilter;

	private AudioReverbFilter reverbFilter;

	private bool audioHD;

	private Renderer underwaterPlaneRenderer;

	private List<MVWaterPlane> waterPlanes = new List<MVWaterPlane>();

	private SkyboxManager skyboxManager;

	private float localAvatarOxygen = 100f;

	public bool IsActive => waterPlanes.Count > 0;

	public List<MVWaterPlane> WaterPlanes => waterPlanes;

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
			underwaterPlaneRenderer.material.SetColor("_Color", value);
			water.Renderer.material.SetColor("_RefrColor", value);
		}
	}

	public void AddWaterPlaneLogicCube(MVWaterPlane wp)
	{
		if (!waterPlanes.Contains(wp))
		{
			waterPlanes.Add(wp);
			if (waterPlanes.Count == 1)
			{
				transform.parent = wp.Transform;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				underwaterPlane.transform.parent = Camera.main.transform;
				underwaterPlane.localPosition = new Vector3(0f, 0f, Camera.main.nearClipPlane + 0.01f);
				underwaterPlane.localRotation = Quaternion.Euler(-90f, 0f, 0f);
				underwaterPlane.gameObject.SetActive(value: true);
				water.gameObject.SetActive(value: true);
			}
		}
	}

	public void RemoveWaterPlaneLogicCube(MVWaterPlane wp)
	{
		waterPlanes.Remove(wp);
		if (waterPlanes.Count == 0)
		{
			transform.parent = null;
			underwaterPlane.transform.parent = transform;
			underwaterPlane.gameObject.SetActive(value: false);
			water.gameObject.SetActive(value: false);
		}
		else
		{
			water.gameObject.SetActive(value: true);
			transform.parent = waterPlanes[0].Transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		enabled = true;
		gameObject.SetActive(value: true);
	}

	public float ComputeAvatarWaterProximity(Vector3 position)
	{
		if (waterPlanes.Count == 0)
		{
			return 0f;
		}
		return Mathf.Clamp01((transform.position.y - position.y) / avatarHeight);
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		this.skyboxManager = MVGameControllerBase.SkyboxManager;
		SkyboxManager skyboxManager = this.skyboxManager;
		skyboxManager.OnSkyboxColorChanged = (SkyboxManager.SkyboxColorChangedDelegate)Delegate.Combine(skyboxManager.OnSkyboxColorChanged, new SkyboxManager.SkyboxColorChangedDelegate(HandleSkyboxColorChanged));
		underwaterPlane.gameObject.SetActive(value: false);
		underwaterPlaneRenderer = underwaterPlane.GetComponent<Renderer>();
		water.gameObject.SetActive(value: false);
		lowPassFilter = Camera.main.GetComponent<AudioLowPassFilter>();
		reverbFilter = Camera.main.GetComponent<AudioReverbFilter>();
	}

	private void OnEnable()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Combine(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(HandleQualityChanged));
	}

	private void OnDisable()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Remove(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(HandleQualityChanged));
	}

	private void FixedUpdate()
	{
		if (waterPlanes.Count != 0)
		{
		}
	}

	private void Update()
	{
		if (waterPlanes.Count == 0)
		{
			return;
		}
		bool flag = Camera.main.transform.position.y < transform.position.y;
		underwaterPlaneRenderer.enabled = flag;
		if (underwaterPlaneRenderer.enabled)
		{
			underwaterPlaneRenderer.material.SetFloat("_WaterY", transform.position.y);
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
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		if (avatarLocal == null)
		{
			return;
		}
		Vector3 position = avatarLocal.GameObject.transform.position;
		position.y = transform.position.y;
		water.transform.position = position;
		bool flag2 = avatarLocal.GameObject.transform.position.y < transform.position.y;
		MVInteractableBase component = avatarLocal.GameObject.GetComponent<MVInteractableBase>();
		InteractionDataHandlerBase interactionDataHandlerBase = avatarLocal.InteractionDataHandlerBase;
		if (!interactionDataHandlerBase.enabled || component == null)
		{
			return;
		}
		if (flag2)
		{
			component.AddModifier(AvatarModifierPackageType.Underwater, -1, new AvatarModifierPackage.AvatarModifier[2]
			{
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, UnderwaterModifierCallback),
				new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, UnderwaterJumpPowerModifierCallback)
			});
			if (waterPlanes.Count > 0 && waterPlanes[0].Data.ContainsKey("avatarModifierPackageType"))
			{
				AvatarModifierPackageType avatarModifierPackageType = (AvatarModifierPackageType)(int)waterPlanes[0].Data["avatarModifierPackageType"];
				if (avatarModifierPackageType != AvatarModifierPackageType.None)
				{
					AvatarModifierPackage package = AvatarModifierPackageFactory.GetPackage(avatarModifierPackageType);
					component.AddModifier(avatarModifierPackageType, package.id, package.avatarModifiers);
				}
			}
		}
		else if (component.HasModifier(AvatarModifierPackageType.Underwater))
		{
			component.RemoveModifier(AvatarModifierPackageType.Underwater);
		}
		float num = ComputeAvatarWaterProximity(avatarLocal.GameObject.transform.position);
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
			component.TakeDamage(5f * Time.deltaTime, null, PlayerKilledByType.Environmental);
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
