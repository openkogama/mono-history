using System;
using ThemeSettings;
using UnityEngine;

namespace ThemeAttributes;

[Serializable]
public class FloatAttribute : NamedThemeAttribute<float>
{
	public abstract class Setter : MonoBehaviour
	{
		public abstract void Initialize(FloatAttribute attrib, Action<float> onChange);
	}

	[SerializeField]
	private float min;

	[SerializeField]
	private float max;

	[Header("Dependencies")]
	[SerializeField]
	private Setter sliderPrefab;

	public override object Data => value;

	public float Min => min;

	public float Max => max;

	public override void Initialize(SettingsWrapper settings, string key, int groups, Action<float> onChange)
	{
		base.Initialize(settings, key, groups, onChange);
		value = Constrain(settings.GetValueForAttribute<float>(this));
	}

	public float Constrain(float value)
	{
		return Mathf.Clamp(value, min, max);
	}

	protected override void OnSettingsChanged(float value)
	{
		base.value = Constrain(value);
		UpdateSettings(Key, value);
		themeCallback(base.value);
	}

	public override RectTransform GetSettingsUIObject()
	{
		Setter setter = UnityEngine.Object.Instantiate(sliderPrefab);
		setter.Initialize(this, OnSettingsChanged);
		return (RectTransform)setter.transform;
	}
}
