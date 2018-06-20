using System.Collections.Generic;
using UnityEngine;

public class ThemeWorldObject : MVWorldObjectClient
{
	public bool SkyboxOverride => Visualization.OverrideSkyboxManager;

	public Dictionary<object, object> SettingsData => (Dictionary<object, object>)Data["settings"];

	public string Identifier => (string)Data["identifier"];

	public Theme Visualization { get; private set; }

	public ThemeWorldObject(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void Initialize()
	{
		Visualization = Object.Instantiate(ThemeRepository.Instance.GetThemePrefab(Identifier));
		Visualization.Initialize(id);
		Visualization.Activate();
	}

	public override void Destroy()
	{
		base.Destroy();
		Visualization.Deactivate();
		Object.Destroy(Visualization.gameObject);
	}

	public void CommitSettings()
	{
		MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(id, new Dictionary<object, object> { { "settings", SettingsData } });
	}

	public override void Reset()
	{
		base.Reset();
		Visualization.ThemeReset();
	}

	public override void OnDataUpdate()
	{
		Visualization.Initialize(id);
	}
}
