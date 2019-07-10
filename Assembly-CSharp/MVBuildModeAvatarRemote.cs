using System.Collections.Generic;
using UnityEngine;

public class MVBuildModeAvatarRemote : MVBuildModeAvatar, ISpawnRoleRemote
{
	private LaserPointer laserPointer;

	private AvatarRemoteBuildMode avatarRemoteBuildMode;

	private DynamicCullingHandler cullingHandler = new DynamicCullingHandler(3.5f);

	public MVBuildModeAvatarRemote(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVAvatarRemoteBuildModePrefab, worldObjects)
	{
		avatarRemoteBuildMode = gameObject.GetComponent<AvatarRemoteBuildMode>();
		SetNetworkObject(local: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		avatarRemoteBuildMode.Initialize(OwnerActorNr, this);
		limbManager = new AvatarLimbManagerRemote();
		limbManager.Initialize(this, body, avatarRemoteBuildMode.EnabledChangeHandler, limbRotationRuntimeData);
		body.StartAnimation("Idle");
		body.GameObject.SetActive(value: true);
		gameObject.SetActive(value: false);
		MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(OwnerActorNr).NotifyAvatarCreated(Id);
		laserPointer = InitLaser(isLocal: false);
	}

	public override void Destroy()
	{
		base.Destroy();
		cullingHandler.DeActivateCulling();
	}

	public void Activate(int idFrom, Vector3 position, Quaternion rotation)
	{
		Debug.Log("MVBuildModeAvatarRemote Activate");
		Position = position;
		Rotation = rotation;
		((MVNetworkListener)MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(Id))?.SetToCurrentPosition();
		gameObject.SetActive(value: true);
		SetLaserPointerVisibility(isVisible: true);
		avatarRemoteBuildMode.Activate();
		cullingHandler.ActivateCulling(gameObject);
	}

	public void DeActivate(int idTo)
	{
		gameObject.SetActive(value: false);
		SetLaserPointerVisibility(isVisible: false);
		avatarRemoteBuildMode.Deactivate();
		cullingHandler.DeActivateCulling();
	}

	private void SetLaserPointerVisibility(bool isVisible)
	{
		if (laserPointer != null)
		{
			laserPointer.gameObject.SetActive(value: false);
		}
	}

	protected override Vector3 GetLookDirection()
	{
		return transform.forward.normalized;
	}
}
