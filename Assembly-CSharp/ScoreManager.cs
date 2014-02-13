using System;
using System.Collections;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ScoreManager
{
	private const int KILL_POINTS = 1;

	private const int SUICIDE_POINTS = -1;

	private const int FLAG_CAPTURE_POINTS = 5;

	public ScoreManager()
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnReceivedGameMsg = (MVNetworkGame.OnReceivedGameMsgDelegate)Delegate.Combine(game.OnReceivedGameMsg, new MVNetworkGame.OnReceivedGameMsgDelegate(ReceiveGameMessage));
		MVGameController.Instance.Game.NetworkGameStateListener.OnGameStateChanged += GameStateListener_OnGameStateChanged;
	}

	private void ReceiveGameMessage(MVGameMsgType type, Hashtable gameMsgData)
	{
		if (type == MVGameMsgType.AvatarKilled)
		{
			GameMessages.PlayerKilledMessage playerKilledMessage = GameMessages.ParsePlayerKilledMessage(gameMsgData);
			MVPlayer mVPlayer = MVGameController.Instance.Game.Players[playerKilledMessage.killerId];
			MVPlayer mVPlayer2 = MVGameController.Instance.Game.Players[playerKilledMessage.playerId];
			int killerScore = 0;
			int teamScore = MVGameController.Instance.Game.TeamManager.GetScore(mVPlayer.Team);
			ScoreCalculation.KillScore(playerKilledMessage.playerId, mVPlayer2.Team, playerKilledMessage.killerId, mVPlayer.Team, MVGameController.Instance.Game.TeamManager.TeamCount(), ref killerScore, ref teamScore);
			Debug.Log((object)"Change score");
			mVPlayer.ChangeScore(killerScore);
			MVGameController.Instance.Game.TeamManager.SetScore(mVPlayer.Team, teamScore);
		}
	}

	private void GameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
	}

	private bool TeamKill(MVPlayer killer, MVPlayer victim)
	{
		return MVGameController.Instance.Game.TeamManager.TeamCount() > 1 && killer.ActorNr != victim.ActorNr && killer.Team == victim.Team;
	}
}
