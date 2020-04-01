using System;
using StatHat;
using UnityEngine;

public static class StatHatWrapper
{
	private static bool isFirstTimeSession = false;

	private const string allBtFormat = "{0}.u.{1}";

	private const string allBtFormatFtsStats = "{0}.u.fts.{1}";

	private const string ezKey = "h5g9REtmi1LT7JY5";

	private const string btFormat = "{0}.u.standalone.{1}";

	private const string btFormatFtsStats = "{0}.u.fts.standalone.{1}";

	private static StatHatConfig statHatConfig = default;

	public static void Initialize(bool isFirstTimeSession, StatHatConfig statHatConfig)
	{
		StatHatWrapper.isFirstTimeSession = isFirstTimeSession;
		StatHatWrapper.statHatConfig = statHatConfig;
	}

	public static void Count(string key, int count)
	{
		try
		{
			if (statHatConfig.isEnabled)
			{
				string allBtKey = $"{statHatConfig.regionKey}.u.{key}";
				string btKey = $"{statHatConfig.regionKey}.u.standalone.{key}";
				Count(allBtKey, btKey, count);
				if (isFirstTimeSession)
				{
					string allBtKey2 = $"{statHatConfig.regionKey}.u.fts.{key}";
					string btKey2 = $"{statHatConfig.regionKey}.u.fts.standalone.{key}";
					Count(allBtKey2, btKey2, count);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void Value(string key, int value)
	{
		try
		{
			if (statHatConfig.isEnabled)
			{
				string allBtKey = $"{statHatConfig.regionKey}.u.{key}";
				string btKey = $"{statHatConfig.regionKey}.u.standalone.{key}";
				Value(allBtKey, btKey, value);
				if (isFirstTimeSession)
				{
					string allBtKey2 = $"{statHatConfig.regionKey}.u.fts.{key}";
					string btKey2 = $"{statHatConfig.regionKey}.u.fts.standalone.{key}";
					Value(allBtKey2, btKey2, value);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void Value(string key, float value)
	{
		try
		{
			if (statHatConfig.isEnabled)
			{
				string allBtKey = $"{statHatConfig.regionKey}.u.{key}";
				string btKey = $"{statHatConfig.regionKey}.u.standalone.{key}";
				Value(allBtKey, btKey, value);
				if (isFirstTimeSession)
				{
					string allBtKey2 = $"{statHatConfig.regionKey}.u.fts.{key}";
					string btKey2 = $"{statHatConfig.regionKey}.u.fts.standalone.{key}";
					Value(allBtKey2, btKey2, value);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void Count(string allBtKey, string btKey, int count)
	{
		Post.EzCounter("h5g9REtmi1LT7JY5", allBtKey, count);
		Post.EzCounter("h5g9REtmi1LT7JY5", btKey, count);
	}

	private static void Value(string allBtKey, string btKey, int value)
	{
		Post.EzValue("h5g9REtmi1LT7JY5", allBtKey, value);
		Post.EzValue("h5g9REtmi1LT7JY5", btKey, value);
	}

	private static void Value(string allBtKey, string btKey, float value)
	{
		Post.EzValue("h5g9REtmi1LT7JY5", allBtKey, value);
		Post.EzValue("h5g9REtmi1LT7JY5", btKey, value);
	}
}
