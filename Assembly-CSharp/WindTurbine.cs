using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class WindTurbine : MVLogicObject
{
	private const float maxWindStrength = 280f;

	private const float maxWindAreaSize = 20f;

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
		: base(data, PrefabPool.Instance.WindTurbinePrefab, worldObjects)
	{
		windAreaSize = (float)Data["windSize"];
		windPitch = (float)Data["windPitch"];
		windStrength = windAreaSize / 20f * 280f;
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

	public override void Initialize()
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		OnDataUpdate();
	}

	public override void OnDataUpdate()
	{
		windAreaSize = (float)Data["windSize"];
		windPitch = (float)Data["windPitch"];
		windStrength = windAreaSize / 20f * 280f;
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
		return new Bounds(new Vector3(0f, 0f, 0f), new Vector3(2f, 2f, 1.5f));
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
				float num = Mathf.Max(1f - vector.z * vector.z / (windAreaSize * 20f), 0f);
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
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigatorWOID);
		MVRigidBody component = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (component != null)
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
		windParticleSystem.startLifetime = windAreaSize / 20f;
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
		ParticleSystem.EmissionModule emission = windParticleSystem.emission;
		emission.enabled = state;
	}

	public override void InitializeInventory()
	{
		base.Initialize();
		colliderObject.SetActive(value: false);
		windParticleSystem.gameObject.SetActive(value: false);
		inputConnectorObject.SetActive(value: false);
	}

	public override void Destroy()
	{
		IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
		iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		base.Destroy();
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		Collider component = gameObject.transform.FindChild("EditCollider").GetComponent<Collider>();
		Collider component2 = colliderObject.GetComponent<Collider>();
		component.enabled = true;
		component2.enabled = false;
		if (arg.playInEditor)
		{
			component.enabled = false;
			component2.enabled = true;
		}
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}
}
