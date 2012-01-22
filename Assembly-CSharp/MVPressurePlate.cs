using MV.WorldObject;
using UnityEngine;

public class MVPressurePlate : MVLogicObject
{
	private TriggerBoxEvents triggerBoxEvents;

	private GameObject plateModel;

	private bool isDown;

	private float minY = -0.249f;

	private float speed = 0.38f;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public override Vector3 OutputConnectorOffset
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(2f, 0f, 0f);
		}
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

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/PressurePlateObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		plateModel = ((Component)gameObject.GetComponentInChildren<Animation>()).gameObject;
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

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Playing)
		{
			MVGameController.Instance.Game.TriggerBoxEnter(this);
			isDown = true;
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Playing)
		{
			MVGameController.Instance.Game.TriggerBoxExit(this);
		}
	}

	public void OnEnter(MVPlayer player)
	{
		if (player != MVGameController.Instance.WOCM.LocalPlayer)
		{
		}
	}

	public void OnExit(MVPlayer player)
	{
		if (player != MVGameController.Instance.WOCM.LocalPlayer)
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
	}
}
