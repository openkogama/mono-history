using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public class MVPressurePlate : MVLogicObject
{
	private const string prefabPath = "Prefabs/PressurePlateObject";

	private TriggerBoxEvents triggerBoxEvents;

	private GameObject plateModel;

	private bool isDown;

	private float minY = -0.249f;

	private float speed = 1.8f;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private UseInteractor useInteractor;

	private Vector3 gameCoinDisplayObjectOffset = new Vector3(0f, 0.9f, 0f);

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset => new Vector3(2f, 0.25f, 0f);

	public override Vector3 WorldPivot => SharedCubeFunctions.GetWorldCenter(transform) + transform.rotation * (0.5f * Vector3.left);

	public MVPressurePlate(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/PressurePlateObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		plateModel = gameObject.GetComponentInChildren<Animation>().gameObject;
		SetVisibility();
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.GetComponent<Collider>(), DoEnter);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, gameCoinDisplayObjectOffset, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
		StarRequirement useRequirement3 = new StarRequirement(gameObject, hasUseButtonWhenFree: false);
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
		useInteractor.UpdateData(Data);
		if (RunTimeData.ContainsObscuredKey("instigator"))
		{
			ObscuredInt obscuredInt = (ObscuredInt)RunTimeData.GetObscuredType("instigator");
			if ((int)obscuredInt != 0)
			{
				MVGameControllerBase.Game.TriggerBoxEnter(Id, obscuredInt);
			}
		}
		SetVisibility();
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		SetVisibility();
	}

	protected override void OnUpdate()
	{
		if (isDown && plateModel.transform.localPosition.y > minY)
		{
			float num = Mathf.Min(speed * Time.smoothDeltaTime, plateModel.transform.localPosition.y - minY);
			plateModel.transform.localPosition = new Vector3(plateModel.transform.localPosition.x, plateModel.transform.localPosition.y - num, plateModel.transform.localPosition.z);
		}
		else if (!isDown && plateModel.transform.localPosition.y < 0f)
		{
			float num2 = Mathf.Min(speed * Time.smoothDeltaTime, 0f - plateModel.transform.localPosition.y);
			plateModel.transform.localPosition = new Vector3(plateModel.transform.localPosition.x, plateModel.transform.localPosition.y + num2, plateModel.transform.localPosition.z);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		gameObject.transform.FindChild("TriggerCube").gameObject.SetActive(value: false);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if ((useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(e.instigatorWOID);
			if (woIDWithLocalOwnerHighestInHierarchy == -1)
			{
				Debug.LogError("Pressure plate entered by object which is not owned locally");
				return;
			}
			DoEnter(woIDWithLocalOwnerHighestInHierarchy);
			isDown = true;
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameControllerBase.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(e.instigatorWOID);
		if (woIDWithLocalOwnerHighestInHierarchy == -1)
		{
			Debug.LogError("Pressure plated exited by object which is not owned locally. This might be ok?");
		}
		else
		{
			DoExit(woIDWithLocalOwnerHighestInHierarchy);
		}
	}

	private bool DoEnter(int instigatorWOID)
	{
		MVGameControllerBase.Game.TriggerBoxEnter(Id, instigatorWOID);
		return true;
	}

	private void DoExit(int instigatorWOID)
	{
		MVGameControllerBase.Game.TriggerBoxExit(Id, instigatorWOID);
	}

	public void OnStayBegin(int actorNr)
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = true;
		}
		isDown = true;
	}

	public void OnStayEnd()
	{
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = false;
		}
		isDown = false;
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit -= triggerBoxEvents_TriggerExit;
		useInteractor.OnDestroy(Data);
		base.Destroy();
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, new Vector3(2.5f, 0.4f, 2.5f));
	}

	private bool IsVisible()
	{
		if (!Data.ContainsKey("hide"))
		{
			return true;
		}
		return !(bool)Data["hide"];
	}

	private void SetVisibility()
	{
		Renderer componentInChildren = plateModel.GetComponentInChildren<Renderer>();
		componentInChildren.enabled = IsVisible() && !disabledByLod;
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
