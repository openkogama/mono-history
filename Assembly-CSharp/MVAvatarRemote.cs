using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVAvatarRemote : MVAvatar
{
	private const float hitTimeOut = 2f;

	private float impulseMagnitudeFactor = 0.6f;

	private float velocityMinMagnitude = 1500f;

	private float velocityMaxMagnitude = 5000f;

	private float minVelocity = 700f;

	private float prevHitTime = Time.time - 2f;

	private float cullDistance = 145f;

	public MVAvatarRemote(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		SetNetworkObject(local: false);
	}

	public override void Initialize()
	{
		base.Initialize();
		avatar.NameTag = (string)Data["ownerUserName"];
		CreateTriggerCollider();
	}

	private void CreateTriggerCollider()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		GameObject val = new GameObject("triggerCollider");
		val.transform.parent = gameObject.transform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localRotation = Quaternion.identity;
		val.layer = LayerMask.NameToLayer("Player");
		CapsuleCollider val2 = val.AddComponent<CapsuleCollider>();
		((Collider)val2).isTrigger = true;
		CapsuleCollider val3 = (CapsuleCollider)gameObject.collider;
		val2.height = val3.height;
		val2.radius = val3.radius;
		TriggerBoxEvents triggerBoxEvents = val.AddComponent<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time - prevHitTime < 2f)
		{
			return;
		}
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(e.instigatorWOID);
		if (!(worldObjectClient is MVVehicleBase))
		{
			return;
		}
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (!((Object)(object)component != (Object)null))
		{
			return;
		}
		Vector3 val = component.Velocity;
		val /= Time.deltaTime;
		if (!(val.magnitude < minVelocity))
		{
			float num = Mathf.Clamp(val.magnitude, velocityMinMagnitude, velocityMaxMagnitude);
			num *= impulseMagnitudeFactor;
			val.y = 0f;
			val.Normalize();
			val.y = 1f;
			val.Normalize();
			val *= num;
			InteractionDataHandlerBase component2 = gameObject.GetComponent<InteractionDataHandlerBase>();
			if ((Object)(object)component2 != (Object)null)
			{
				Debug.Log((object)("Applying impulse " + val));
				component2.HandleInteraction(ImpulseHitPackage.Create(val), interactionIsLocal: false);
				prevHitTime = Time.time;
			}
		}
	}

	public override void ChangeLOD(float distance)
	{
		if (!Body.Visible && distance < cullDistance)
		{
			Body.Visible = true;
		}
		else if (Body.Visible && distance >= cullDistance)
		{
			Body.Visible = false;
		}
	}

	protected override void AttachBody(MVBody newBody)
	{
		base.AttachBody(newBody);
		newBody.Visible = true;
	}

	public override void OnLeaveVehicle()
	{
		HandleLeaveVehicle();
	}
}
