using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public static class GameSessionCounterRules
{
	private abstract class IRule
	{
		protected IRule(Dictionary<GameSessionCounterType, GameSessionCounters.GameSessionCounter> sessionCounters)
		{
		}
	}

	private class WallJump5 : IRule
	{
		private int wallJumpCount;

		private Vector3 prevPos;

		private float minJumpDistance = 2f;

		public WallJump5(Dictionary<GameSessionCounterType, GameSessionCounters.GameSessionCounter> sessionCounters)
			: base(sessionCounters)
		{
			GameSessionCounters.GameSessionCounter gameSessionCounter = sessionCounters[GameSessionCounterType.WallJump];
			gameSessionCounter.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter.callbacks, new Action<GameSessionCounterType, int>(WallJump));
			GameSessionCounters.GameSessionCounter gameSessionCounter2 = sessionCounters[GameSessionCounterType.Grounded];
			gameSessionCounter2.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter2.callbacks, new Action<GameSessionCounterType, int>(Ground));
		}

		private void WallJump(GameSessionCounterType gameSessionCounterType, int count)
		{
			if (wallJumpCount <= 0 || !((prevPos - MVGameControllerBase.Game.LocalPlayer.Avatar.Transform.position).magnitude < minJumpDistance))
			{
				prevPos = MVGameControllerBase.Game.LocalPlayer.Avatar.Transform.position;
				wallJumpCount++;
				if (wallJumpCount >= 5)
				{
					LevelingManager.AddXPToLocalPlayer("WallJump5", MVGameMode.Play);
					wallJumpCount = 0;
				}
			}
		}

		private void Ground(GameSessionCounterType gameSessionCounterType, int count)
		{
			wallJumpCount = 0;
		}
	}

	private static List<IRule> rules = new List<IRule>();

	private static bool cubeGunCubeAdded100Rewarded = false;

	public static void AddRules(Dictionary<GameSessionCounterType, GameSessionCounters.GameSessionCounter> sessionCounters)
	{
		GameSessionCounters.GameSessionCounter gameSessionCounter = sessionCounters[GameSessionCounterType.CubeGunCubeDelta];
		gameSessionCounter.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter.callbacks, new Action<GameSessionCounterType, int>(CubeGunCubeAdded100));
		GameSessionCounters.GameSessionCounter gameSessionCounter2 = sessionCounters[GameSessionCounterType.GameWon];
		gameSessionCounter2.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter2.callbacks, new Action<GameSessionCounterType, int>(GameWon));
		GameSessionCounters.GameSessionCounter gameSessionCounter3 = sessionCounters[GameSessionCounterType.AllMaterialsUnlocked];
		gameSessionCounter3.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter3.callbacks, new Action<GameSessionCounterType, int>(AllMaterialUnlocked));
		GameSessionCounters.GameSessionCounter gameSessionCounter4 = sessionCounters[GameSessionCounterType.Session1Min];
		gameSessionCounter4.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter4.callbacks, new Action<GameSessionCounterType, int>(EditMode60Min));
		GameSessionCounters.GameSessionCounter gameSessionCounter5 = sessionCounters[GameSessionCounterType.JumpOnOculusEye];
		gameSessionCounter5.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter5.callbacks, new Action<GameSessionCounterType, int>(JumpOnOculusEyeOnce));
		GameSessionCounters.GameSessionCounter gameSessionCounter6 = sessionCounters[GameSessionCounterType.OculusKilledByLocalPlayerInCloseCombat];
		gameSessionCounter6.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter6.callbacks, new Action<GameSessionCounterType, int>(OculusCloseCombat));
		GameSessionCounters.GameSessionCounter gameSessionCounter7 = sessionCounters[GameSessionCounterType.KillOnOtherTeam];
		gameSessionCounter7.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter7.callbacks, new Action<GameSessionCounterType, int>(KillOnOtherTeam));
		GameSessionCounters.GameSessionCounter gameSessionCounter8 = sessionCounters[GameSessionCounterType.Session1Min];
		gameSessionCounter8.callbacks = (Action<GameSessionCounterType, int>)Delegate.Combine(gameSessionCounter8.callbacks, new Action<GameSessionCounterType, int>(Min1XpRewardPlayMode));
		rules.Add(new WallJump5(sessionCounters));
	}

	public static void OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
		GameStatCounterType counterType = e.counterType;
		if (counterType == GameStatCounterType.Kill && e.actorNumber == MVGameControllerBase.Game.LocalPlayer.ActorNr && e.actorNumber != e.otherID)
		{
			GameSessionCounters.Increment(GameSessionCounterType.Kill);
			if (MVGameControllerBase.Game.MVPlayerContainer[e.otherID].Team != MVGameControllerBase.Game.MVPlayerContainer[e.actorNumber].Team)
			{
				GameSessionCounters.Increment(GameSessionCounterType.KillOnOtherTeam);
			}
		}
	}

	private static void Min1XpRewardPlayMode(GameSessionCounterType gameSessionCounterType, int count)
	{
		if (count % 1 == 0)
		{
			LevelingManager.AddXPToLocalPlayer("1MinXpRewardPlayMode", MVGameMode.Play);
		}
	}

	private static void KillOnOtherTeam(GameSessionCounterType gameSessionCounterType, int count)
	{
		LevelingManager.AddXPToLocalPlayer("KillOnOtherTeam", MVGameMode.Play);
	}

	private static void OculusCloseCombat(GameSessionCounterType gameSessionCounterType, int count)
	{
		LevelingManager.AddXPToLocalPlayer("OculusKilledInCloseCombat", MVGameMode.Play);
	}

	private static void JumpOnOculusEyeOnce(GameSessionCounterType gameSessionCounterType, int count)
	{
		LevelingManager.AddXPToLocalPlayer("JumpOnOculusEyeOnce", MVGameMode.Play);
	}

	private static void EditMode60Min(GameSessionCounterType gameSessionCounterType, int count)
	{
		if (count % 60 == 0)
		{
			LevelingManager.AddXPToLocalPlayer("EditMode60Min", MVGameMode.Edit);
		}
	}

	private static void AllMaterialUnlocked(GameSessionCounterType gameSessionCounterType, int count)
	{
		Debug.Log($"{gameSessionCounterType}: {count}");
		LevelingManager.AddXPToLocalPlayer("AllMaterialsUnlocked", MVGameMode.Edit);
		LevelingManager.AddXPToLocalPlayer("AllMaterialsUnlocked", MVGameMode.CharacterEditor);
	}

	private static void GameWon(GameSessionCounterType gameSessionCounterType, int count)
	{
		LevelingManager.AddXPToLocalPlayer("GameWon", MVGameMode.Play);
	}

	private static void DebugCallback(GameSessionCounterType gameSessionCounterType, int count)
	{
	}

	private static void CubeGunCubeAdded100(GameSessionCounterType gameSessionCounterType, int count)
	{
		if (!cubeGunCubeAdded100Rewarded && count != 0 && count % 100 == 0)
		{
			LevelingManager.AddXPToLocalPlayer("CubeGunCubeAdded100", MVGameMode.Play);
			cubeGunCubeAdded100Rewarded = true;
		}
	}
}
