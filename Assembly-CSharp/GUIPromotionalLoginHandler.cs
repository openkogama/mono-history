using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class GUIPromotionalLoginHandler : MonoBehaviour
{
	private Dictionary<string, object> gameSessionData = new Dictionary<string, object>
	{
		{ "newToken", "clientDefinedNewToken." },
		{ "newPlanetName", "game-server-test" },
		{ "isSoftLaunch", false },
		{ "embedded", false }
	};

	public void Play2D()
	{
		StartAutoJoinSession(48932);
	}

	public void Play()
	{
		StartAutoJoinSession(48887);
	}

	private void StartAutoJoinSession(int planetID)
	{
		gameSessionData["profileID"] = 0;
		gameSessionData["planetID"] = planetID;
		gameSessionData["token"] = "0";
		gameSessionData["serverIP"] = "95.211.162.200:5055";
		gameSessionData["gameMode"] = MVGameMode.Play;
		gameSessionData["language"] = "en_US";
		GameSessionData message = new GameSessionData(gameSessionData);
		MVGameControllerBase.SetGameSessionData(message);
		gameObject.SetActive(value: false);
		Debug.Log(message);
		Application.LoadLevel("BaseLegacyUI");
	}
}
