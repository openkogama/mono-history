using System;
using UnityEngine;

public class AvatarLocal : Avatar
{
	public override void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		base.Initialize(mvAvatar, isLocal);
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXpProgress));
	}

	private void OnXpProgress(XPProgressData xpProgressData)
	{
		AvatarPooledXPParticles avatarPooledXPParticles = PrefabPool.Instance.EnumPoolManager.Instantiate<AvatarPooledXPParticles>(PoolEnums.XP);
		avatarPooledXPParticles.transform.parent = transform;
		avatarPooledXPParticles.transform.localPosition = Vector3.up;
		avatarPooledXPParticles.transform.localRotation = Quaternion.identity;
		avatarPooledXPParticles.transform.localScale = Vector3.one;
		avatarPooledXPParticles.gameObject.layer = mvAvatar.Body.GameObject.layer;
		avatarPooledXPParticles.Initialize(xpProgressData.XPDelta);
		avatarPooledXPParticles.Play();
	}
}
