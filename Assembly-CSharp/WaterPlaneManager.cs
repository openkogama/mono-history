using System;
using UnityEngine;

public class WaterPlaneManager : MonoBehaviour
{
	private const float avatarHeight = 2.1f;

	[SerializeField]
	private Water water;

	[SerializeField]
	private Transform underwaterCameraPlane;

	[SerializeField]
	private SplashController splashController;

	private Renderer underwaterCameraPlaneRenderer;

	private AudioLowPassFilter lowPassFilter;

	private AudioReverbFilter reverbFilter;

	private bool audioHD;

	private MVWaterPlane waterPlaneLogicCube;

	private SkyboxManager skyboxManager;

	private Camera mainCamera;

	public SplashController Splash => splashController;

	public bool IsActive => water.gameObject.activeInHierarchy;

	public float WaterLevel => transform.position.y;

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

	protected void Awake()
	{
		Splash.Initialize();
	}

	protected void OnDestroy()
	{
		Splash.Destroy();
	}

	protected void Start()
	{
		this.skyboxManager = MVGameControllerBase.SkyboxManager;
		SkyboxManager skyboxManager = this.skyboxManager;
		skyboxManager.OnSkyboxColorChanged = (SkyboxManager.SkyboxColorChangedDelegate)Delegate.Combine(skyboxManager.OnSkyboxColorChanged, new SkyboxManager.SkyboxColorChangedDelegate(HandleSkyboxColorChanged));
		underwaterCameraPlane.gameObject.SetActive(value: false);
		underwaterCameraPlaneRenderer = underwaterCameraPlane.GetComponent<Renderer>();
		water.gameObject.SetActive(value: false);
		mainCamera = Camera.main;
		lowPassFilter = mainCamera.GetComponent<AudioLowPassFilter>();
		reverbFilter = mainCamera.GetComponent<AudioReverbFilter>();
	}

	protected void Update()
	{
		if (waterPlaneLogicCube != null)
		{
			UpdateUnderwaterCameraEffects();
			Splash.CleanUpInactiveObjectIDs();
			if (MVGameControllerBase.Game.LocalPlayer.IsReady)
			{
				water.transform.position = MVGameControllerBase.MainCameraManager.transform.position;
				Vector3 localPosition = water.transform.localPosition;
				localPosition.y = 0f;
				water.transform.localPosition = localPosition;
			}
		}
	}

	protected void OnEnable()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Combine(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(HandleQualityChanged));
	}

	protected void OnDisable()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Remove(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(HandleQualityChanged));
	}

	public void AddWaterPlaneLogicCube(MVWaterPlane logicCube)
	{
		if (waterPlaneLogicCube != null)
		{
			Debug.LogError("Added water plane to manager twice.");
		}
		waterPlaneLogicCube = logicCube;
		transform.SetParent(logicCube.Transform, worldPositionStays: false);
		underwaterCameraPlane.transform.SetParent(mainCamera.transform, worldPositionStays: false);
		underwaterCameraPlane.localPosition = new Vector3(0f, 0f, mainCamera.nearClipPlane + 0.01f);
		underwaterCameraPlane.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		underwaterCameraPlane.gameObject.SetActive(value: true);
		water.gameObject.SetActive(value: true);
	}

	public void RemoveWaterPlaneLogicCube(MVWaterPlane wp)
	{
		waterPlaneLogicCube = null;
		transform.SetParent(null, worldPositionStays: false);
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

	public float GetHeightAboveWaterLevel(Vector3 position)
	{
		return GetHeightAboveWaterLevel(position.y);
	}

	public float GetHeightAboveWaterLevel(float altitude)
	{
		return altitude - WaterLevel;
	}

	private void UpdateUnderwaterCameraEffects()
	{
		bool flag = mainCamera.transform.position.y < transform.position.y;
		underwaterCameraPlaneRenderer.enabled = flag;
		if (underwaterCameraPlaneRenderer.enabled)
		{
			underwaterCameraPlaneRenderer.material.SetFloat("_WaterY", transform.position.y);
		}
		if (lowPassFilter == null)
		{
			lowPassFilter = mainCamera.GetComponent<AudioLowPassFilter>();
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
