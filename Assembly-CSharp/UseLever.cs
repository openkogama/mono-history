using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class UseLever : MVLogicObject
{
	private bool isActivated;

	private UseLeverObject useLeverObject;

	private float minY = -0.25f;

	private float speed = 1.8f;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 WorldPivot => transform.position;

	public override Vector3 OutputConnectorOffset => new Vector3(2.2f, 0f, 0f);

	public UseLever(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.UseLeverPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		useLeverObject = (UseLeverObject)component;
		useLeverObject.UseInteractor = new UseInteractor(Id, gameObject, reset: false, useLeverObject.LeverCollider, Use);
		useLeverObject.TriggerBoxEvents.TriggerEnter += useLeverObject.UseInteractor.triggerBoxEvents_TriggerEnter;
		useLeverObject.TriggerBoxEvents.TriggerExit += useLeverObject.UseInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, new Vector3(0.5f, 1f, 0f));
		useLeverObject.UseInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject);
		useLeverObject.UseInteractor.AddRequirement(useRequirement2);
		StarRequirement useRequirement3 = new StarRequirement(gameObject);
		useLeverObject.UseInteractor.AddRequirement(useRequirement3);
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override void Initialize()
	{
		base.Initialize();
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Combine(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			OnEditModeChange(new EditModeChangeArgs(state: false));
		}
		isActivated = (bool)Data["beginActivated"];
		if (RunTimeData.ContainsObscuredKey("activated"))
		{
			isActivated = (ObscuredBool)RunTimeData.GetObscuredType("activated");
		}
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
		}
		useLeverObject.UseInteractor.UpdateData(Data);
		SetLinks(isActivated);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (isActivated && useLeverObject.PlateButtonTransform.localPosition.z > minY)
		{
			float num = Mathf.Min(speed * Time.smoothDeltaTime, useLeverObject.PlateButtonTransform.localPosition.z - minY);
			useLeverObject.PlateButtonTransform.localPosition = new Vector3(useLeverObject.PlateButtonTransform.localPosition.x, useLeverObject.PlateButtonTransform.localPosition.y, useLeverObject.PlateButtonTransform.localPosition.z - num);
		}
		else if (!isActivated && useLeverObject.PlateButtonTransform.localPosition.z < 0f)
		{
			float num2 = Mathf.Min(speed * Time.smoothDeltaTime, 0f - useLeverObject.PlateButtonTransform.localPosition.z);
			useLeverObject.PlateButtonTransform.localPosition = new Vector3(useLeverObject.PlateButtonTransform.localPosition.x, useLeverObject.PlateButtonTransform.localPosition.y, useLeverObject.PlateButtonTransform.localPosition.z + num2);
		}
	}

	public bool Use(int userWoID)
	{
		isActivated = !isActivated;
		if (isActivated)
		{
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, userWoID);
		}
		else
		{
			MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, userWoID);
		}
		return true;
	}

	public void SetLinks(bool linkFlag)
	{
		isActivated = linkFlag;
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = linkFlag;
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		useLeverObject.EditCollider.gameObject.SetActive(value: false);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, new Vector3(1.2f, 1.2f, 0.2f));
	}

	public override void Reset()
	{
		OnDataUpdate();
	}

	public override void OnDataUpdate()
	{
		isActivated = (bool)Data["beginActivated"];
		useLeverObject.UseInteractor.UpdateData(Data);
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		if (avatarLocal != null)
		{
			if (isActivated)
			{
				MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, avatarLocal.Id);
			}
			else
			{
				MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, avatarLocal.Id);
			}
		}
	}

	public override void Destroy()
	{
		if (MVGameControllerBase.IEditModeUI != null)
		{
			IEditModeUI iEditModeUI = MVGameControllerBase.IEditModeUI;
			iEditModeUI.EditModeChange = (Action<EditModeChangeArgs>)Delegate.Remove(iEditModeUI.EditModeChange, new Action<EditModeChangeArgs>(OnEditModeChange));
		}
		useLeverObject.TriggerBoxEvents.TriggerEnter -= useLeverObject.UseInteractor.triggerBoxEvents_TriggerEnter;
		useLeverObject.TriggerBoxEvents.TriggerExit -= useLeverObject.UseInteractor.triggerBoxEvents_TriggerExit;
		useLeverObject.UseInteractor.OnDestroy(Data);
		base.Destroy();
	}

	public override void OnOutputLinkChanged()
	{
		base.OnOutputLinkChanged();
		Reset();
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		useLeverObject.EditCollider.enabled = true;
		useLeverObject.LeverCollider.enabled = false;
		if (arg.playInEditor)
		{
			useLeverObject.EditCollider.enabled = false;
			useLeverObject.LeverCollider.enabled = true;
		}
	}

	private void SetVisibility()
	{
		MeshRenderer[] meshRenderers = useLeverObject.MeshRenderers;
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			meshRenderers[i].enabled = !disabledByLod;
		}
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
}
