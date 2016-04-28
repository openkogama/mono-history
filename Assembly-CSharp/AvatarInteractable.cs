using System.Collections.Generic;
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
		if (!IgnoreDamage(damageDealer) && MVGameControllerBase.Game.IsPlaying && !(bool)invulnerable.Value && !HasModifierEffect(AvatarModifierEffect.Invulnerable))
		{
			amount *= HandleModifierEffect(AvatarModifierEffect.DamageMultiplier, 1f);
			float value = health.Value;
			health.Value -= amount;
			if (health.Value <= 0f && value > 0f)
			{
				int num = damageDealer?.ActorNr ?? MVGameControllerBase.Game.LocalPlayerActorNumber;
				MVGameControllerBase.OperationRequests.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameControllerBase.Game.LocalPlayerActorNumber, num, damageType));
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)7, MVGameControllerBase.Game.LocalPlayerActorNumber);
				dictionary.Add((byte)6, num);
				dictionary.Add((byte)8, damageType);
				Dictionary<object, object> notificationData = dictionary;
				MVGameControllerBase.OperationRequests.PostNotificationOperation(NotificationType.Kill, notificationData);
			}
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		if (type != AvatarModifierPackageType.None && MVGameControllerBase.Game.IsPlaying && (type != AvatarModifierPackageType.Poison || !HasModifierEffect(AvatarModifierEffect.PoisonImmune)))
		{
			base.AddModifier(type, id, additionalModifers);
		}
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		AddModifier(moveHit.material.modifierPackageType);
	}
}
