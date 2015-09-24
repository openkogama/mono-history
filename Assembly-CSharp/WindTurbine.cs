using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MVLogicObject
{
	private const float maxWindStrength = 520f;

	private const float maxWindAreaSize = 10f;

	private const string prefabPath = "Prefabs/WindTurbineObject";

	private TriggerBoxEvents triggerBoxEvents;

	private Dictionary<int, MVRigidBody> affectedBodies;

	private bool isActive;

	private float windStrength = 1000f;

	private float windAreaSize = 10f;

	private float windPitch;

	private GameObject colliderObject;

	private ParticleSystem windParticleSystem;

	public override Vector3 WorldPivot => transform.position;

	public override Vector3 InputConnectorOffset => new Vector3(-1.55f, 0f, 0f);

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public WindTurbine(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/WindTurbineObject", worldObjects)
	{
		windAreaSize = (float)Data["windSize"];
		windPitch = (float)Data["windPitch"];
		windStrength = windAreaSize / 10f * 520f;
		colliderObject = gameObject.transform.FindChild("Cube").gameObject;
		windParticleSystem = gameObject.transform.FindChild("PushPad_prefab").FindChild("Wind").GetComponent<ParticleSystem>();
		Rescale();
		Rotate();
		interactionFlags |= InteractionFlags.HasSettings;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		affectedBodies = new Dictionary<int, MVRigidBody>();
		isActive = true;
	}

	public override void OnDataUpdate()
	{
		windAreaSize = (float)Data["windSize"];
		windPitch = (float)Data["windPitch"];
		windStrength = windAreaSize / 10f * 520f;
		Rescale();
		Rotate();
	}

	public override void OnInputLinkChanged()
	{
		ToggleTurbine(state: true);
		if (InputLinkRefs.Count != 0)
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputStateChanged()
	{
		ToggleTurbine(InputState);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		if (boundsContext == BoundsContext.Preview)
		{
			return new Bounds(new Vector3(0f, 0f, 0f), new Vector3(2f, 2f, 1f));
		}
		return new Bounds(new Vector3(0f, 0f, windAreaSize / 2f), new Vector3(2f, 2f, windAreaSize + 1f));
	}

	protected override void OnUpdate()
	{
	}

	public override void FixedUpdate()
	{
		if (!isActive)
		{
			return;
		}
		foreach (KeyValuePair<int, MVRigidBody> affectedBody in affectedBodies)
		{
			if (affectedBody.Value == null)
			{
				affectedBodies.Remove(affectedBody.Key);
			}
			else if (!affectedBody.Value.IsMovementLocked)
			{
				Vector3 vector = gameObject.transform.InverseTransformPoint(affectedBody.Value.GetComponent<Collider>().bounds.center);
				float num = Mathf.Max(1f - vector.z * vector.z / (windAreaSize * 8f), 0f);
				float num2 = Mathf.Max(1f - (vector.x * vector.x + vector.y * vector.y) / 3f, 0f);
				float num3 = windStrength * (num * num2);
				affectedBody.Value.AddImpulse(gameObject.transform.forward * num3, suspendImpactDamage: true);
			}
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		EnterWindZone(e.instigatorWOID);
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		ExitWindZone(e.instigatorWOID);
	}

	private void EnterWindZone(int instigatorWOID)
	{
		if (affectedBodies.ContainsKey(instigatorWOID))
		{
			return;
		}
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(instigatorWOID);
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (component != null && isActive)
		{
			MVInteractableBase component2 = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
			if (!(component2 == null))
			{
				component2.AddModifier(AvatarModifierPackageType.WindFriction);
				affectedBodies[instigatorWOID] = component;
			}
		}
	}

	private void ExitWindZone(int instigatorWOID)
	{
		if (affectedBodies.ContainsKey(instigatorWOID))
		{
			MVInteractableBase component = affectedBodies[instigatorWOID].GetComponent<MVInteractableBase>();
			if (!(component == null))
			{
				component.RemoveModifier(AvatarModifierPackageType.WindFriction);
				affectedBodies.Remove(instigatorWOID);
			}
		}
	}

	private void Rescale()
	{
		Vector3 localScale = colliderObject.transform.localScale;
		localScale.z = windAreaSize;
		colliderObject.transform.localScale = localScale;
		localScale = colliderObject.transform.localPosition;
		localScale.z = windAreaSize / 2f + 0.5f;
		colliderObject.transform.localPosition = localScale;
		windParticleSystem.startLifetime = windAreaSize / 10f;
		windParticleSystem.startSize = 0.1f;
	}

	private void Rotate()
	{
		Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
		localEulerAngles.x = windPitch - 90f;
		gameObject.transform.localEulerAngles = localEulerAngles;
	}

	private void ToggleTurbine(bool state)
	{
		isActive = state;
		windParticleSystem.enableEmission = state;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnDataUpdate();
	}

	public override void InitializeInventory()
	{
		base.Initialize();
		colliderObject.SetActive(value: false);
	}
}
