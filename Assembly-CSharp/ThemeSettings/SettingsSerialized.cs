namespace ThemeSettings;

public class SettingsSerialized : SettingsWrapper
{
	private WorldObjectClientRef<ThemeWorldObject> worldObjectRef;

	public SettingsSerialized(int woid)
	{
		worldObjectRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef<ThemeWorldObject>(woid);
		settingsData = worldObjectRef.WorldObjectClient.SettingsData;
	}

	public override void CommitChanges()
	{
		worldObjectRef.WorldObjectClient?.CommitSettings();
	}
}
