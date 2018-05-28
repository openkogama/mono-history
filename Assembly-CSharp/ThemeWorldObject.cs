using System.Collections.Generic;

public class ThemeWorldObject : MVWorldObjectClient
{
	public Dictionary<object, object> SettingsData => (Dictionary<object, object>)Data["settings"];

	private string Identifier => (string)Data["identifier"];

	private Theme Theme { get; set; }

	public ThemeWorldObject(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void Initialize()
	{
		Theme = ThemeRepository.Instance.CreateTheme(Identifier, id);
	}

	public void CommitSettings()
	{
		MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(id, new Dictionary<object, object> { { "settings", SettingsData } });
	}

	public override void Reset()
	{
		base.Reset();
		Theme.ThemeReset();
	}
}
