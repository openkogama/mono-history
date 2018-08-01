using System;
using ThemeSettings;
using UnityEngine;

namespace ThemeAttributes;

[Serializable]
public class BoolAttribute : NamedThemeAttribute<bool>
{
	public abstract class Setter : MonoBehaviour
	{
		public abstract void Initialize(BoolAttribute attrib, Action<bool> onChange);
	}

	[Header("Dependencies")]
	[SerializeField]
	private Setter togglePrefab;

	public override object Data => value;

	public override void Initialize(SettingsWrapper settings, string key, int groups, Action<bool> onChange)
	{
		base.Initialize(settings, key, groups, onChange);
		value = settings.GetValueForAttribute<bool>(this);
	}

	protected override void OnSettingsChanged(bool value)
	{
		base.value = value;
		UpdateSettings(Key, value);
		themeCallback(base.value);
	}

	public override RectTransform GetSettingsUIObject()
	{
		Setter setter = UnityEngine.Object.Instantiate(togglePrefab);
		setter.Initialize(this, OnSettingsChanged);
		return (RectTransform)setter.transform;
	}
}
