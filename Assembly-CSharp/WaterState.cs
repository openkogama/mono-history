using MV.Common;
using UnityEngine;

public class WaterState
{
	private readonly AvatarModifierPackage.AvatarModifier[] additionalUnderWaterModifiers;

	private float localAvatarOxygen = 100f;

	private const float avatarHeight = 2.1f;

	private Vector3 worldPosition = Vector3.zero;

	public WaterState()
	{
		additionalUnderWaterModifiers = new AvatarModifierPackage.AvatarModifier[2]
		{
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, UnderwaterModifierCallback),
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, UnderwaterJumpPowerModifierCallback)
		};
	}

	public void Update(Vector3 worldPosition, MVInteractableBase avatarInteractable)
	{
		this.worldPosition = worldPosition;
		UpdateModifiers(avatarInteractable);
		UpdateLocalAvatarOxygen(avatarInteractable);
	}

	private void UpdateModifiers(MVInteractableBase avatarInteractable)
	{
		if (worldPosition.y < MVGameControllerBase.WaterPlaneManager.WaterLevel && MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			avatarInteractable.AddModifier(AvatarModifierPackageType.Underwater, -1, additionalUnderWaterModifiers);
		}
		else if (avatarInteractable.HasModifier(AvatarModifierPackageType.Underwater))
		{
			avatarInteractable.RemoveModifier(AvatarModifierPackageType.Underwater);
		}
	}

	private void UpdateLocalAvatarOxygen(MVInteractableBase avatarInteractable)
	{
		float num = ComputeAvatarWaterProximity(worldPosition);
		if (num >= 0.6f)
		{
			localAvatarOxygen = Mathf.Max(0f, localAvatarOxygen - Time.deltaTime * 5f);
		}
		else
		{
			localAvatarOxygen = Mathf.Min(100f, localAvatarOxygen + Time.deltaTime * 25f);
		}
		if (localAvatarOxygen <= 0f)
		{
			avatarInteractable.TakeDamage(5f * Time.deltaTime, null, PlayerKilledByType.Environmental);
		}
	}

	private float UnderwaterJumpPowerModifierCallback()
	{
		float num = MVGameControllerBase.WaterPlaneManager.ComputeAvatarWaterProximity(worldPosition);
		return (!((double)num > 0.7)) ? 1f : 2f;
	}

	private float UnderwaterModifierCallback()
	{
		float num = 1f - ComputeAvatarWaterProximity(worldPosition);
		return num * 0.3f + 0.7f;
	}

	private float ComputeAvatarWaterProximity(Vector3 position)
	{
		if (!MVGameControllerBase.WaterPlaneManager.IsActive)
		{
			return 0f;
		}
		return Mathf.Clamp01((MVGameControllerBase.WaterPlaneManager.WaterLevel - position.y) / 2.1f);
	}
}
