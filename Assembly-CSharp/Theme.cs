using System.Collections.Generic;
using ThemeSettings;
using UnityEngine;

public abstract class Theme : MonoBehaviour
{
	[SerializeField]
	private bool overrideSkyboxManager;

	[SerializeField]
	private RectTransform themeButtonImagePrefab;

	private List<ThemeComponent> components = new List<ThemeComponent>();

	public bool OverrideSkyboxManager => overrideSkyboxManager;

	public RectTransform ThemeButtonImagePrefab => themeButtonImagePrefab;

	public abstract string Identifier { get; }

	public abstract string DisplayName { get; }

	public abstract string Description { get; }

	public SettingsWrapper Settings { get; private set; }

	public virtual List<RectTransform> Controllers => new List<RectTransform>(0);

	protected abstract void InitializeComponents();

	protected abstract void InitializeAttributes();

	public void InitializeForPreview()
	{
		Settings = new SettingsPreview();
		Initialize();
	}

	public void Initialize(int woid)
	{
		Settings = new SettingsSerialized(woid);
		Initialize();
	}

	private void Initialize()
	{
		InitializeAttributes();
		Settings.Initialize();
		InitializeComponents();
	}

	public void Activate()
	{
		if (overrideSkyboxManager)
		{
			MVGameControllerBase.SkyboxManager.enabled = false;
		}
		foreach (ThemeComponent component in components)
		{
			component.Activate();
		}
	}

	public void Deactivate()
	{
		if (overrideSkyboxManager)
		{
			MVGameControllerBase.SkyboxManager.enabled = true;
		}
		foreach (ThemeComponent component in components)
		{
			component.Deactivate();
		}
	}

	public void Add(ThemeComponent component)
	{
		components.Add(component);
	}

	public void Purchase(int id)
	{
		MVGameControllerBase.OperationRequests.PurchaseSwitchTheme(id, Settings.Data);
	}

	public virtual void ThemeReset()
	{
	}
}
