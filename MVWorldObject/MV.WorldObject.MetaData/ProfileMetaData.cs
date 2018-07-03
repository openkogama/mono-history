using System;
using System.Collections.Generic;
using MV.WorldObject.HighlightSystem.HighlightPayloads;

namespace MV.WorldObject.MetaData;

public class ProfileMetaData
{
	[Flags]
	public enum SerializeFlag : byte
	{
		Nothing = 0,
		FirstTimeState = 1,
		TestData = 4,
		ProfileHighlightState = 8,
		All = 0xF
	}

	protected readonly bool IsInitialized = true;

	public FirstTimeState FirstTimeState = new FirstTimeState();

	public ProfileHighlightState ProfileHighlightState = new ProfileHighlightState();

	public Dictionary<string, string> TestData = new Dictionary<string, string>();

	protected SerializeFlag serializeFlags = SerializeFlag.All;

	public ProfileMetaData()
	{
		TestData.Add("TestString", "This is a test");
	}

	protected ProfileMetaData(bool isInitialized)
	{
		IsInitialized = isInitialized;
	}

	public bool ShouldSerializeFirstTimeState()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data");
		}
		return (serializeFlags & SerializeFlag.FirstTimeState) != 0;
	}

	public bool ShouldSerializeTestData()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data");
		}
		return (serializeFlags & SerializeFlag.TestData) != 0;
	}

	public bool ShouldSerializeProfileHighlightState()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data");
		}
		return (serializeFlags & SerializeFlag.ProfileHighlightState) != 0;
	}
}
