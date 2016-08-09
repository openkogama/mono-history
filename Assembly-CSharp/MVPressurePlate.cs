using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public class MVPressurePlate : MVLogicObject
{
	private MVPressurePlateObject plateObject;

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
		: base(data, PrefabPool.Instance.MVPressurePlatePrefab, worldObjects)
	{
		plateObject = (MVPressurePlateObject)component;
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseStars;
		plateObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		plateObject.TriggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		SetVisibility();
		useInteractor = new UseInteractor(Id, plateObject.useInteractionRotator, reset: false, plateObject.TriggerBoxEvents.Collider, DoEnter);
		plateObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		plateObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(plateObject.useInteractionRotator, gameCoinDisplayObjectOffset, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(plateObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
		StarRequirement useRequirement3 = new StarRequirement(plateObject.useInteractionRotator, hasUseButtonWhenFree: false);
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
				MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, obscuredInt);
			}
		}
		SetVisibility();
		SetupCulling(plateObject.gameObject);
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		SetVisibility();
	}

	protected override void OnUpdate()
	{
		if (isDown && plateObject.PlateModelTranform.localPosition.y > minY)
		{
			float num = Mathf.Min(speed * Time.smoothDeltaTime, plateObject.PlateModelTranform.localPosition.y - minY);
			plateObject.PlateModelTranform.localPosition = new Vector3(plateObject.PlateModelTranform.localPosition.x, plateObject.PlateModelTranform.localPosition.y - num, plateObject.PlateModelTranform.localPosition.z);
		}
		else if (!isDown && plateObject.PlateModelTranform.localPosition.y < 0f)
		{
			float num2 = Mathf.Min(speed * Time.smoothDeltaTime, 0f - plateObject.PlateModelTranform.localPosition.y);
			plateObject.PlateModelTranform.localPosition = new Vector3(plateObject.PlateModelTranform.localPosition.x, plateObject.PlateModelTranform.localPosition.y + num2, plateObject.PlateModelTranform.localPosition.z);
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
		MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, instigatorWOID);
		return true;
	}

	private void DoExit(int instigatorWOID)
	{
		MVGameControllerBase.OperationRequests.TriggerBoxExit(Id, instigatorWOID);
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
		plateObject.TriggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		plateObject.TriggerBoxEvents.TriggerExit -= triggerBoxEvents_TriggerExit;
		plateObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		plateObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
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
		MeshRenderer[] meshRenderers = plateObject.MeshRenderers;
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			meshRenderers[i].enabled = IsVisible() && !disabledByLod;
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
