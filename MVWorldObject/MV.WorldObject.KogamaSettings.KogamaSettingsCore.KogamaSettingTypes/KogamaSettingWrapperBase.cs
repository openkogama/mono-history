namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public abstract class KogamaSettingWrapperBase
{
	private readonly string key;

	public KogamaSettingsCollectionBase Parent { get; private set; }

	public string Key => key;

	protected KogamaSettingWrapperBase(string key, KogamaSettingsCollectionBase parent)
	{
		Parent = parent;
		this.key = key;
	}
}
