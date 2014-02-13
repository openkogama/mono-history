using MV.Common;

public class AvatarInteractable : MVInteractable
{
	private MVRuntimeDataVariable invulnerable;

	public void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariable invulnerable, MVRuntimeDataVariableClampedFloat health)
	{
		Init(runtimeDataModifiers, health);
		this.invulnerable = invulnerable;
	}

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (!IgnoreDamage(damageDealer) && MVGameController.Instance.Game.IsPlaying && !(bool)invulnerable.Value && !HasModifier(AvatarModifierPackageType.Mutant))
		{
			float value = health.Value;
			health.Value -= amount;
			if (health.Value <= 0f && value > 0f)
			{
				int killerId = damageDealer?.ActorNr ?? MVGameController.Instance.Game.LocalPlayerActorNumber;
				MVGameController.Instance.Game.PostGameMsg(MVGameMsgType.AvatarKilled, GameMessages.MakePlayerKilledMessage(MVGameController.Instance.Game.LocalPlayerActorNumber, killerId, damageType));
			}
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		switch (type)
		{
		case AvatarModifierPackageType.None:
			return;
		case AvatarModifierPackageType.Poison:
			if (HasModifier(AvatarModifierPackageType.Mutant))
			{
				return;
			}
			break;
		}
		base.AddModifier(type, id, additionalModifers);
	}
}
