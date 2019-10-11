using MV.Common;
using UnityEngine;

public class WaterState
{
	private const float suffocationDamage = 5f;

	private const float avatarHeight = 2.1f;

	private readonly AvatarModifierPackage.AvatarModifier[] additionalUnderWaterModifiers;

	private readonly float oxygenMax = 20f;

	private float oxygen = 20f;

	private Vector3 worldPosition = Vector3.zero;

	private bool hasGillsNoLungs;

	public WaterState(WorldObjectSkillDataManager skillDataManager)
	{
		additionalUnderWaterModifiers = new AvatarModifierPackage.AvatarModifier[2]
		{
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.Speed, UnderwaterModifierCallback),
			new AvatarModifierPackage.AvatarModifier(AvatarModifierType.Multiply, AvatarModifierEffect.JumpPower, UnderwaterJumpPowerModifierCallback)
		};
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange += OnAvatarModeChange;
		bool flag = skillDataManager.HasSkill("OxygenSupply");
		oxygenMax = ((!flag) ? 20f : skillDataManager.GetSkillFloatValue("OxygenSupply"));
		oxygen = oxygenMax;
		hasGillsNoLungs = skillDataManager.HasSkill("BreathesWater");
	}

	public void Destroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange -= OnAvatarModeChange;
		}
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
		bool flag = num >= 0.6f;
		if (flag ^ hasGillsNoLungs)
		{
			oxygen = Mathf.Max(0f, oxygen - Time.deltaTime);
			if (oxygen <= 0f)
			{
				avatarInteractable.TakeDamage(5f * Time.deltaTime, null, PlayerKilledByType.Environmental);
			}
		}
		else
		{
			oxygen = oxygenMax;
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

	private void OnAvatarModeChange(SpawnRoleModeType newSpawnRoleMode)
	{
		oxygen = oxygenMax;
	}
}
