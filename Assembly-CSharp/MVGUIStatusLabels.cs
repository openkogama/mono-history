using System;
using Localize;
using UnityEngine;

public class MVGUIStatusLabels : UXViewScript
{
	private MVJoinState prevGameState = MVJoinState.Leaving;

	private MVConnState prevConnState;

	public UXText connState;

	public UXText gameState;

	public void Update()
	{
		if (!((Object)(object)MVGameController.Instance != (Object)null) || MVGameController.Instance.Game == null)
		{
			return;
		}
		MVConnState mVConnState = MVGameController.Instance.Game.ConnState;
		MVJoinState joinState = MVGameController.Instance.Game.JoinState;
		if (mVConnState != prevConnState)
		{
			prevConnState = mVConnState;
			connState.Text = Localization.Instance.GetText(TextSlotIndex.Connection) + ": " + Localization.Instance.GetText((TextSlotIndex)(int)Enum.Parse(typeof(TextSlotIndex), "Connection" + mVConnState));
		}
		if (joinState != prevGameState)
		{
			prevGameState = joinState;
			try
			{
				gameState.Text = Localization.Instance.GetText(TextSlotIndex.Game) + ": " + Localization.Instance.GetText((TextSlotIndex)(int)Enum.Parse(typeof(TextSlotIndex), "Join" + joinState));
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)string.Concat(new object[4] { "Issue with localization and join state: ", joinState, " exception: \n", ex.Message }));
			}
			if (joinState == MVJoinState.Playing)
			{
				gameState.Text = string.Empty;
				connState.Text = string.Empty;
			}
		}
	}
}
