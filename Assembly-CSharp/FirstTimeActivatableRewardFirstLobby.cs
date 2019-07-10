using UnityEngine;

public class FirstTimeActivatableRewardFirstLobby : FirstTimeActivatableSetEventOnShow
{
	public override void OnShow()
	{
		base.OnShow();
		Debug.Log(FirstTimeEvent);
	}
}
