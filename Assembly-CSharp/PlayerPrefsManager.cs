using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class PlayerPrefsManager
{
	private const string signInStateKey = "signInState";

	private static bool playerHasChangedFromTouristToRegistered;

	private static bool isReturningPlayer;

	private const string playedGamesListKey = "playedGamesList";

	private const int maxPlayGamesListCount = 50;

	private static bool isFirstTimeSession;

	private const string isFirstTimeSessionKey = "isFirstTimeSession";

	public static bool IsFirstTimeSession => isFirstTimeSession;

	public static bool IsReturningAsSignedUp => playerHasChangedFromTouristToRegistered && isReturningPlayer;

	public static bool IsReturningPlayer => isReturningPlayer;

	private static void HandleSignInState(bool isRegistered)
	{
		if (PlayerPrefs.HasKey("signInState") && PlayerPrefs.GetInt("signInState") == 0 && isRegistered)
		{
			playerHasChangedFromTouristToRegistered = true;
		}
		if (isRegistered)
		{
			PlayerPrefs.SetInt("signInState", 1);
		}
		else
		{
			PlayerPrefs.SetInt("signInState", 0);
		}
	}

	public static void EarlyInitialize()
	{
		try
		{
			if (!PlayerPrefs.HasKey("isFirstTimeSession"))
			{
				isFirstTimeSession = true;
				PlayerPrefs.SetInt("isFirstTimeSession", 1);
				PlayerPrefs.Save();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void Initialize(GameSessionData gameSessionData)
	{
		try
		{
			HandlePlayedGames(gameSessionData.planetID);
			HandleSignInState(gameSessionData.profileID > 0);
			PlayerPrefs.Save();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void HandlePlayedGames(int gameId)
	{
		if (!PlayerPrefs.HasKey("playedGamesList"))
		{
			PlayerPrefs.SetString("playedGamesList", JsonConvert.SerializeObject(new List<int>()));
		}
		string value = PlayerPrefs.GetString("playedGamesList");
		List<int> list = JsonConvert.DeserializeObject<List<int>>(value);
		int num = -1;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == gameId)
			{
				isReturningPlayer = true;
				num = i;
			}
		}
		if (num != -1)
		{
			list.RemoveAt(num);
		}
		list.Add(gameId);
		if (list.Count > 50)
		{
			list.RemoveRange(0, list.Count - 50);
		}
		PlayerPrefs.SetString("playedGamesList", JsonConvert.SerializeObject(list));
	}
}
