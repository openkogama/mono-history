using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerBoxEvents))]
public class ForceField : MonoBehaviour
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private CapsuleCollider trigger;

	[SerializeField]
	private float strength;

	private HashSet<MVInteractableBase> interactablesInField = new HashSet<MVInteractableBase>();

	private HashSet<MVRigidBody> bodiesInField = new HashSet<MVRigidBody>();

	private void Awake()
	{
		triggerBoxEvents.TriggerEnter += OnEnter;
		triggerBoxEvents.TriggerExit += OnExit;
	}

	private void OnExit(object sender, TriggerEventArgs e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
		if (worldObjectClient != null)
		{
			MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if (component != null)
			{
				bodiesInField.Remove(component);
			}
			MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
			if (component2 != null)
			{
				interactablesInField.Remove(component2);
			}
		}
	}

	private void OnEnter(object sender, TriggerEventArgs e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.instigatorWOID);
		if (worldObjectClient != null)
		{
			MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
			if (component != null)
			{
				bodiesInField.Add(component);
			}
			MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
			if (component2 != null)
			{
				interactablesInField.Add(component2);
			}
		}
	}

	private void ApplyForceTo(MVRigidBody body)
	{
		Vector3 vector = transform.position + trigger.center.Multiply(transform.lossyScale);
		float num = (trigger.height / 2f - trigger.radius) * transform.lossyScale.y;
		float y = vector.y;
		y += num;
		float y2 = vector.y;
		y2 -= num;
		float y3 = Mathf.Clamp(body.transform.position.y, y2, y);
		Vector3 position = transform.position;
		position.y = y3;
		Vector3 rhs = body.transform.position - position;
		if (Vector3.Dot(body.Velocity, rhs) <= 0f)
		{
			float num2 = 1f / Time.fixedDeltaTime;
			body.AddImpulse(rhs.normalized * body.Velocity.magnitude * num2 * 2f);
		}
	}

	private void ApplyNoFriction(MVInteractableBase interactable)
	{
		interactable.AddModifier(AvatarModifierPackageType.NoFriction);
	}

	private void FixedUpdate()
	{
		foreach (MVInteractableBase item in interactablesInField)
		{
			ApplyNoFriction(item);
		}
		foreach (MVRigidBody item2 in bodiesInField)
		{
			if (item2 == null)
			{
				bodiesInField.Remove(item2);
			}
			else
			{
				ApplyForceTo(item2);
			}
		}
	}
}
