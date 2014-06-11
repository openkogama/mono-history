using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class GameSessionData
{
	private readonly string ip = string.Empty;

	private readonly int profileID = -1;

	private readonly int planetID = -1;

	private readonly MVGameMode gameMode = MVGameMode.Play;

	private readonly string language = string.Empty;

	private readonly string planetName = string.Empty;

	private readonly bool embedded;

	private readonly string profileToken = string.Empty;

	public string Ip => ip;

	public int ProfileID => profileID;

	public int PlanetID => planetID;

	public MVGameMode GameMode => gameMode;

	public string Language => language;

	public string PlanetName => planetName;

	public bool Embedded => embedded;

	public string ProfileToken => profileToken;

	public GameSessionData(Dictionary<string, object> gameSessionData)
	{
		ip = (string)gameSessionData["serverIP"];
		profileID = (int)gameSessionData["profileID"];
		planetID = (int)gameSessionData["planetID"];
		gameMode = (MVGameMode)(int)gameSessionData["gameMode"];
		language = (string)gameSessionData["language"];
		embedded = (bool)gameSessionData["embedded"];
		profileToken = (string)gameSessionData["token"];
		Debug.Log((object)("embedded " + embedded));
		if (gameSessionData.ContainsKey("planetName"))
		{
			planetName = (string)gameSessionData["planetName"];
		}
		if (GameMode == MVGameMode.CharacterEditor)
		{
			planetID = -1;
		}
	}
}
