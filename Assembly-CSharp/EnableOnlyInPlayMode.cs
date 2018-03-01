using MV.Common;
using UnityEngine;

public class EnableOnlyInPlayMode : MonoBehaviour
{
	private void Update()
	{
		MVPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		if ((localPlayer == null || !localPlayer.IsReady || !MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Playing)) && gameObject.activeSelf)
		{
			gameObject.SetActive(value: false);
		}
	}
}
