using UnityEngine;

public class VehicleSeatHamsterWheel : VehicleSeatBase
{
	private GameObject newParent;

	private Transform oldParent;

	private Vector3 oldLocalPos = Vector3.zero;

	public override void Attach(MVAvatar avatar)
	{
		base.Attach(avatar);
		oldParent = avatar.Body.Transform.parent;
		oldLocalPos = avatar.Body.Transform.localPosition;
		newParent = new GameObject("HamsterWheelRotationRoot");
		newParent.transform.parent = avatar.Body.Transform.parent;
		newParent.transform.localPosition = Vector3.zero;
		newParent.transform.localRotation = Quaternion.identity;
		avatar.Body.Transform.parent = newParent.transform;
		avatar.Body.Transform.localPosition += new Vector3(0f, -0.9f, 0f);
		avatar.Body.BlobShadow.enabled = false;
	}

	public override void Detach(MVAvatar avatar)
	{
		avatar.Body.Transform.parent = oldParent;
		avatar.Body.Transform.localRotation = Quaternion.identity;
		avatar.Body.Transform.localPosition = oldLocalPos;
		Object.Destroy(newParent);
		avatar.Body.BlobShadow.enabled = true;
		base.Detach(avatar);
	}
}
