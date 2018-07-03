using System;
using System.Collections.Generic;
using ThemeSettings;
using UnityEngine;

namespace ThemeAttributes;

[Serializable]
public class ColorAttribute : NamedThemeAttribute<Color>
{
	public abstract class Setter : MonoBehaviour
	{
		public abstract void Initialize(ColorAttribute attrib, Action<Color> onChange);
	}

	private class ColorKeys
	{
		public const string r = "Red";

		public const string g = "Green";

		public const string b = "Blue";

		public const string a = "Alpha";
	}

	[Header("Dependencies")]
	[SerializeField]
	private Setter prefab;

	public override object Data => ToSerializable(value);

	public override void Initialize(SettingsWrapper settings, string key, int groups, Action<Color> onChange)
	{
		base.Initialize(settings, key, groups, onChange);
		value = Constrain(ConvertToColor(settings.GetValueForAttribute<Dictionary<object, object>>(this)));
	}

	private Dictionary<object, object> ToSerializable(Color c)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("Red", c.r);
		dictionary.Add("Green", c.g);
		dictionary.Add("Blue", c.b);
		dictionary.Add("Alpha", c.a);
		return dictionary;
	}

	private static Color ConvertToColor(Dictionary<object, object> value)
	{
		return new Color((float)value["Red"], (float)value["Green"], (float)value["Blue"], (float)value["Alpha"]);
	}

	private float Constrain(float value)
	{
		return Mathf.Clamp(value, 0f, 1f);
	}

	private Color Constrain(Color value)
	{
		value.r = Constrain(value.r);
		value.g = Constrain(value.g);
		value.b = Constrain(value.b);
		value.a = Constrain(value.a);
		return value;
	}

	protected override void OnSettingsChanged(Color value)
	{
		base.value = value;
		UpdateSettings(Key, ToSerializable(value));
		themeCallback(base.value);
	}

	public override RectTransform GetSettingsUIObject()
	{
		Setter setter = UnityEngine.Object.Instantiate(prefab);
		setter.Initialize(this, OnSettingsChanged);
		return (RectTransform)setter.transform;
	}
}
