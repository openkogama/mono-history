using System.Collections.Generic;

namespace ThemeSettings;

public class SettingsPreview : SettingsWrapper
{
	public SettingsPreview()
	{
		settingsData = new Dictionary<object, object>();
	}

	public override void CommitChanges()
	{
	}
}
