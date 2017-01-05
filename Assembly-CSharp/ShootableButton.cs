using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ShootableButton : MVLogicObject, ITriggerBoxEventsHandler
{
	private LogicInteractable interactable;

	private Collider targetCollider;

	private ShootableButtonObject buttonObject;

	public override Vector3 WorldPivot => transform.position;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => new Vector3(1.6f, 0f, 0f);

	public ShootableButton(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.ShootableButtonPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		PlayInteractionType = PlayInteractionType.HandlesHits;
		buttonObject = (ShootableButtonObject)component;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 1.5f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override void Initialize()
	{
		base.Initialize();
		interactable = gameObject.AddComponent<LogicInteractable>();
		gameObject.AddComponent<InteractionDataHandler>();
		interactable.OnDamageEvent += Activate;
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			buttonObject.TargetCollider3D.enabled = false;
			targetCollider = buttonObject.TargetCollider2D;
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 90f, transform.localEulerAngles.z);
		}
		else
		{
			buttonObject.TargetCollider2D.enabled = false;
			targetCollider = buttonObject.TargetCollider3D;
		}
		collider = targetCollider;
		if (RunTimeData.ContainsObscuredKey("isActivated") && (bool)(ObscuredBool)RunTimeData.GetObscuredType("isActivated"))
		{
			Enter(-1);
		}
		SetupCulling(buttonObject.VisualRoot);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		buttonObject.EditCollider.gameObject.SetActive(value: false);
	}

	public override void Reset()
	{
		MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, MVGameControllerBase.WOCM.AvatarLocal.Id);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, new Vector3(2.001f, 2.001f, 0.701f));
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
	}

	public void Activate(object sender, TakeDamageEventArgs e)
	{
		int triggerInstigatorId = 0;
		if (e.damageSource != null)
		{
			triggerInstigatorId = e.damageSource.Avatar.Id;
		}
		MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, triggerInstigatorId);
	}

	public void Enter(int instigatorWoID)
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = true;
		}
		targetCollider.enabled = false;
		buttonObject.GreyOutObject.GreyOut();
	}

	public void Exit()
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = false;
		}
		targetCollider.enabled = true;
		buttonObject.GreyOutObject.GreyIn();
	}

	public override void Destroy()
	{
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		base.Destroy();
	}

	private void SetVisibility()
	{
		Renderer componentInChildren = GameObject.GetComponentInChildren<Renderer>();
		componentInChildren.enabled = !disabledByLod;
	}

	public override void ChangeLOD(float distance)
	{
		bool flag = disabledByLod;
		base.ChangeLOD(distance);
		if (flag != disabledByLod)
		{
			SetVisibility();
		}
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		buttonObject.EditCollider.enabled = true;
		targetCollider.enabled = false;
		if (arg.playInEditor)
		{
			buttonObject.EditCollider.enabled = false;
			targetCollider.enabled = true;
		}
	}
}
