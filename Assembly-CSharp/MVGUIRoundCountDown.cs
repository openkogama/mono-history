using MV.Common;
using UnityEngine;

public class MVGUIRoundCountDown : MonoBehaviour
{
	[SerializeField]
	protected UXText timeCounter;

	private void Update()
	{
		if (MVGameController.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.PrepareRound)
		{
			timeCounter.Text = (Mathf.Ceil(MVGameController.Game.NetworkGameStateListener.TimeLeftMS / 1000) + 1f).ToString();
		}
	}
}
