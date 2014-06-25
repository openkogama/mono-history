using System;
using System.Collections.Generic;

namespace MV.WorldObject;

public class MvAvatarMetaDataWoMap
{
	private Dictionary<int, MvAvatarMetaData> avatarWOIDAvatarMetaData = new Dictionary<int, MvAvatarMetaData>();

	public MvAvatarMetaDataWoMap()
	{
	}

	public MvAvatarMetaDataWoMap(BytePacker bp)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int key = bp.ReadInt32();
			MvAvatarMetaData value = new MvAvatarMetaData(bp);
			avatarWOIDAvatarMetaData.Add(key, value);
		}
	}

	public bool TryGetValue(int woID, out MvAvatarMetaData avatarMetaData)
	{
		return avatarWOIDAvatarMetaData.TryGetValue(woID, out avatarMetaData);
	}

	public void Add(int woID, MvAvatarMetaData avatarMetaData)
	{
		avatarWOIDAvatarMetaData[woID] = avatarMetaData;
	}

	public void ResetAvatar(int prevAvatarWoID, int newAvatarWoID)
	{
		if (avatarWOIDAvatarMetaData.TryGetValue(prevAvatarWoID, out var value))
		{
			avatarWOIDAvatarMetaData.Remove(prevAvatarWoID);
			avatarWOIDAvatarMetaData.Add(newAvatarWoID, value);
			return;
		}
		throw new ArgumentException("AvatarWo not present in avatarWOIDAvatarMetaData");
	}

	public byte[] ToByteArray()
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(avatarWOIDAvatarMetaData.Count);
		foreach (KeyValuePair<int, MvAvatarMetaData> avatarWOIDAvatarMetaDatum in avatarWOIDAvatarMetaData)
		{
			bytePacker.Write(avatarWOIDAvatarMetaDatum.Key);
			bytePacker.Write(avatarWOIDAvatarMetaDatum.Value.ToByteArray());
		}
		return bytePacker.ToArray();
	}

	public override string ToString()
	{
		string text = $"Count: {avatarWOIDAvatarMetaData.Count}.\n";
		foreach (KeyValuePair<int, MvAvatarMetaData> avatarWOIDAvatarMetaDatum in avatarWOIDAvatarMetaData)
		{
			text += $"WoID {avatarWOIDAvatarMetaDatum.Key}. MetaData {avatarWOIDAvatarMetaDatum.Value}.\n";
		}
		return text;
	}
}
