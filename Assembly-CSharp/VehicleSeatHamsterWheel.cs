using UnityEngine;

public class VehicleSeatHamsterWheel : VehicleSeatBase
{
	private GameObject newParent;

	private Transform oldParent;

	private Vector3 oldLocalPos = Vector3.zero;

	public VehicleSeatHamsterWheel()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Attach(MVAvatar avatar)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		base.Attach(avatar);
		oldParent = avatar.Body.Transform.parent;
		oldLocalPos = avatar.Body.Transform.localPosition;
		newParent = new GameObject("HamsterWheelRotationRoot");
		newParent.transform.parent = avatar.Body.Transform.parent;
		newParent.transform.localPosition = Vector3.zero;
		newParent.transform.localRotation = Quaternion.identity;
		avatar.Body.Transform.parent = newParent.transform;
		Transform transform = avatar.Body.Transform;
		transform.localPosition += new Vector3(0f, -0.9f, 0f);
		((Behaviour)avatar.Body.BlobShadow).enabled = false;
	}

	public override void Detach(MVAvatar avatar)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		avatar.Body.Transform.parent = oldParent;
		avatar.Body.Transform.localRotation = Quaternion.identity;
		avatar.Body.Transform.localPosition = oldLocalPos;
		Object.Destroy((Object)(object)newParent);
		((Behaviour)avatar.Body.BlobShadow).enabled = true;
		base.Detach(avatar);
	}
}
