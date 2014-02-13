using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVGUIGameMessages : UXViewScript
{
	public float windowScale = 0.8f;

	public float messageStayTime = 2f;

	public int maxMessages = 5;

	public bool queueMessages;

	private float messageFadeOutTime = 1f;

	public Transform messageRoot;

	public MVGUIPlayerKilledLine playerKilledPrefab;

	public MVGUIPlayerJoinedLeftLine playerJoinedLeftPrefab;

	public MVGUIXPGainedLine xpGainedPrefab;

	public MVGUICollectibleLine collectiblePrefab;

	public MVGUIAchievementGetLine achievementGetPrefab;

	public MVGUICheckpointLine checkpointPrefab;

	private List<UXLine> cachedLines = new List<UXLine>();

	private List<UXLine> lines = new List<UXLine>();

	private bool listenersInitialized;

	private void InitializeListeners()
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnReceivedGameMsg = (MVNetworkGame.OnReceivedGameMsgDelegate)Delegate.Combine(game.OnReceivedGameMsg, new MVNetworkGame.OnReceivedGameMsgDelegate(AddLine));
		MVNetworkGame game2 = MVGameController.Instance.Game;
		game2.OnReceivedXP = (MVNetworkGame.OnReceivedXPDelegate)Delegate.Combine(game2.OnReceivedXP, new MVNetworkGame.OnReceivedXPDelegate(AddXPLine));
		UXScreen uXScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(OnFullScreenChange));
		OnFullScreenChange(uXScreen.Fullscreen);
		listenersInitialized = true;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (!listenersInitialized)
		{
			InitializeListeners();
		}
	}

	private void OnFullScreenChange(bool full)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float num = ((!full) ? windowScale : 1f);
		((Component)messageRoot).transform.localScale = new Vector3(num, num, num);
		((Component)messageRoot).transform.localPosition = new Vector3(0f, (!full) ? (-5.5f) : (-8f), 0f);
	}

	public void AddJoinLine()
	{
		MVGUIPlayerJoinedLeftLine mVGUIPlayerJoinedLeftLine = GetLine(playerJoinedLeftPrefab) as MVGUIPlayerJoinedLeftLine;
		mVGUIPlayerJoinedLeftLine.BuildLine(new GameMessages.PlayerJoinMessage
		{
			playerId = MVGameController.Instance.Game.LocalPlayerActorNumber
		});
		AddLine(mVGUIPlayerJoinedLeftLine);
	}

	public void AddXPLine(int amount)
	{
		MVGUIXPGainedLine mVGUIXPGainedLine = GetLine(xpGainedPrefab) as MVGUIXPGainedLine;
		mVGUIXPGainedLine.BuildLine(amount);
		AddLine(mVGUIXPGainedLine);
	}

	public void AddLine(MVGameMsgType msgType, Hashtable message)
	{
		switch (msgType)
		{
		case MVGameMsgType.AvatarKilled:
		{
			MVGUIPlayerKilledLine mVGUIPlayerKilledLine = GetLine(playerKilledPrefab) as MVGUIPlayerKilledLine;
			GameMessages.PlayerKilledMessage data6 = GameMessages.ParsePlayerKilledMessage(message);
			mVGUIPlayerKilledLine.BuildLine(data6);
			AddLine(mVGUIPlayerKilledLine);
			break;
		}
		case MVGameMsgType.UserJoined:
		case MVGameMsgType.UserLeft:
		{
			MVGUIPlayerJoinedLeftLine mVGUIPlayerJoinedLeftLine = GetLine(playerJoinedLeftPrefab) as MVGUIPlayerJoinedLeftLine;
			if (msgType == MVGameMsgType.UserJoined)
			{
				GameMessages.PlayerJoinMessage data4 = GameMessages.ParsePlayerJoinMessage(message);
				mVGUIPlayerJoinedLeftLine.BuildLine(data4);
			}
			else
			{
				GameMessages.PlayerLeftMessage data5 = GameMessages.ParsePlayerLeftMessage(message);
				mVGUIPlayerJoinedLeftLine.BuildLine(data5);
			}
			AddLine(mVGUIPlayerJoinedLeftLine);
			break;
		}
		case MVGameMsgType.CollectiblePickedUp:
		{
			MVGUICollectibleLine mVGUICollectibleLine = GetLine(collectiblePrefab) as MVGUICollectibleLine;
			GameMessages.CollectibleMessage data3 = GameMessages.ParseCollectibleMessage(message);
			mVGUICollectibleLine.BuildLine(data3);
			AddLine(mVGUICollectibleLine);
			break;
		}
		case MVGameMsgType.AchievementUnlocked:
		{
			MVGUIAchievementGetLine mVGUIAchievementGetLine = GetLine(achievementGetPrefab) as MVGUIAchievementGetLine;
			GameMessages.AchievementGetMessage data2 = GameMessages.ParseAchievementGetMessage(message);
			mVGUIAchievementGetLine.BuildLine(data2);
			AddLine(mVGUIAchievementGetLine);
			break;
		}
		case MVGameMsgType.CheckpointReached:
		{
			MVGUICheckpointLine mVGUICheckpointLine = GetLine(checkpointPrefab) as MVGUICheckpointLine;
			GameMessages.CheckpointMessage data = GameMessages.ParseCheckpointMessage(message);
			mVGUICheckpointLine.BuildLine(data);
			AddLine(mVGUICheckpointLine);
			break;
		}
		default:
			Debug.Log((object)("GameMsg of type " + msgType.ToString() + " received..."));
			break;
		}
	}

	private UXLine GetLine(UXLine prefab)
	{
		if (lines.Count == maxMessages && !queueMessages)
		{
			RemoveLine(lines.First());
		}
		for (int i = 0; i < cachedLines.Count; i++)
		{
			if ((object)((object)cachedLines[i]).GetType() == ((object)prefab).GetType())
			{
				UXLine uXLine = cachedLines[i];
				cachedLines.Remove(uXLine);
				uXLine.killed = false;
				return uXLine;
			}
		}
		return Object.Instantiate((Object)(object)prefab) as UXLine;
	}

	public void AddLine(UXLine line)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.Game.CameraController.CurCamera is FreeRoamCamera)
		{
			Object.Destroy((Object)(object)((Component)line).gameObject);
			return;
		}
		((Component)line).transform.parent = messageRoot;
		((Component)line).transform.localScale = Vector3.one;
		((Component)line).transform.localPosition = GetLinePosition(line);
		line.SetAlpha((lines.Count >= maxMessages) ? 0f : 1f);
		lines.Add(line);
		if (lines.Count == 1)
		{
			((MonoBehaviour)line).StartCoroutine(Fade(line));
		}
	}

	private void RemoveLine(UXLine line)
	{
		if (!line.killed)
		{
			lines.Remove(line);
			((MonoBehaviour)line).StopAllCoroutines();
			line.killed = true;
			line.SetAlpha(0f);
			cachedLines.Add(line);
			MoveLines();
		}
	}

	private void MoveLines()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].SetAlpha((i >= maxMessages) ? 0f : 1f);
			((Component)lines[i]).transform.localPosition = GetLinePosition(lines[i], i);
		}
		if (lines.Count > 0)
		{
			((MonoBehaviour)lines[0]).StartCoroutine(Fade(lines[0]));
		}
	}

	private Vector3 GetLinePosition(UXLine line, int stop = -1)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (stop == -1)
		{
			stop = lines.Count;
		}
		float num = 0f;
		for (int i = 0; i < stop; i++)
		{
			num += lines[i].GetLineSize().y;
		}
		return new Vector3(0f - line.GetLineSize().x / 2f, 0f - num, 1f);
	}

	private IEnumerator Fade(UXLine line)
	{
		yield return (object)new WaitForSeconds(messageStayTime);
		if (line.killed)
		{
			yield return null;
		}
		UXLine line2 = default;
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(messageFadeOutTime, 1f, 0f, (float t) =>
		{
			if (!line2.killed)
			{
				line2.SetAlpha(t);
			}
		}));
		RemoveLine(line);
	}
}
