using UnityEngine;

public class AvatarNodEmote : AvatarEmote
{
	private const float nodAngle = 15f;

	private const float amountOfRotations = 5f;

	private LimbController headController;

	private float originalInterpolationSpeed;

	public override void Initialize(AvatarLimbManager limbManager, float lifeTime)
	{
		base.Initialize(limbManager, lifeTime);
		headController = limbManager.LimbRotator.GetLimbController(BodyData.PartIndex.Head);
		originalInterpolationSpeed = headController.InterpolationSpeed;
		emote = EmoteTypes.Nod;
	}

	public override void StartEmote()
	{
		base.StartEmote();
		headController.InterpolationSpeed = 5f / lifeTime * 0.5f;
		headController.IsEventControllingLimb = true;
	}

	public override void StopEmote()
	{
		base.StopEmote();
		headController.InterpolationSpeed = originalInterpolationSpeed;
		headController.IsEventControllingLimb = false;
	}

	public override void Update()
	{
		if (isActive)
		{
			duration -= Time.deltaTime;
			if (duration < 0f)
			{
				StopEmote();
			}
			HandleRotation();
		}
	}

	private void HandleRotation()
	{
		float num = duration / lifeTime;
		num = num * 5f / 10f;
		num = Mathf.Floor(num * 10f) + 1f;
		Quaternion identity = Quaternion.identity;
		if (num % 2f == 0f)
		{
			identity.eulerAngles = new Vector3(15f, 0f, 0f);
		}
		else
		{
			identity.eulerAngles = new Vector3(345f, 0f, 0f);
		}
		RotateHead(identity);
	}

	private void RotateHead(Quaternion pitchRotation)
	{
		if (headController.InterpolateTowardsPitchRotation != pitchRotation)
		{
			headController.ResetInterpolation();
			headController.SetNewRotation(headController.InterpolateTowardsYawRotation, pitchRotation);
		}
	}
}
