using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GodzillaTrigger : MVLogicObject, ILogicWorldObject
{
	public const int noOccupant = -1;

	private ShockWaveEmitter shockWaveEmitter;

	private MVRuntimeDataVariable<int> occupantWOID;

	private GameObject logicCube;

	private GameObject godzillaArea;

	private Vector3 originalScale;

	private AvatarModifierPackageType avatarModifierPackageType;

	private List<GameObject> toHideOnEntry;

	private OutputSignalTransmitter outputSignalTransmitter;

	public override bool HasOutputConnector => true;

	private int OccupantWOID
	{
		get
		{
			return occupantWOID.Value;
		}
		set
		{
			occupantWOID.Value = value;
		}
	}

	private GodzillaSettings.Sizes Size => (GodzillaSettings.Sizes)(int)Data["size"];

	private bool IsOccupied => OccupantWOID != -1;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public GodzillaTrigger(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.GodzillaTriggerPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		occupantWOID = RuntimeDataVariables.New<int>("occupantWOID", float.PositiveInfinity, writeThrough: false);
		GodzillaTriggerObject godzillaTriggerObject = (GodzillaTriggerObject)component;
		logicCube = godzillaTriggerObject.LogicCube;
		shockWaveEmitter = godzillaTriggerObject.ShockWaveEmitter;
		toHideOnEntry = godzillaTriggerObject.ToHideOnEntry;
		UseInteractor useInteractor = new UseInteractor(this, transform.gameObject, reset: false, godzillaTriggerObject.TriggerBoxEvents.Collider, Enter, CanUse);
		godzillaTriggerObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		godzillaTriggerObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
	}

	public override void Initialize()
	{
		base.Initialize();
		originalScale = Scale;
		if (IsOccupied)
		{
			CreateGodzillaArea();
			SetOccupied(occupied: true);
		}
		avatarModifierPackageType = GodzillaSettings.GetPackageType(Size);
		SetupScale();
		InputSignalReceiver = LogicClientsideFactory.CreateInputSignalReceiver(this, defaultInput: false, SignalCallback);
		outputSignalTransmitter = new OutputSignalTransmitter(Id);
	}

	private void SignalCallback(bool b, bool wasHot, LogicObjectManager logicObjectManager)
	{
		outputSignalTransmitter.Send(IsOccupied);
	}

	public override void Destroy()
	{
		if (IsOccupied)
		{
			ResetGodzilla();
		}
		if (IsLocal(OccupantWOID))
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
		base.Destroy();
	}

	private bool CanUse(MVInteractableBase interactable)
	{
		return !IsOccupied && !interactable.HasModifierEffect(AvatarModifierEffect.DisableVehicles);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		logicCube.SetActive(value: false);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		avatarModifierPackageType = GodzillaSettings.GetPackageType(Size);
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void SetupScale()
	{
		if (IsOccupied)
		{
			float sizeModifier = GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)avatarModifierPackageType].sizeModifier;
			godzillaArea.transform.localScale = new Vector3(sizeModifier, sizeModifier, sizeModifier);
		}
	}

	public override void Reset()
	{
		if (IsLocal(OccupantWOID))
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
		OccupationChange(-1);
	}

	private bool IsLocal(int woid)
	{
		return woid == MVGameControllerBase.WOCM.AvatarLocal.Id;
	}

	private MVInteractableBase GetInteractableBase(int woid)
	{
		return MVGameControllerBase.WOCM.GetWorldObjectClient(woid)?.GameObject.GetComponent<MVInteractableBase>();
	}

	private bool Enter(int instigatorWOID)
	{
		MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, instigatorWOID);
		return true;
	}

	public void Exit(int woid)
	{
		MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, woid);
	}

	public void OccupationChange(int newOccupant)
	{
		if (IsOccupied)
		{
			ResetGodzilla();
		}
		OccupantWOID = newOccupant;
		if (IsOccupied)
		{
			SetGodzilla();
		}
	}

	private void ResetGodzilla()
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(OccupantWOID);
		godzillaArea.transform.localScale = originalScale;
		if (worldObjectClient != null)
		{
			worldObjectClient.ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Remove(worldObjectClient.ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(UpdateScale));
		}
		DestroyGodzillaArea();
		SetOccupied(occupied: false);
	}

	private void SetGodzilla()
	{
		if (IsLocal(OccupantWOID))
		{
			SetGodzillaLocal();
		}
		CreateGodzillaArea();
		SetOccupied(occupied: true);
		EmitShockWave();
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(OccupantWOID);
		worldObjectClient.ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(worldObjectClient.ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(UpdateScale));
	}

	private void SetGodzillaLocal()
	{
		MVInteractableBase interactableBase = GetInteractableBase(OccupantWOID);
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.SetMode(AvatarRuntimeState.Godzilla);
		MVAvatarLocal.GodzillaMode godzillaMode = (MVAvatarLocal.GodzillaMode)avatarLocal.CurrentMode;
		godzillaMode.SetGodzillaTriggerRef(this);
		godzillaMode.ActivateModifier(avatarModifierPackageType);
	}

	private void CreateGodzillaArea()
	{
		godzillaArea = UnityEngine.Object.Instantiate(PrefabPool.Instance.GodzillaAreaPrefab);
		godzillaArea.transform.SetParent(transform);
		godzillaArea.transform.position = Position;
		godzillaArea.transform.rotation = Rotation;
	}

	private void SetOccupied(bool occupied)
	{
		for (int i = 0; i < toHideOnEntry.Count; i++)
		{
			toHideOnEntry[i].SetActive(!occupied);
		}
	}

	private void DestroyGodzillaArea()
	{
		UnityEngine.Object.Destroy(godzillaArea);
	}

	private void EmitShockWave()
	{
		shockWaveEmitter.TriggerFrom(Position, GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)avatarModifierPackageType].sizeModifier);
	}

	private void UpdateScale(MVWorldObjectClient notUsed = null, ScaleChangedEventArgs notUsed_ = null)
	{
		godzillaArea.transform.localScale = MVGameControllerBase.WOCM.GetWorldObjectClient(OccupantWOID).Scale;
	}
}
