using UnityEngine;

public class AvatarLocal : Avatar
{
	private IAvatarCameraController avatarCameraController;

	[SerializeField]
	private AvatarCamerasMobile avatarCamerasMobile;

	[SerializeField]
	private AvatarCamerasDesktop avatarCamerasDesktop;

	public IAvatarCameraController CameraController => avatarCameraController;

	public override void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		base.Initialize(mvAvatar, isLocal);
		avatarCameraController = Object.Instantiate(avatarCamerasDesktop);
		avatarCameraController.Initialize((MVAvatarLocal)mvAvatar);
		PrewarmXPParticles();
	}

	private void OnDestroy()
	{
		MonoBehaviour monoBehaviour = avatarCameraController as MonoBehaviour;
		if (monoBehaviour != null)
		{
			Object.Destroy(monoBehaviour.gameObject);
			avatarCameraController = null;
		}
	}

	private void PrewarmXPParticles()
	{
		CreateXPParticlesWithLayer(0, LayerUtil.GetLayerNumber(LayerFlags.Hidden));
	}

	private void CreateXPParticlesWithLayer(int xp, int layer)
	{
		AvatarPooledXPParticles avatarPooledXPParticles = PrefabPool.Instance.EnumPoolManager.Instantiate<AvatarPooledXPParticles>(PoolEnums.XP);
		avatarPooledXPParticles.transform.parent = transform;
		avatarPooledXPParticles.transform.localPosition = Vector3.up;
		avatarPooledXPParticles.transform.localRotation = Quaternion.identity;
		avatarPooledXPParticles.transform.localScale = Vector3.one;
		avatarPooledXPParticles.gameObject.layer = layer;
		avatarPooledXPParticles.Initialize(xp);
	}

	public void OnXpProgressing(int xp)
	{
		Debug.Log("OnXpProgressing");
		CreateXPParticlesWithLayer(xp, mvAvatar.Body.GameObject.layer);
	}
}
