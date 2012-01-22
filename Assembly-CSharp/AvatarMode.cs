using UnityEngine;

public abstract class AvatarMode
{
	protected AvatarController avatarController;

	protected AvatarAnimation avatarAnimation;

	protected MovementMap movementMap = new MovementMap();

	public AvatarMode(AvatarController avatarController)
	{
		this.avatarController = avatarController;
		avatarAnimation = avatarController.Avatar.AvatarAnimation;
	}

	public virtual void Activate()
	{
		movementMap.Reset();
	}

	public virtual void FixedUpdate()
	{
	}

	public virtual void ProxyUpdate()
	{
	}

	public virtual void Deactivate()
	{
		movementMap.Reset();
	}

	public virtual void ApplyImpulse(Vector3 impulse, bool suspendImpactDamage = false)
	{
	}

	public virtual void HandleInput(NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
		movementMap.Update(actionCode, keyCode);
	}

	public virtual void Reset()
	{
	}
}
