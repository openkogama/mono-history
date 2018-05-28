using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ThemeRepository : ScriptableObject
{
	[SerializeField]
	private List<Theme> themePrefabs;

	private Dictionary<string, Theme> IdentifierToTheme = new Dictionary<string, Theme>();

	private Theme currentlySerializedTheme;

	public static ThemeRepository Instance { get; private set; }

	public Theme CurrentlySerializedTheme
	{
		get
		{
			return currentlySerializedTheme;
		}
		private set
		{
			currentlySerializedTheme = value;
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

	public Theme CreatePreviewTheme(string identifier)
	{
		Debug.Log("Preview theme created");
		if (CurrentlySerializedTheme != null)
		{
			CurrentlySerializedTheme.Deactivate();
		}
		Theme theme = Object.Instantiate(GetThemePrefab(identifier));
		theme.InitializeForPreview();
		theme.Activate();
		return theme;
	}

	public void DestroyPreviewTheme(Theme theme)
	{
		Debug.Log("Preview theme destroyed");
		theme.Deactivate();
		Object.Destroy(theme.gameObject);
		if (CurrentlySerializedTheme != null)
		{
			CurrentlySerializedTheme.Activate();
		}
	}

	public Theme CreateTheme(string identifier, int woid)
	{
		if (CurrentlySerializedTheme != null)
		{
			DestroyCurrentTheme();
		}
		Debug.Log("Created theme from WO");
		Theme theme = Object.Instantiate(GetThemePrefab(identifier));
		theme.Initialize(woid);
		theme.Activate();
		CurrentlySerializedTheme = theme;
		return theme;
	}

	public void DestroyCurrentTheme()
	{
		Debug.Log("Destroying old theme: " + CurrentlySerializedTheme.name);
		CurrentlySerializedTheme.Deactivate();
		Object.Destroy(CurrentlySerializedTheme.gameObject);
		CurrentlySerializedTheme = null;
		MVWorldObjectClient mVWorldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.Theme)[0];
		MVGameControllerBase.OperationRequests.UnregisterWorldObject(mVWorldObjectClient.Id);
	}
}
