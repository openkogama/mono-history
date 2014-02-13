using UnityEngine;

public class VehicleSeatHideLegs : VehicleSeatBase
{
	public override void Attach(MVAvatar avatar)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.Attach(avatar);
		if ((Object)(object)avatar.Body.BodyData != (Object)null)
		{
			if ((Object)(object)avatar.Body.BodyData.GetPartBone("RLowLeg") != (Object)null)
			{
				((Component)avatar.Body.BodyData.GetPartBone("RLowLeg")).transform.localScale = Vector3.one * 0.001f;
			}
			if ((Object)(object)avatar.Body.BodyData.GetPartBone("LLowLeg") != (Object)null)
			{
				((Component)avatar.Body.BodyData.GetPartBone("LLowLeg")).transform.localScale = Vector3.one * 0.001f;
			}
		}
	}

	public override void Detach(MVAvatar avatar)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		base.Detach(avatar);
		if ((Object)(object)avatar.Body.BodyData != (Object)null)
		{
			if ((Object)(object)avatar.Body.BodyData.GetPartBone("RLowLeg") != (Object)null)
			{
				((Component)avatar.Body.BodyData.GetPartBone("RLowLeg")).transform.localScale = Vector3.one;
			}
			if ((Object)(object)avatar.Body.BodyData.GetPartBone("LLowLeg") != (Object)null)
			{
				((Component)avatar.Body.BodyData.GetPartBone("LLowLeg")).transform.localScale = Vector3.one;
			}
		}
	}
}
