using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public abstract class MVBuildModeAvatar : MVGroup, IUpdatecontrollerSubscriberLateUpdate, IUpdatecontrollerSubscriberBase
{
	protected MVBody body;

	protected AvatarLimbManager limbManager;

	protected MVRuntimeDataVariable CurrentItem;

	protected LimbRotationRuntimeData limbRotationRuntimeData = new LimbRotationRuntimeData();

	public AvatarLimbManager LimbManager => limbManager;

	public MVBuildModeAvatar(Dictionary<object, object> data, GameObject prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		CurrentItem = RuntimeDataVariables.New("currentItem", 0f, writeThrough: true);
		limbRotationRuntimeData.HeadRotationYaw = RuntimeDataVariables.New("headRotationYaw", 0.8f, writeThrough: false);
		limbRotationRuntimeData.HeadRotationPitch = RuntimeDataVariables.New("headRotationPitch", 0.8f, writeThrough: false);
		limbRotationRuntimeData.PointRotationYaw = RuntimeDataVariables.New("pointRotationYaw", 0.8f, writeThrough: false);
		limbRotationRuntimeData.PointRotationPitch = RuntimeDataVariables.New("pointRotationPitch", 0.8f, writeThrough: false);
		limbRotationRuntimeData.Emote = RuntimeDataVariables.New("emote", 0.5f, writeThrough: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		UpdateController.AddLateUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public override void Destroy()
	{
		UpdateController.RemoveLateUpdateObject(this);
		base.Destroy();
	}

	public override void AddChild(MVWorldObjectClient child)
	{
		base.AddChild(child);
		body = child as MVBody;
		body.GameObject.SetActive(value: false);
	}

	public void UpdateControllerLateUpdate()
	{
		limbManager.UpdateLimbRotations(GetLookDirection());
	}

	protected LaserPointer InitLaser(bool isLocal)
	{
		LaserPointer laserPointer = PickupItem.InstantiateAvatarItemType(AvatarItemType.LaserPointer).GetComponent<LaserPointer>();
		laserPointer.Initialize(isLocal, CurrentItem, Transform);
		laserPointer.OnEquip();
		return laserPointer;
	}

	protected abstract Vector3 GetLookDirection();
}
