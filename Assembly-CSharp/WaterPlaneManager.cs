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

	private List<MVWaterPlane> waterPlanes = new List<MVWaterPlane>();

	private SkyboxManager skyboxManager;

	private float localAvatarOxygen = 100f;

	public bool IsActive => waterPlanes.Count > 0;

	public List<MVWaterPlane> WaterPlanes => waterPlanes;

	private Color HorizonColor
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)water).renderer.material.GetColor("_HorizonColor");
		}
		set
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			((Component)water).renderer.material.SetColor("_HorizonColor", value);
		}
	}

	public Color WaterColor
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)water).renderer.material.GetColor("_RefrColor");
		}
		set
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			((Component)underwaterPlane).renderer.material.SetColor("_Color", value);
			((Component)water).renderer.material.SetColor("_RefrColor", value);
		}
	}

	public void AddWaterPlaneLogicCube(MVWaterPlane wp)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!waterPlanes.Contains(wp))
		{
			waterPlanes.Add(wp);
			if (waterPlanes.Count == 1)
			{
				((Component)this).transform.parent = wp.Transform;
				((Component)this).transform.localPosition = Vector3.zero;
				((Component)this).transform.localRotation = Quaternion.identity;
				((Component)underwaterPlane).transform.parent = ((Component)Camera.main).transform;
				underwaterPlane.localPosition = new Vector3(0f, 0f, Camera.main.nearClipPlane + 0.01f);
				underwaterPlane.localRotation = Quaternion.Euler(-90f, 0f, 0f);
				((Component)underwaterPlane).gameObject.active = true;
				((Component)water).gameObject.active = true;
			}
		}
	}

	public void RemoveWaterPlaneLogicCube(MVWaterPlane wp)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Removing water plane");
		waterPlanes.Remove(wp);
		if (waterPlanes.Count == 0)
		{
			((Component)this).transform.parent = null;
			((Component)underwaterPlane).transform.parent = ((Component)this).transform;
			((Component)underwaterPlane).gameObject.active = false;
			((Component)water).gameObject.active = false;
		}
		else
		{
			((Component)water).gameObject.active = true;
			((Component)this).transform.parent = waterPlanes[0].Transform;
			((Component)this).transform.localPosition = Vector3.zero;
			((Component)this).transform.localRotation = Quaternion.identity;
		}
		((Behaviour)this).enabled = true;
		((Component)this).gameObject.active = true;
	}

	public float ComputeAvatarWaterProximity(Vector3 position)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (waterPlanes.Count == 0)
		{
			return 0f;
		}
		return Mathf.Clamp01((((Component)this).transform.position.y - position.y) / avatarHeight);
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Start()
	{
		this.skyboxManager = Object.FindObjectOfType(typeof(SkyboxManager)) as SkyboxManager;
		SkyboxManager skyboxManager = this.skyboxManager;
		skyboxManager.OnSkyboxColorChanged = (SkyboxManager.SkyboxColorChangedDelegate)Delegate.Combine(skyboxManager.OnSkyboxColorChanged, new SkyboxManager.SkyboxColorChangedDelegate(HandleSkyboxColorChanged));
		((Component)underwaterPlane).gameObject.active = false;
		((Component)water).gameObject.active = false;
		lowPassFilter = ((Component)Camera.main).GetComponent<AudioLowPassFilter>();
		reverbFilter = ((Component)Camera.main).GetComponent<AudioReverbFilter>();
		MVGameController.Instance.WOCM.WaterPlaneManager = this;
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		if (waterPlanes.Count == 0)
		{
			return;
		}
		bool flag = ((Component)Camera.main).transform.position.y < ((Component)this).transform.position.y;
		((Component)underwaterPlane).renderer.enabled = flag;
		if (((Component)underwaterPlane).renderer.enabled)
		{
			((Component)underwaterPlane).renderer.material.SetFloat("_WaterY", ((Component)this).transform.position.y);
		}
		if ((Object)(object)lowPassFilter == (Object)null)
		{
			lowPassFilter = ((Component)Camera.main).GetComponent<AudioLowPassFilter>();
		}
		if (flag && !((Behaviour)lowPassFilter).enabled)
		{
			((Behaviour)lowPassFilter).enabled = true;
			if (audioHD)
			{
				((Behaviour)reverbFilter).enabled = true;
			}
		}
		else if (!flag && ((Behaviour)lowPassFilter).enabled)
		{
			((Behaviour)lowPassFilter).enabled = false;
			((Behaviour)reverbFilter).enabled = false;
		}
		MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
		if (avatarLocal == null)
		{
			return;
		}
		Vector3 position = avatarLocal.GameObject.transform.position;
		position.y = ((Component)this).transform.position.y;
		((Component)water).transform.position = position;
		bool flag2 = avatarLocal.GameObject.transform.position.y < ((Component)this).transform.position.y;
		MVInteractableBase component = avatarLocal.GameObject.GetComponent<MVInteractableBase>();
		if ((Object)(object)component == (Object)null)
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
		avatarLocal.HealthAndOxygenBar.Oxygen = ((!flag2 && !(localAvatarOxygen < 100f)) ? 0f : localAvatarOxygen);
		if (localAvatarOxygen <= 0f)
		{
			component.TakeDamage(5f * Time.deltaTime, null, PlayerKilledByType.Environmental);
		}
	}

	private float UnderwaterJumpPowerModifierCallback()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float num = ComputeAvatarWaterProximity(MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position);
		return (!((double)num > 0.7)) ? 1f : 2f;
	}

	private float UnderwaterModifierCallback()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - ComputeAvatarWaterProximity(MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position);
		return num * 0.3f + 0.7f;
	}

	private void HandleSkyboxColorChanged(Color newColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		HorizonColor = newColor;
	}

	private void HandleQualityChanged(int level)
	{
		switch (level)
		{
		case 0:
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
