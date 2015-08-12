using UnityEngine;

public class VehicleSeatHideLegs : VehicleSeatBase
{
	public override void Attach(MVAvatar avatar)
	{
		base.Attach(avatar);
		if (avatar.Body.BodyData != null)
		{
			if (avatar.Body.BodyData.GetPartBone("RLowLeg") != null)
			{
				avatar.Body.BodyData.GetPartBone("RLowLeg").transform.localScale = Vector3.one * 0.001f;
			}
			if (avatar.Body.BodyData.GetPartBone("LLowLeg") != null)
			{
				avatar.Body.BodyData.GetPartBone("LLowLeg").transform.localScale = Vector3.one * 0.001f;
			}
		}
	}

	public override void Detach(MVAvatar avatar)
	{
		base.Detach(avatar);
		if (avatar.Body.BodyData != null)
		{
			if (avatar.Body.BodyData.GetPartBone("RLowLeg") != null)
			{
				avatar.Body.BodyData.GetPartBone("RLowLeg").transform.localScale = Vector3.one;
			}
			if (avatar.Body.BodyData.GetPartBone("LLowLeg") != null)
			{
				avatar.Body.BodyData.GetPartBone("LLowLeg").transform.localScale = Vector3.one;
			}
		}
	}
}
