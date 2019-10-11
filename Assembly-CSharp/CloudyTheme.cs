using UnityEngine;

public class CloudyTheme : CloudyThemeBase
{
	[Header("Meta")]
	[SerializeField]
	private string identifier = "UID";

	[SerializeField]
	private string displayName = "No name";

	[SerializeField]
	private string description = "Description missing.";

	public override string Identifier => identifier;

	public override string DisplayName => displayName;

	public override string Description => description;

	private string Validate(string str)
	{
		if (str.Length < "_(\"".Length || str.Substring(0, "_(\"".Length) != "_(\"")
		{
			str = "_(\"" + str;
		}
		if (str.Length < "\")".Length || str.Substring(str.Length - "\")".Length, "\")".Length) != "\")")
		{
			str += "\")";
		}
		return str;
	}
}
