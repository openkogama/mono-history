using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarInteractable : MVInteractable, IMoveHitHandler
{
	public class DamageSource
	{
		private const float lifeTime = 4f;

		public static readonly DamageSource none = new DamageSource();

		public MVPlayer shooter;

		public PlayerKilledByType damageType;

		public float time;

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

	public Action OnShieldReplenished;

	private DamageSource lastDamageSource = DamageSource.none;

	private HashSet<PlayerKilledByType> KillNotificationBlacklist = new HashSet<PlayerKilledByType>
	{
		PlayerKilledByType.Environmental,
		PlayerKilledByType.Crushed,
		PlayerKilledByType.FallOffWorld,
		PlayerKilledByType.Impact
	};

	private readonly AvatarModifierPackageType[] canAffectGodzilla = new AvatarModifierPackageType[12]
	{
		AvatarModifierPackageType.Fire,
		AvatarModifierPackageType.FlamerBurn,
		AvatarModifierPackageType.Poison,
		AvatarModifierPackageType.GodzillaS,
		AvatarModifierPackageType.GodzillaM,
		AvatarModifierPackageType.GodzillaL,
		AvatarModifierPackageType.GodzillaXL,
		AvatarModifierPackageType.GodzillaLaserBurnS,
		AvatarModifierPackageType.GodzillaLaserBurnM,
		AvatarModifierPackageType.GodzillaLaserBurnL,
		AvatarModifierPackageType.GodzillaLaserBurnXL,
		AvatarModifierPackageType.GodzillaGrowthInvulnerability
	};

	private readonly MaterialHitPackage[] hitPackages = new MaterialHitPackage[1]
	{
		new MaterialHitPackage(AvatarModifierPackageType.Poison, PrefabPool.Instance.PoisonParticles)
	};

	private InteractableMaterialHitHandler materialHitHandler = new InteractableMaterialHitHandler();

	public DamageSource LastDamageSource => (!lastDamageSource.Outdated) ? lastDamageSource : null;

	public override void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariableClampedFloat health, MVRuntimeDataVariableClampedFloat shield)
	{
		base.Init(runtimeDataModifiers, health, shield);
		materialHitHandler.Initialize(hitPackages, transform);
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
			if (health.Value >= 100f)
			{
				float num = HandleModifierEffect(AvatarModifierEffect.OverHeal, 0f) * Time.deltaTime;
				shield.Value += num;
				if (OnShieldReplenished != null)
				{
					OnShieldReplenished();
				}
			}
		}
		if (!MVGameControllerBase.Game.IsPlaying || HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			return;
		}
		amount *= HandleModifierEffect(AvatarModifierEffect.DamageMultiplier, 1f);
		amount = DamageShield(amount);
		float value = health.Value;
		health.Value -= amount;
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
			PlayerKilledByType playerKilledByType = damageType;
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
		if (!MVGameControllerBase.Game.IsPlaying)
		{
			return;
		}
		BitArray bitArray = new BitArray(28);
		if (HasModifierEffect(AvatarModifierEffect.GodzillaImmunity))
		{
			bitArray.SetAll(value: true);
			for (int i = 0; i < canAffectGodzilla.Length; i++)
			{
				bitArray.Set((int)canAffectGodzilla[i], value: false);
			}
		}
		bitArray.Set(0, value: true);
		bitArray.Set(4, HasModifierEffect(AvatarModifierEffect.PoisonImmune));
		if (!bitArray[(int)type])
		{
			base.AddModifier(type, id, additionalModifers);
		}
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		if (moveHit.material.modifierPackageType != AvatarModifierPackageType.None)
		{
			AddModifier(moveHit.material.modifierPackageType);
		}
		materialHitHandler.HandleHit(moveHit);
	}
}
