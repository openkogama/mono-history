using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class FlagWinningCondition : WinningConditionBase
{
	private const float updateFlagFrequency = 10f;

	[SerializeField]
	private Image flagIcon;

	[SerializeField]
	private GameObject graphics;

	[SerializeField]
	private Text score;

	protected override GameStatCounterType StatType => GameStatCounterType.Flag;

	public override bool WinningConditionAbleToBeFulfilled => CanWinningConditionBeFullfilledForTeam(MVGameControllerBase.Game.LocalPlayer.Team);

	public override void InitializeGameUI(RectTransform lobbyState)
	{
		UpdateFlagGraphics();
		MVPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		UpdateValue(MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.Flag, localPlayer.Team, localPlayer.ActorNr));
		base.InitializeGameUI(lobbyState);
	}

	public override void UpdateValue(int newValue)
	{
		MVPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		score.text = WinningConditionControl.MakeIntoScoreText(MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.Flag, localPlayer.Team, localPlayer.ActorNr), GameStatCounterType.Flag);
	}

	public override bool CanWinningConditionBeFullfilledForTeam(MVTeam team)
	{
		return GetClosestFlags(team).Count > 0;
	}

	private void Update()
	{
		if ((float)Time.frameCount % 10f == 0f)
		{
			UpdateFlagGraphics();
		}
	}

	private List<MVWorldObjectClient> GetClosestFlags(MVTeam team)
	{
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.Flag);
		for (int num = worldObjectsByType.Count - 1; num >= 0; num--)
		{
			if (worldObjectsByType[num] is MVFlag mVFlag && mVFlag.Data.ContainsKey("team"))
			{
				bool flag = (int)mVFlag.Data["team"] == (int)team;
				bool flag2 = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
				if (!flag)
				{
					worldObjectsByType.RemoveAt(num);
				}
				else if (!flag2)
				{
					worldObjectsByType.RemoveAt(num);
				}
			}
		}
		return worldObjectsByType;
	}

	private void UpdateFlagGraphics()
	{
		List<MVWorldObjectClient> closestFlags = GetClosestFlags(MVGameControllerBase.Game.LocalPlayer.Team);
		if (closestFlags.Count == 0)
		{
			graphics.SetActive(value: false);
		}
		else if (!graphics.activeSelf)
		{
			graphics.SetActive(value: true);
		}
	}

	public override void RoundEndReset()
	{
		score.text = WinningConditionControl.MakeIntoScoreText(0, GameStatCounterType.Flag);
		base.RoundEndReset();
	}
}
