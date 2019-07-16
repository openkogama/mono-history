using System.Collections.Generic;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingsCollectionBase : KogamaSettingWrapperBase
{
	protected readonly Dictionary<string, KogamaSettingWrapperBase> children = new Dictionary<string, KogamaSettingWrapperBase>();

	public Dictionary<string, KogamaSettingWrapperBase> Children => children;

	public virtual void AddChild(KogamaSettingWrapperBase kogamaSetting)
	{
		children.Add(kogamaSetting.Key, kogamaSetting);
	}

	public KogamaSettingsCollectionBase(string key, KogamaSettingsCollectionBase kogamaSettingsCollection)
		: base(key, kogamaSettingsCollection)
	{
	}

	public virtual KogamaSettingsCollectionBase CopyWithOutChildren(KogamaSettingsCollectionBase parent)
	{
		return new KogamaSettingsCollectionBase(Key, parent);
	}

	public override string ToString()
	{
		return string.Format(Key);
	}
}
