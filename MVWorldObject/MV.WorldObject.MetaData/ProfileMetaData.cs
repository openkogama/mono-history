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
		MouseSensitivity = 0x10,
		GoldRewardLevel = 0x20,
		PlayNewGamesForGoldData = 0x40,
		All = 0x7F
	}

	protected readonly bool IsInitialized = true;

	public FirstTimeState FirstTimeState = new FirstTimeState();

	public ProfileHighlightState ProfileHighlightState = new ProfileHighlightState();

	public float MS = 1f;

	public Dictionary<string, string> TestData = new Dictionary<string, string>();

	protected SerializeFlag serializeFlags = SerializeFlag.All;

	public ProfileMetaData()
	{
	}

	protected ProfileMetaData(bool isInitialized)
	{
		IsInitialized = isInitialized;
	}

	public bool ShouldSerializeFirstTimeState()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data. Not initialized.");
		}
		return (serializeFlags & SerializeFlag.FirstTimeState) != 0;
	}

	public bool ShouldSerializeTestData()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data. Not initialized.");
		}
		return (serializeFlags & SerializeFlag.TestData) != 0;
	}

	public bool ShouldSerializeProfileHighlightState()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data. Not initialized.");
		}
		return (serializeFlags & SerializeFlag.ProfileHighlightState) != 0;
	}

	public bool ShouldSerializeMS()
	{
		if (!IsInitialized)
		{
			throw new Exception("Can't serialize meta data. Not initialized.");
		}
		return (serializeFlags & SerializeFlag.MouseSensitivity) != 0;
	}
}
