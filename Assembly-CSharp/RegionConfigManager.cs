using System;
using UnityEngine;

[CreateAssetMenu]
public class RegionConfigManager : ScriptableObject
{
	[SerializeField]
	private RegionConfig local;

	[SerializeField]
	private RegionConfig dev;

	[SerializeField]
	private RegionConfig test;

	[SerializeField]
	private RegionConfig friends;

	[SerializeField]
	private RegionConfig br;

	[SerializeField]
	private RegionConfig www;

	[SerializeField]
	private TextAsset regionTag;

	private string RegionTag => regionTag.text;

	public RegionConfig RegionConfig
	{
		get
		{
			if (RegionTag == "local")
			{
				return local;
			}
			if (RegionTag == "dev")
			{
				return dev;
			}
			if (RegionTag == "test")
			{
				return test;
			}
			if (RegionTag == "friends")
			{
				return friends;
			}
			if (RegionTag == "br")
			{
				return br;
			}
			if (RegionTag == "www")
			{
				return www;
			}
			throw new Exception("RegionConfig not found");
		}
	}
}
