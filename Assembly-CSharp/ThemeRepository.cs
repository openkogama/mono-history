using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ThemeRepository : ScriptableObject
{
	[SerializeField]
	private List<Theme> themePrefabs;

	private Dictionary<string, Theme> IdentifierToTheme = new Dictionary<string, Theme>();

	public static ThemeRepository Instance { get; private set; }

	public int CurrentThemeWoid => (!ThemeIsActive) ? (-1) : CurrentTheme.Id;

	public Theme CurrentThemeVisualization => CurrentTheme?.Visualization;

	public string CurrentThemeIdentifier => (CurrentTheme == null) ? null : CurrentTheme.Identifier;

	public bool SkyboxOverride
	{
		get
		{
			Theme currentThemeVisualization = CurrentThemeVisualization;
			return currentThemeVisualization != null && currentThemeVisualization.OverrideSkyboxManager;
		}
	}

	public bool ThemeIsActive => CurrentTheme != null;

	private ThemeWorldObject CurrentTheme
	{
		get
		{
			List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.Theme);
			return (worldObjectsByType.Count <= 0) ? null : ((ThemeWorldObject)MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.Theme)[0]);
		}
	}

	public void Initialize()
	{
		Instance = this;
		foreach (Theme themePrefab in themePrefabs)
		{
			IdentifierToTheme[themePrefab.Identifier] = themePrefab;
		}
	}

	public Theme GetThemePrefab(string identifier)
	{
		if (IdentifierToTheme.ContainsKey(identifier))
		{
			return IdentifierToTheme[identifier];
		}
		Debug.Log(identifier + " is not present in theme repository.");
		Debug.LogError("Theme is missing");
		return themePrefabs[0];
	}

	public Theme CreateTemporaryThemeVisualization(string identifier)
	{
		Debug.Log("Preview theme created");
		if (CurrentThemeVisualization != null)
		{
			CurrentThemeVisualization.Deactivate();
		}
		Theme theme = Object.Instantiate(GetThemePrefab(identifier));
		theme.InitializeForPreview();
		theme.Activate();
		return theme;
	}

	public void DestroyTemporary(Theme theme)
	{
		Debug.Log("Preview theme destroyed");
		theme.Deactivate();
		Object.Destroy(theme.gameObject);
		if (CurrentThemeVisualization != null)
		{
			CurrentThemeVisualization.Activate();
		}
	}
}
