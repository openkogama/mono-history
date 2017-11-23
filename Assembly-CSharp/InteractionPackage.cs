using MV.Common;
using MV.WorldObject;
using UnityEngine;

public abstract class InteractionPackage
{
	public abstract void ParseAndHandlePackage(MVWorldObjectClient worldObjectClient, MVPlayer shooter, InteractionData interactionStruct);

	protected void HandlePackage(MVWorldObjectClient target, MVPlayer shooter, float damage, PlayerKilledByType killedByType, Vector3 impulse, AvatarModifierPackageType modType)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (IsValidTarget(shooter, target, component))
		{
			if (component != null)
			{
				component.TakeDamage(damage, shooter, killedByType);
				component.AddModifier(modType, shooter.ActorNr);
			}
			MVRigidBody component2 = target.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(shooter, impulse, suspendImpactDamage: true);
			}
		}
	}

	protected void HandlePackage(MVWorldObjectClient target, MVPlayer shooter, float damage, PlayerKilledByType killedByType, Vector3 impulse)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (IsValidTarget(shooter, target, component))
		{
			if (component != null)
			{
				component.TakeDamage(damage, shooter, killedByType);
			}
			MVRigidBody component2 = target.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(shooter, impulse, suspendImpactDamage: true);
			}
		}
	}

	protected void HandlePackage(MVWorldObjectClient target, MVPlayer shooter, float damage, PlayerKilledByType killedByType)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (IsValidTarget(shooter, target, component) && component != null)
		{
			component.TakeDamage(damage, shooter, killedByType);
		}
	}

	protected void HandlePackage(MVWorldObjectClient target, MVPlayer shooter, Vector3 impulse, AvatarModifierPackageType modifierType)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (IsValidTarget(shooter, target, component))
		{
			if (component != null)
			{
				component.AddModifier(modifierType, shooter.ActorNr);
			}
			MVRigidBody component2 = target.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(shooter, impulse, suspendImpactDamage: true);
			}
		}
	}

	protected void HandlePackage(MVWorldObjectClient target, Vector3 impulse, AvatarModifierPackageType modifierType)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (!IsSpawnProtected(component))
		{
			if (component != null)
			{
				component.AddModifier(modifierType);
			}
			MVRigidBody component2 = target.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(null, impulse, suspendImpactDamage: true);
			}
		}
	}

	protected void HandlePackage(MVWorldObjectClient target, MVPlayer shooter, Vector3 impulse)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (IsValidTarget(shooter, target, component))
		{
			MVRigidBody component2 = target.GameObject.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(shooter, impulse, suspendImpactDamage: true);
			}
		}
	}

	protected void HandlePackage(MVWorldObjectClient target, MVPlayer shooter, AvatarModifierPackageType modifierType)
	{
		MVInteractableBase component = target.GameObject.GetComponent<MVInteractableBase>();
		if (IsValidTarget(shooter, target, component) && component != null)
		{
			component.AddModifier(modifierType, shooter.ActorNr);
		}
	}

	private bool IsValidTarget(MVPlayer shooter, MVWorldObjectClient target, MVInteractableBase targetInteractable)
	{
		return !IsSpawnProtected(targetInteractable);
	}

	private bool IsSpawnProtected(MVInteractableBase targetInteractable)
	{
		return targetInteractable != null && targetInteractable.HasModifier(AvatarModifierPackageType.SpawnProtection);
	}
}
