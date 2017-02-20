using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;

public class AvatarInteractable : MVInteractable, IMoveHitHandler
{
	public Action<float, MVPlayer, PlayerKilledByType> OnDamageTaken;

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

	private MVRuntimeDataVariable invulnerable;

	public void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariable invulnerable, MVRuntimeDataVariableClampedFloat health)
	{
		Init(runtimeDataModifiers, health);
		this.invulnerable = invulnerable;
		materialHitHandler.Initialize(hitPackages, transform);
	}

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (IgnoreDamage(damageDealer) || !MVGameControllerBase.Game.IsPlaying || (bool)invulnerable.Value || HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			return;
		}
		amount *= HandleModifierEffect(AvatarModifierEffect.DamageMultiplier, 1f);
		float value = health.Value;
		health.Value -= amount;
		if (OnDamageTaken != null)
		{
			OnDamageTaken(amount, damageDealer, damageType);
		}
		if (health.Value <= 0f && value > 0f)
		{
			int num = damageDealer?.ActorNr ?? MVGameControllerBase.Game.LocalPlayerActorNumber;
			MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameControllerBase.Game.LocalPlayerActorNumber, num, damageType));
			if (!KillNotificationBlacklist.Contains(damageType))
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)7, MVGameControllerBase.Game.LocalPlayerActorNumber);
				dictionary.Add((byte)6, num);
				dictionary.Add((byte)8, damageType);
				Dictionary<object, object> dictionary2 = dictionary;
				NotificationController.OnNotificationReceived(NotificationType.Kill, dictionary2);
				MVGameControllerBase.OperationRequests.PostNotificationOperation(NotificationType.Kill, dictionary2);
			}
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (!MVGameControllerBase.Game.IsPlaying)
		{
			return;
		}
		BitArray bitArray = new BitArray(27);
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
