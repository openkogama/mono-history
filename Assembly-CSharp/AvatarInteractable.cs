using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarInteractable : MVInteractable, IMoveHitHandler
{
	public class DamageSource
	{
		public static readonly DamageSource none = new DamageSource();

		public MVPlayer shooter;

		public PlayerKilledByType damageType;

		public float time;

		private const float lifeTime = 4f;

		public bool Outdated => Time.time - time > 4f;

		public DamageSource(MVPlayer shooter, PlayerKilledByType damageType)
		{
			this.shooter = shooter;
			this.damageType = damageType;
			time = Time.time;
		}

		private DamageSource()
		{
			time = 0f;
		}
	}

	public Action<float, MVPlayer, PlayerKilledByType> OnDamageTaken;

	public Action<Vector3> OnNewSafePosition;

	public Action OnShieldReplenished;

	private DamageSource lastDamageSource = DamageSource.none;

	private float boostedHealthMultiplier = 1f;

	private float damageMultiplier = 1f;

	private HashSet<PlayerKilledByType> KillNotificationBlacklist = new HashSet<PlayerKilledByType>
	{
		PlayerKilledByType.Environmental,
		PlayerKilledByType.Crushed,
		PlayerKilledByType.FallOffWorld,
		PlayerKilledByType.Impact
	};

	private readonly MaterialHitPackage[] hitPackages = new MaterialHitPackage[1]
	{
		new MaterialHitPackage(AvatarModifierPackageType.Poison, PrefabPool.Instance.PoisonParticles)
	};

	private InteractableMaterialHitHandler materialHitHandler = new InteractableMaterialHitHandler();

	private bool canWallJumpAnySurfaces;

	public DamageSource LastDamageSource => (!lastDamageSource.Outdated) ? lastDamageSource : null;

	public override void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariable<float> health, MVRuntimeDataVariable<int> maxHealth, MVRuntimeDataVariableClampedFloat shield, WorldObjectSkillDataManager skillDataManager)
	{
		base.Init(runtimeDataModifiers, health, maxHealth, shield, skillDataManager);
		InitializeSkills(skillDataManager);
		materialHitHandler.Initialize(hitPackages, transform);
		MVGameControllerBase.Game.LocalPlayer.BoostController.SubscribeToBoostChanged(BoostType.ExtraHealthFloatMultiplier, SetupBoostedHealthMultiplier);
		SetupBoostedHealthMultiplier();
		MVGameControllerBase.Game.LocalPlayer.BoostController.SubscribeToBoostChanged(BoostType.PoisonResistPercentage, HandlePoisonResistBoost);
		HandlePoisonResistBoost();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.Game.LocalPlayer.BoostController.UnSubscribeToBoostChanged(BoostType.ExtraHealthFloatMultiplier, SetupBoostedHealthMultiplier);
			MVGameControllerBase.Game.LocalPlayer.BoostController.UnSubscribeToBoostChanged(BoostType.PoisonResistPercentage, HandlePoisonResistBoost);
		}
	}

	public void InitializeSkills(WorldObjectSkillDataManager skillDataManager)
	{
		bool flag = skillDataManager.HasSkill("DamageReduction");
		damageMultiplier = ((!flag) ? 1f : ((float)(100 - skillDataManager.GetSkillIntValue("DamageReduction")) / 100f));
		canWallJumpAnySurfaces = skillDataManager.HasSkill("CanWallJumpAnySurface");
	}

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (amount > 0f)
		{
			if (IgnoreDamage(damageDealer))
			{
				return;
			}
		}
		else
		{
			if (IgnoreHealing(damageDealer))
			{
				return;
			}
			if (health.Value >= (float)maxHealth.Value)
			{
				float restoredShieldAmount = HandleModifierEffect(AvatarModifierEffect.OverHeal, 0f) * Time.deltaTime;
				RestoreShield(restoredShieldAmount);
			}
		}
		if (MVGameControllerBase.Game.IsPlaying && !HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			amount *= HandleModifierEffect(AvatarModifierEffect.DamageMultiplier, 1f);
			amount *= damageMultiplier;
			amount = DamageShield(amount);
			float value = health.Value;
			if (maxHealth != null)
			{
				float value2 = Mathf.Clamp(health.Value - amount, 0f, maxHealth.Value);
				health.Value = value2;
			}
			else
			{
				health.Value -= amount;
			}
			if (damageDealer != null)
			{
				lastDamageSource = new DamageSource(damageDealer, damageType);
			}
			if (OnDamageTaken != null)
			{
				OnDamageTaken(amount, damageDealer, damageType);
			}
			if (health.Value <= 0f && value > 0f)
			{
				DoKilledNotification(damageDealer, damageType);
			}
		}
	}

	private void DoKilledNotification(MVPlayer damageDealer, PlayerKilledByType defaultDamageType)
	{
		PlayerKilledByType playerKilledByType = defaultDamageType;
		int actorNr;
		if (damageDealer != null)
		{
			actorNr = damageDealer.ActorNr;
		}
		else if (LastDamageSource != null)
		{
			actorNr = LastDamageSource.shooter.ActorNr;
			playerKilledByType = LastDamageSource.damageType;
		}
		else
		{
			actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		}
		Dictionary<object, object> gameMsgData = GameMessages.MakePlayerKilledMessage(MVGameControllerBase.Game.LocalPlayer.ActorNr, actorNr, playerKilledByType);
		MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, gameMsgData);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)7, MVGameControllerBase.Game.LocalPlayer.ActorNr);
		dictionary.Add((byte)6, actorNr);
		dictionary.Add((byte)8, playerKilledByType);
		Dictionary<object, object> dictionary2 = dictionary;
		NotificationController.OnNotificationReceived(NotificationType.Kill, dictionary2);
		if (!KillNotificationBlacklist.Contains(playerKilledByType))
		{
			MVGameControllerBase.OperationRequests.PostNotificationOperation(NotificationType.Kill, dictionary2);
		}
	}

	private float GetBoostedHealth(float defaultHealth)
	{
		return defaultHealth * boostedHealthMultiplier;
	}

	public void DieFromRespawn(MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		float value = health.Value;
		health.Value = 0f;
		if (OnDamageTaken != null)
		{
			OnDamageTaken(value, damageDealer, damageType);
		}
		DoKilledNotification(damageDealer, damageType);
	}

	public void DieFromStuck()
	{
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		Dictionary<object, object> gameMsgData = GameMessages.MakePlayerKilledMessage(actorNr, actorNr, PlayerKilledByType.Crushed);
		MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, gameMsgData);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)7, MVGameControllerBase.Game.LocalPlayer.ActorNr);
		dictionary.Add((byte)6, actorNr);
		dictionary.Add((byte)8, PlayerKilledByType.Crushed);
		Dictionary<object, object> dictionary2 = dictionary;
		NotificationController.OnNotificationReceived(NotificationType.Kill, dictionary2);
		if (!KillNotificationBlacklist.Contains(PlayerKilledByType.Crushed))
		{
			MVGameControllerBase.OperationRequests.PostNotificationOperation(NotificationType.Kill, dictionary2);
		}
		OnDamageTaken(1000f, null, PlayerKilledByType.Crushed);
	}

	public void DieFromFalling()
	{
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		Dictionary<object, object> gameMsgData = GameMessages.MakePlayerKilledMessage(actorNr, actorNr, PlayerKilledByType.FallOffWorld);
		MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, gameMsgData);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)7, MVGameControllerBase.Game.LocalPlayer.ActorNr);
		dictionary.Add((byte)6, actorNr);
		dictionary.Add((byte)8, PlayerKilledByType.FallOffWorld);
		Dictionary<object, object> dictionary2 = dictionary;
		NotificationController.OnNotificationReceived(NotificationType.Kill, dictionary2);
		if (!KillNotificationBlacklist.Contains(PlayerKilledByType.FallOffWorld))
		{
			MVGameControllerBase.OperationRequests.PostNotificationOperation(NotificationType.Kill, dictionary2);
		}
		OnDamageTaken(1000f, null, PlayerKilledByType.FallOffWorld);
	}

	public void DieFromBeingStuck()
	{
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		Dictionary<object, object> gameMsgData = GameMessages.MakePlayerKilledMessage(actorNr, actorNr, PlayerKilledByType.Crushed);
		MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, gameMsgData);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)7, MVGameControllerBase.Game.LocalPlayer.ActorNr);
		dictionary.Add((byte)6, actorNr);
		dictionary.Add((byte)8, PlayerKilledByType.Crushed);
		Dictionary<object, object> dictionary2 = dictionary;
		NotificationController.OnNotificationReceived(NotificationType.Kill, dictionary2);
		if (!KillNotificationBlacklist.Contains(PlayerKilledByType.Crushed))
		{
			MVGameControllerBase.OperationRequests.PostNotificationOperation(NotificationType.Kill, dictionary2);
		}
		OnDamageTaken(1000f, null, PlayerKilledByType.Crushed);
	}

	protected override void RestoreShield(float restoredShieldAmount)
	{
		base.RestoreShield(restoredShieldAmount);
		if (OnShieldReplenished != null)
		{
			OnShieldReplenished();
		}
	}

	private float DamageShield(float amount)
	{
		if (amount < 0f)
		{
			return amount;
		}
		if (amount > shield.Value)
		{
			float result = amount - shield.Value;
			shield.Value = 0f;
			return result;
		}
		shield.Value -= amount;
		return 0f;
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (MVGameControllerBase.Game.IsPlaying)
		{
			BitArray bitArray = new BitArray(20);
			bitArray.Set(0, value: true);
			bitArray.Set(4, HasModifierEffect(AvatarModifierEffect.PoisonImmune));
			if (!bitArray[(int)type])
			{
				base.AddModifier(type, id, additionalModifers);
			}
		}
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		AvatarModifierPackageType modifierPackageType = moveHit.material.ModifierPackageType;
		if (modifierPackageType != AvatarModifierPackageType.None)
		{
			AddModifier(moveHit.material.ModifierPackageType);
		}
		if (modifierPackageType == AvatarModifierPackageType.None && IsGroundedSafely(moveHit) && MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.Value == SpawnRoleModeType.Playing && OnNewSafePosition != null)
		{
			OnNewSafePosition(moveHit.positionTouchingHit);
		}
		if (canWallJumpAnySurfaces)
		{
			AddModifier(AvatarModifierPackageType.WallJump);
		}
		materialHitHandler.HandleHit(moveHit);
	}

	private bool IsGroundedSafely(MVControllerColliderHit moveHit)
	{
		float friction = moveHit.material.PhysicalProperties.friction;
		float y = moveHit.slopeNormal.y;
		float num = y * friction;
		float sqrMagnitude = moveHit.impactVelocity.sqrMagnitude;
		bool flag = sqrMagnitude < 1000f;
		if (!flag)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value.SuppressSafeSpotSaving(1f);
		}
		return num >= 0.35f && y >= 0.6f && friction > 0.2f && flag;
	}

	private void SetupBoostedHealthMultiplier()
	{
		boostedHealthMultiplier = 1f;
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.ExtraHealthFloatMultiplier, out var boost))
		{
			boostedHealthMultiplier = 1f + (float)(int)boost.Value / 100f;
		}
	}

	private void HandlePoisonResistBoost()
	{
		poisonResist = 0f;
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.PoisonResistPercentage, out var boost))
		{
			poisonResist = (float)(int)boost.Value / 100f;
		}
	}
}
