using UnityEngine;

public class AvatarWaveEmote : AvatarEmote
{
	private const float waveAngle = 25f;

	private const float amountOfRotations = 8f;

	private LimbController RArmController;

	private LimbController LArmController;

	private float originalInterpolationSpeed;

	public override void Initialize(AvatarLimbManager limbManager, float lifeTime)
	{
		base.Initialize(limbManager, lifeTime);
		RArmController = limbManager.LimbRotator.GetLimbController(BodyData.PartIndex.RArm);
		LArmController = limbManager.LimbRotator.GetLimbController(BodyData.PartIndex.LArm);
		originalInterpolationSpeed = RArmController.InterpolationSpeed;
		emote = EmoteTypes.wave;
	}

	public override void StartEmote()
	{
		base.StartEmote();
		RArmController.InterpolationSpeed = 8f / lifeTime * 0.5f;
		RArmController.IsEventControllingLimb = true;
		LArmController.InterpolationSpeed = 8f / lifeTime * 0.5f;
		LArmController.IsEventControllingLimb = true;
	}

	public override void StopEmote()
	{
		base.StopEmote();
		RArmController.InterpolationSpeed = originalInterpolationSpeed;
		RArmController.IsEventControllingLimb = false;
		RArmController.StopRotating();
		LArmController.InterpolationSpeed = originalInterpolationSpeed;
		LArmController.IsEventControllingLimb = false;
		LArmController.StopRotating();
	}

	public override void Update()
	{
		if (isActive)
		{
			duration -= Time.deltaTime;
			if (duration <= 0f)
			{
				StopEmote();
				return;
			}
			HandleArmRotation(RArmController, 65f);
			HandleArmRotation(LArmController, 295f);
		}
	}

	private void HandleArmRotation(LimbController armController, float yawAngle)
	{
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, yawAngle, 0f);
		float num = duration / lifeTime;
		num *= 0.8f;
		num = Mathf.Floor(num * 10f) + 1f;
		if (num % 2f == 0f)
		{
			MoveArmDownwards(armController, identity);
		}
		else
		{
			MoveArmUpwards(armController, identity);
		}
	}

	private void MoveArmUpwards(LimbController armController, Quaternion yawRotation)
	{
		float num = -25f;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(335f + num, 0f, 0f);
		if (armController.InterpolateTowardsPitchRotation != identity)
		{
			armController.ResetInterpolation();
			armController.SetNewRotation(yawRotation, identity);
		}
	}

	private void MoveArmDownwards(LimbController armController, Quaternion yawRotation)
	{
		float num = -25f;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(25f + num, 0f, 0f);
		if (armController.InterpolateTowardsPitchRotation != identity)
		{
			armController.ResetInterpolation();
			armController.SetNewRotation(yawRotation, identity);
		}
	}
}
