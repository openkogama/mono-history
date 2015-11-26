using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class UseLever : MVLogicObject
{
	private const string prefabPath = "Prefabs/UseLeverObject";

	private bool isActivated;

	private Collider leverCollider;

	private Collider pushCollider;

	private GameObject plateButton;

	private UseInteractor useInteractor;

	private TriggerBoxEvents triggerBoxEvents;

	private float minY = -0.25f;

	private float speed = 1.8f;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 WorldPivot => transform.position;

	public override Vector3 OutputConnectorOffset => new Vector3(2.2f, 0f, 0f);

	public UseLever(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/UseLeverObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		Transform transform = gameObject.transform.FindChild("LeverUseCube");
		leverCollider = transform.GetComponent<Collider>();
		pushCollider = gameObject.transform.FindChild("EditCube").GetComponent<Collider>();
		plateButton = gameObject.transform.FindChildRecursively("Switch").gameObject;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.GetComponent<Collider>(), Use);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, new Vector3(0.5f, 1f, 0f));
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject);
		useInteractor.AddRequirement(useRequirement2);
		StarRequirement useRequirement3 = new StarRequirement(gameObject);
		useInteractor.AddRequirement(useRequirement3);
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
		useInteractor.UpdateData(Data);
		SetLinks(isActivated);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (isActivated && plateButton.transform.localPosition.z > minY)
		{
			float num = Mathf.Min(speed * Time.smoothDeltaTime, plateButton.transform.localPosition.z - minY);
			plateButton.transform.localPosition = new Vector3(plateButton.transform.localPosition.x, plateButton.transform.localPosition.y, plateButton.transform.localPosition.z - num);
		}
		else if (!isActivated && plateButton.transform.localPosition.z < 0f)
		{
			float num2 = Mathf.Min(speed * Time.smoothDeltaTime, 0f - plateButton.transform.localPosition.z);
			plateButton.transform.localPosition = new Vector3(plateButton.transform.localPosition.x, plateButton.transform.localPosition.y, plateButton.transform.localPosition.z + num2);
		}
	}

	public bool Use(int userWoID)
	{
		isActivated = !isActivated;
		if (isActivated)
		{
			MVGameControllerBase.Game.TriggerBoxEnter(Id, userWoID);
		}
		else
		{
			MVGameControllerBase.Game.TriggerBoxExit(Id, userWoID);
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
		gameObject.transform.FindChild("EditCube").gameObject.SetActive(value: false);
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
		useInteractor.UpdateData(Data);
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		if (avatarLocal != null)
		{
			if (isActivated)
			{
				MVGameControllerBase.Game.TriggerBoxEnter(Id, avatarLocal.Id);
			}
			else
			{
				MVGameControllerBase.Game.TriggerBoxExit(Id, avatarLocal.Id);
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
		useInteractor.OnDestroy(Data);
		base.Destroy();
	}

	public override void OnOutputLinkChanged()
	{
		base.OnOutputLinkChanged();
		Reset();
	}

	public void OnEditModeChange(EditModeChangeArgs arg)
	{
		pushCollider.enabled = true;
		leverCollider.enabled = false;
		if (arg.playInEditor)
		{
			pushCollider.enabled = false;
			leverCollider.enabled = true;
		}
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
}
