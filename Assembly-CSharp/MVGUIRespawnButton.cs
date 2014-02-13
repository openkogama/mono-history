using System;
using UnityEngine;

public class MVGUIRespawnButton : MonoBehaviour
{
	public UXIconButton respawn;

	public void Initialize()
	{
		UXIconButton uXIconButton = respawn;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.IngameController.RespawnAvatar();
		}));
	}
}
