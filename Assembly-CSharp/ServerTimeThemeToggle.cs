using System;
using ThemeAttributes;

public class ServerTimeThemeToggle : ThemeToggle
{
	private string labelText = string.Empty;

	protected void Awake()
	{
		enabled = false;
	}

	public override void Initialize(BoolAttribute attrib, Action<bool> onChange)
	{
		base.Initialize(attrib, onChange);
		labelText = label.text;
		enabled = true;
	}

	protected void Update()
	{
		DateTime dateTime = DateTime.UtcNow.AddHours(MVGameControllerBase.Game.TimeZone);
		label.text = string.Format("{0} {1}", labelText, dateTime.ToString("HH:mm:ss"));
	}
}
