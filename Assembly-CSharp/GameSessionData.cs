using System.Collections.Generic;
using MV.Common;

public class GameSessionData
{
	public string serverIP = string.Empty;

	public int profileID = -1;

	public int planetID = -1;

	public MVGameMode gameMode = MVGameMode.Play;

	public string language = string.Empty;

	public string planetName = string.Empty;

	public bool embedded;

	public string token = string.Empty;

	public string newToken = "newProfileTokenNotSet";

	public string newPlanetName = "game-server-test";

	public string pingURL = string.Empty;

	public string disconnectURL = string.Empty;

	public string gameRewardURL = string.Empty;

	public string convertToSilverURL = string.Empty;

	public string gamePublishedURL = string.Empty;

	public string purchaseGoldURL = string.Empty;

	public string loginURL = string.Empty;

	public string signupURL = string.Empty;

	public string idleURL = string.Empty;

	public string disconnectedURL = string.Empty;

	public string region = "RegionNotDefined";

	public string ezKey = "z6U6gACPMA1m8zg5";

	public string reauthURL = string.Empty;

	public string gameRewardDataURL = string.Empty;

	public GameSessionData()
	{
	}

	public GameSessionData(Dictionary<string, object> gameSessionData)
	{
		serverIP = (string)gameSessionData["serverIP"];
		profileID = (int)gameSessionData["profileID"];
		planetID = (int)gameSessionData["planetID"];
		gameMode = (MVGameMode)(int)gameSessionData["gameMode"];
		language = (string)gameSessionData["language"];
		embedded = (bool)gameSessionData["embedded"];
		token = (string)gameSessionData["token"];
		if (gameSessionData.ContainsKey("newToken"))
		{
			newToken = (string)gameSessionData["newToken"];
		}
		if (gameSessionData.ContainsKey("newPlanetName"))
		{
			newPlanetName = (string)gameSessionData["newPlanetName"];
		}
		if (gameSessionData.ContainsKey("planetName"))
		{
			planetName = (string)gameSessionData["planetName"];
		}
		if (gameMode == MVGameMode.CharacterEditor)
		{
			planetID = -1;
		}
	}

	public override string ToString()
	{
		return $"serverIP {serverIP}. profileID {profileID}. planetID {planetID}. gameMode {gameMode}. language {language}. embedded {embedded}. token {token}. newToken {newToken}. newPlanetName {newPlanetName}. planetName {planetName}.";
	}
}
