using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class GodzillaTrigger : MVLogicObject
{
	public const int noOccupant = -1;

	private ShockWaveEmitter shockWaveEmitter;

	private MVRuntimeDataVariable<int> occupantWOID;

	private GameObject logicCube;

	private GameObject godzillaArea;

	private Vector3 originalScale;

	private AvatarModifierPackageType avatarModifierPackageType;

	private List<GameObject> toHideOnEntry;

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

	public override void Destroy()
	{
		ResetGodzilla();
		if (IsLocal(OccupantWOID))
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
		base.Destroy();
	}

	private bool CanUse(MVInteractableBase interactable)
	{
		return OccupantWOID == -1 && !interactable.HasModifierEffect(AvatarModifierEffect.DisableVehicles);
	}

	public override void Initialize()
	{
		base.Initialize();
		originalScale = Scale;
		if (!Data.ContainsKey("size"))
		{
			Data["size"] = 1;
		}
		if (OccupantWOID != -1)
		{
			CreateGodzillaArea();
			SetOccupied(occupied: true);
		}
		OnDataUpdate();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		logicCube.SetActive(value: false);
	}

	private bool HasGodzillaModifier(MVInteractableBase interactable)
	{
		return interactable.HasModifier(AvatarModifierPackageType.GodzillaS) || interactable.HasModifier(AvatarModifierPackageType.GodzillaM) || interactable.HasModifier(AvatarModifierPackageType.GodzillaL) || interactable.HasModifier(AvatarModifierPackageType.GodzillaXL);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		GodzillaSettings.Sizes s = (GodzillaSettings.Sizes)(int)Data["size"];
		avatarModifierPackageType = GodzillaSettings.GetPackageType(s);
		if (OccupantWOID != -1)
		{
			float sizeModifier = GodzillaModifier.constants[(GodzillaModifier.GodzillaModifierPackageType)avatarModifierPackageType].sizeModifier;
			godzillaArea.transform.localScale = new Vector3(sizeModifier, sizeModifier, sizeModifier);
			if (MVGameControllerBase.WOCM.AvatarLocal != null && IsLocal(OccupantWOID))
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
			}
		}
	}

	private void SetOutput(bool output)
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = output;
		}
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

	public void OccupationChange(object newValue)
	{
		int num = (int)newValue;
		SetOutput(num != -1);
		ResetGodzilla();
		SetGodzilla(num);
	}

	private void ResetGodzilla()
	{
		if (OccupantWOID != -1)
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
	}

	private void SetGodzilla(int woid)
	{
		OccupantWOID = woid;
		if (OccupantWOID != -1)
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
