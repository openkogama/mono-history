using System;
using ThemeSettings;
using UnityEngine;

namespace ThemeAttributes;

[Serializable]
public class IntAttribute : NamedThemeAttribute<int>
{
	public abstract class Setter : MonoBehaviour
	{
		public abstract void Initialize(IntAttribute attrib, Action<int> onChange);
	}

	[SerializeField]
	private int min = int.MinValue;

	[SerializeField]
	private int max = int.MaxValue;

	[SerializeField]
	[Header("Dependencies")]
	private Setter prefab;

	public override object Data => value;

	public int Min => min;

	public int Max => max;

	public override void Initialize(SettingsWrapper settings, string key, int groups, Action<int> onChange)
	{
		base.Initialize(settings, key, groups, onChange);
		value = Constrain(settings.GetValueForAttribute<int>(this));
	}

	private int Constrain(int value)
	{
		return Mathf.Clamp(value, min, max);
	}

	protected override void OnSettingsChanged(int value)
	{
		base.value = Constrain(value);
		UpdateSettings(Key, Data);
		themeCallback(base.value);
	}

	public override RectTransform GetSettingsUIObject()
	{
		Setter setter = UnityEngine.Object.Instantiate(prefab);
		setter.Initialize(this, OnSettingsChanged);
		return (RectTransform)setter.transform;
	}
}
