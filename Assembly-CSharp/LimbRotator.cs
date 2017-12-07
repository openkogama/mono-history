using System.Collections.Generic;
using UnityEngine;

public class LimbRotator
{
	private Dictionary<BodyData.PartIndex, LimbController> limbControllers;

	private bool isActive = true;

	public LimbController GetLimbController(BodyData.PartIndex partIndex)
	{
		if (HasLimbController(partIndex))
		{
			return limbControllers[partIndex];
		}
		return null;
	}

	private bool HasLimbController(BodyData.PartIndex partIndex)
	{
		if (limbControllers.ContainsKey(partIndex))
		{
			return true;
		}
		Debug.LogError(string.Concat("LimbRotator does not have a limb controller for  ", partIndex, ". Returning null instead"));
		return false;
	}

	public void Initialize(MVAvatar avatar, AvatarLimbManager limbManager)
	{
		limbControllers = new Dictionary<BodyData.PartIndex, LimbController>();
		CreateLimbController(BodyData.PartIndex.Torso, avatar, limbManager);
		CreateLimbController(BodyData.PartIndex.Head, avatar, limbManager);
		CreateLimbController(BodyData.PartIndex.RArm, avatar, limbManager);
		CreateLimbController(BodyData.PartIndex.LArm, avatar, limbManager);
	}

	private void CreateLimbController(BodyData.PartIndex partIndex, MVAvatar avatar, AvatarLimbManager limbManager)
	{
		switch (partIndex)
		{
		case BodyData.PartIndex.Head:
		{
			LimbController limbController4 = new LimbController();
			List<string> blendAnimations4 = new List<string>();
			List<string> list4 = new List<string>();
			list4.Add("Dead");
			limbController4.Initialize(limbManager, avatar, partIndex, Quaternion.identity, Quaternion.identity, blendAnimations4, list4, 89f, 45f);
			limbControllers.Add(partIndex, limbController4);
			break;
		}
		case BodyData.PartIndex.Torso:
		{
			LimbController limbController3 = new LimbController();
			List<string> blendAnimations3 = new List<string>();
			List<string> list3 = new List<string>();
			list3.Add("Dead");
			limbController3.Initialize(limbManager, avatar, partIndex, Quaternion.identity, Quaternion.identity, blendAnimations3, list3, 90f, 45f);
			limbControllers.Add(partIndex, limbController3);
			break;
		}
		case BodyData.PartIndex.RArm:
		{
			LimbController limbController2 = new LimbController();
			List<string> blendAnimations2 = new List<string>();
			List<string> list2 = new List<string>();
			list2.Add("Dead");
			list2.Add("Jump");
			Quaternion identity3 = Quaternion.identity;
			identity3.eulerAngles = new Vector3(0f, 270f, 0f);
			Quaternion identity4 = Quaternion.identity;
			identity4.eulerAngles = new Vector3(355.2f, 359f, 282f);
			limbController2.Initialize(limbManager, avatar, partIndex, identity3, identity4, blendAnimations2, list2, 90f, 45f);
			limbControllers.Add(partIndex, limbController2);
			break;
		}
		case BodyData.PartIndex.LArm:
		{
			LimbController limbController = new LimbController();
			List<string> blendAnimations = new List<string>();
			List<string> list = new List<string>();
			list.Add("Dead");
			list.Add("Jump");
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(0f, 90f, 0f);
			Quaternion identity2 = Quaternion.identity;
			identity2.eulerAngles = new Vector3(6.2f, 358.5f, 76.2f);
			limbController.Initialize(limbManager, avatar, partIndex, identity, identity2, blendAnimations, list, 90f, 45f);
			limbControllers.Add(partIndex, limbController);
			break;
		}
		}
	}

	public void UpdateLimbs()
	{
		if (!isActive || MVGameControllerBase.CameraController.BlueModeEnabled)
		{
			return;
		}
		foreach (KeyValuePair<BodyData.PartIndex, LimbController> limbController in limbControllers)
		{
			limbController.Value.UpdateRotation();
		}
	}

	public void TrySetLimbRotation(BodyData.PartIndex partIndex, Quaternion limbYawRotation, Quaternion limbPitchRotation)
	{
		if (HasLimbController(partIndex))
		{
			limbControllers[partIndex].TrySetNewRotation(limbYawRotation, limbPitchRotation);
		}
	}

	public void SetLimbRotation(BodyData.PartIndex partIndex, Quaternion limbYawRotation, Quaternion limbPitchRotation, float duration)
	{
		if (HasLimbController(partIndex))
		{
			limbControllers[partIndex].TrySetNewRotation(limbYawRotation, limbPitchRotation, duration);
		}
	}

	public void StopLimbRotation(BodyData.PartIndex partIndex)
	{
		limbControllers[partIndex].StopRotating();
	}

	public void StartBlendingWithAnimation(BodyData.PartIndex partIndex, string animation)
	{
		if (HasLimbController(partIndex))
		{
			limbControllers[partIndex].StartBlendingWithAnimation(animation);
		}
	}

	public void StopBlendingWithAnimation(BodyData.PartIndex partIndex, string animation)
	{
		if (HasLimbController(partIndex))
		{
			limbControllers[partIndex].StopBlendingWithAnimation(animation);
		}
	}

	public void SetIsActive(bool shouldBeActive)
	{
		isActive = shouldBeActive;
	}
}
