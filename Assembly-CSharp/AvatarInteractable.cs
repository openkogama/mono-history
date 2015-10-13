using MV.Common;

public class AvatarInteractable : MVInteractable, IMoveHitHandler
{
	private MVRuntimeDataVariable invulnerable;

	public void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariable invulnerable, MVRuntimeDataVariableClampedFloat health)
	{
		Init(runtimeDataModifiers, health);
		this.invulnerable = invulnerable;
	}

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (!IgnoreDamage(damageDealer) && MVGameController.Game.IsPlaying && !(bool)invulnerable.Value && !HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			amount *= HandleModifierEffect(AvatarModifierEffect.DamageMultiplier, 1f);
			float value = health.Value;
			health.Value -= amount;
			if (health.Value <= 0f && value > 0f)
			{
				int killerId = damageDealer?.ActorNr ?? MVGameController.Game.LocalPlayerActorNumber;
				MVGameController.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Game.LocalPlayerActorNumber, killerId, damageType));
			}
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (type != AvatarModifierPackageType.None && MVGameController.Game.IsPlaying && (type != AvatarModifierPackageType.Poison || !HasModifierEffect(AvatarModifierEffect.PoisonImmune)))
		{
			base.AddModifier(type, id, additionalModifers);
		}
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		AddModifier(moveHit.material.modifierPackageType);
	}
}
