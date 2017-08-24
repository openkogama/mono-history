using System;
using System.Collections.Generic;

namespace MV.WorldObject.MetaData;

public class ProfileMetaData
{
	[Flags]
	public enum SerializeFlag : byte
	{
		Nothing = 0,
		FirstTimeState = 1,
		TestData = 4,
		All = 7
	}

	protected readonly bool CanSerialize = true;

	public FirstTimeState FirstTimeState = new FirstTimeState();

	public Dictionary<string, string> TestData = new Dictionary<string, string>();

	protected SerializeFlag serializeFlags = SerializeFlag.All;

	public ProfileMetaData()
	{
	}

	protected ProfileMetaData(bool canSerialize)
	{
		CanSerialize = canSerialize;
	}

	public bool ShouldSerializeFirstTimeState()
	{
		if (!CanSerialize)
		{
			throw new Exception("Can't serialize meta data");
		}
		return (serializeFlags & SerializeFlag.FirstTimeState) != 0;
	}

	public bool ShouldSerializeTestData()
	{
		if (!CanSerialize)
		{
			throw new Exception("Can't serialize meta data");
		}
		return (serializeFlags & SerializeFlag.TestData) != 0;
	}
}
