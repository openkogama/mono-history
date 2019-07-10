using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class WindTurbine : MVLogicObject, ILogicWorldObject, IUpdatecontrollerSubscriberFixedUpdate, IUpdatecontrollerSubscriberBase
{
	private Dictionary<int, MVRigidBody> affectedBodies;

	private bool isActive;

	private float windStrength = 1000f;

	private float windAreaSize = 10f;

	private float windPitch;

	private const float maxWindStrength = 280f;

	private const float maxWindAreaSize = 20f;

	private WindTurbineObject windTurbineObject;

	private List<int> keysToRemove = new List<int>();

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.WindTurbine;

	public override Vector3 WorldPivot => transform.position;

	public override Vector3 InputConnectorOffset => new Vector3(-1.55f, 0f, 0f);

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public WindTurbine(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.WindTurbinePrefab, worldObjects)
	{
		windTurbineObject = (WindTurbineObject)component;
		windAreaSize = (float)Data["windSize"];
		windPitch = (float)Data["windPitch"];
		windStrength = windAreaSize / 20f * 280f;
		Rescale();
		Rotate();
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanResetLogic;
		windTurbineObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		windTurbineObject.TriggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		affectedBodies = new Dictionary<int, MVRigidBody>();
		isActive = true;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		SetData();
		SetupCulling(windTurbineObject.VisualObject).Radius = 4f;
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, InputStateUpdateCallback);
		ToggleTurbine(InputSignalReceiver.CurrentlyIsHot);
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			ToggleTurbine(state: true);
		}
		if (logicInputState == LogicInputState.FromHotToCold)
		{
			ToggleTurbine(state: false);
		}
	}

	public override void OnDataUpdate()
	{
		SetData();
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void SetData()
	{
		windAreaSize = (float)Data["windSize"];
		windPitch = (float)Data["windPitch"];
		windStrength = windAreaSize / 20f * 280f;
		Rescale();
		Rotate();
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(new Vector3(0f, 0f, 0f), new Vector3(2f, 2f, 1.5f));
	}

	public override void UpdateControllerFixedUpdate()
	{
		if (!isActive)
		{
			return;
		}
		foreach (KeyValuePair<int, MVRigidBody> affectedBody in affectedBodies)
		{
			if (affectedBody.Value == null)
			{
				keysToRemove.Add(affectedBody.Key);
			}
			else if (!affectedBody.Value.IsMovementLocked)
			{
				Vector3 vector = gameObject.transform.InverseTransformPoint(affectedBody.Value.GetComponent<Collider>().bounds.center);
				float num = Mathf.Max(1f - vector.z * vector.z / (windAreaSize * 20f), 0f);
				float num2 = windStrength * num;
				affectedBody.Value.AddImpulse(gameObject.transform.forward * num2, suspendImpactDamage: true);
			}
		}
		for (int i = 0; i < keysToRemove.Count; i++)
		{
			affectedBodies.Remove(keysToRemove[i]);
		}
		keysToRemove.Clear();
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
		MVRigidBody mVRigidBody = worldObjectClient.GameObject.GetComponent<MVRigidBody>();
		if (!(mVRigidBody != null))
		{
			return;
		}
		MVInteractableBase mVInteractableBase = worldObjectClient.GameObject.GetComponent<MVInteractableBase>();
		if (!(mVInteractableBase == null))
		{
			if (InputLinkRefs.Count == 0 || InputState)
			{
				mVInteractableBase.AddModifier(AvatarModifierPackageType.WindFriction);
			}
			affectedBodies[instigatorWOID] = mVRigidBody;
		}
	}

	private void ExitWindZone(int instigatorWOID)
	{
		if (affectedBodies.ContainsKey(instigatorWOID))
		{
			MVInteractableBase mVInteractableBase = affectedBodies[instigatorWOID].GetComponent<MVInteractableBase>();
			if (!(mVInteractableBase == null))
			{
				mVInteractableBase.RemoveModifier(AvatarModifierPackageType.WindFriction);
				affectedBodies.Remove(instigatorWOID);
			}
		}
	}

	private void Rescale()
	{
		Vector3 localScale = windTurbineObject.AreaColliderTransform.localScale;
		localScale.z = windAreaSize;
		windTurbineObject.AreaColliderTransform.localScale = localScale;
		localScale = windTurbineObject.AreaColliderTransform.localPosition;
		localScale.z = windAreaSize / 2f + 0.5f;
		windTurbineObject.AreaColliderTransform.localPosition = localScale;
		ParticleSystem.MainModule main = windTurbineObject.WindParticleSystem.main;
		main.startLifetimeMultiplier = windAreaSize / 20f;
		main.startSizeMultiplier = 0.1f;
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
		ParticleSystem.EmissionModule emission = windTurbineObject.WindParticleSystem.emission;
		emission.enabled = state;
	}

	public override void InitializeInventory()
	{
		base.Initialize();
		windTurbineObject.AreaColliderTransform.gameObject.SetActive(value: false);
		windTurbineObject.WindParticleSystem.gameObject.SetActive(value: false);
		inputConnectorObject.SetActive(value: false);
	}

	public override void Destroy()
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			IEditModeUI editModeUI = MVGameControllerBase.EditModeUI;
			editModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(editModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		UpdateController.RemoveFixedUpdateObject(this);
		base.Destroy();
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		windTurbineObject.EditorCollider.enabled = true;
		windTurbineObject.AreaCollider.enabled = false;
		if (arg.playInEditor)
		{
			windTurbineObject.EditorCollider.enabled = false;
			windTurbineObject.AreaCollider.enabled = true;
		}
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}
}
