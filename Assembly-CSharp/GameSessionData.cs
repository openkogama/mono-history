using System.Collections.Generic;
using ExitGames.Client.Photon;
using MV.Common;

public class GameSessionData
{
	private static class ReferrerStrings
	{
		public const string Poki = "gsm";
	}

	public string serverIP = string.Empty;

	public int profileID = -1;

	public int planetID = -1;

	public MVGameMode gameMode = MVGameMode.Play;

	public string language = string.Empty;

	public bool embedded;

	public string token = string.Empty;

	public string newToken = "newProfileTokenNotSet";

	public string newPlanetName = "game-server-test";

	public string pingURL = string.Empty;

	public string disconnectURL = string.Empty;

	public string gameRewardURL = string.Empty;

	public string gamePublishedURL = string.Empty;

	public string purchaseGoldURL = string.Empty;

	public string loginURL = string.Empty;

	public string signupURL = string.Empty;

	public string idleURL = string.Empty;

	public string disconnectedURL = string.Empty;

	public string region = "RegionNotDefined";

	public string ezKey = "h5g9REtmi1LT7JY5";

	public string reauthURL = string.Empty;

	public string gameRewardDataURL = string.Empty;

	public string referrer = string.Empty;

	public bool detailedStats;

	private Dictionary<string, LoadLogoType> referralToLogoMap = new Dictionary<string, LoadLogoType> { 
	{
		"gsm",
		LoadLogoType.Poki
	} };

	public bool IsPlayedFromPoki => referrer == "gsm";

	public LoadLogoType LoadLogoType
	{
		get
		{
			if (referralToLogoMap.ContainsKey(referrer))
			{
				return referralToLogoMap[referrer];
			}
			return LoadLogoType.None;
		}
	}

	public ConnectionProtocol ConnectionProtocol => ConnectionProtocol.Udp;

	public GameSessionData()
	{
	}

	public GameSessionData(Dictionary<string, object> gameSessionData)
	{
		serverIP = (string)gameSessionData["serverIP"];
		profileID = (int)gameSessionData["profileID"];
		planetID = (int)gameSessionData["planetID"];
		gameMode = (MVGameMode)gameSessionData["gameMode"];
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
		if (gameMode == MVGameMode.CharacterEditor)
		{
			planetID = -1;
		}
	}

	public override string ToString()
	{
		return $"serverIP {serverIP}. profileID {profileID}. planetID {planetID}. gameMode {gameMode}. language {language}. embedded {embedded}. token {token}. newToken {newToken}. newPlanetName {newPlanetName}.";
	}
}
