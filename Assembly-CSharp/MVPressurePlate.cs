using System.Collections;
using System.Collections.Generic;
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

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(2f, 0.25f, 0f);
		}
	}

	public MVPressurePlate(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/PressurePlateObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		plateModel = ((Component)gameObject.GetComponentInChildren<Animation>()).gameObject;
		SetVisibility();
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, Vector3.one * 2f);
	}

	protected override void OnUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
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

	public override void OnDataUpdate()
	{
		SetVisibility();
	}

	public override void Initialize()
	{
		base.Initialize();
		SetVisibility();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		((Component)gameObject.transform.FindChild("TriggerCube")).gameObject.active = false;
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameController.Instance.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(e.instigatorWOID);
		if (woIDWithLocalOwnerHighestInHierarchy == -1)
		{
			Debug.LogError((object)"Pressure plate entered by object which is not owned locally");
			return;
		}
		MVGameController.Instance.Game.TriggerBoxEnter(Id, woIDWithLocalOwnerHighestInHierarchy);
		isDown = true;
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		int woIDWithLocalOwnerHighestInHierarchy = MVGameController.Instance.WOCM.GetWoIDWithLocalOwnerHighestInHierarchy(e.instigatorWOID);
		if (woIDWithLocalOwnerHighestInHierarchy == -1)
		{
			Debug.LogError((object)"Pressure plated exited by object which is not owned locally. This might be ok?");
		}
		else
		{
			MVGameController.Instance.Game.TriggerBoxExit(Id, woIDWithLocalOwnerHighestInHierarchy);
		}
	}

	public void OnEnter(MVPlayer player)
	{
		if (player != MVGameController.Instance.Game.LocalPlayer)
		{
		}
	}

	public void OnExit(MVPlayer player)
	{
		if (player != MVGameController.Instance.Game.LocalPlayer)
		{
		}
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
		Debug.Log((object)"TriggerBox destroy...");
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit -= triggerBoxEvents_TriggerExit;
		base.Destroy();
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
