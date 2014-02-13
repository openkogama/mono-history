using System.Collections.Generic;
using MV.Common;

public class GameSessionData
{
	private readonly string ip = string.Empty;

	private readonly int profileID = -1;

	private readonly int planetID = -1;

	private readonly MVGameMode gameMode = MVGameMode.Play;

	private readonly string language = string.Empty;

	private readonly string planetName = string.Empty;

	public string Ip => ip;

	public int ProfileID => profileID;

	public int PlanetID => planetID;

	public MVGameMode GameMode => gameMode;

	public string Language => language;

	public string PlanetName => planetName;

	public GameSessionData(Dictionary<string, object> gameSessionData)
	{
		ip = (string)gameSessionData["serverIP"];
		profileID = (int)gameSessionData["profileID"];
		planetID = (int)gameSessionData["planetID"];
		gameMode = (MVGameMode)(int)gameSessionData["gameMode"];
		language = (string)gameSessionData["language"];
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
