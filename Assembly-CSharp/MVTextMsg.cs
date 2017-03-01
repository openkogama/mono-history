using System.Collections.Generic;
using UnityEngine;

public class MVTextMsg : MVLogicObject, ILogicWorldObject
{
	private Bounds localBounds;

	private MVTextMsgObject msgObject;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public IInputSignalReceiver InputSignalReceiver { get; private set; }

	public MVTextMsg(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVTextMsgPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanResetLogic;
		interactionFlags |= InteractionFlags.HasSettings;
		msgObject = (MVTextMsgObject)component;
		localBounds = ComputeLocalBounds(gameObject.transform.position, msgObject.MeshRenderers);
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return localBounds;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(msgObject.VisualObject);
		cullingSubscriberBase.Radius = msgObject.TextMeshRenderer.bounds.extents.magnitude;
		UpdateText();
		InputSignalReceiver = LogicClientsideFactory.CreateStateChangeInputSignalReceiver(this, defaultInput: true, null, InputStateUpdateCallback);
		ToggleText(InputSignalReceiver.CurrentlyIsHot);
	}

	private void ToggleText(bool visible)
	{
		msgObject.TextMeshRenderer.enabled = visible;
	}

	private void InputStateUpdateCallback(LogicInputState logicInputState, LogicObjectManager logicObjectManager)
	{
		if (logicInputState == LogicInputState.FromColdToHot)
		{
			ToggleText(visible: true);
		}
		if (logicInputState == LogicInputState.FromHotToCold)
		{
			ToggleText(visible: false);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		Data["text"] = string.Empty;
		UpdateText();
	}

	public override void OnDataUpdate()
	{
		UpdateText();
		LogicObjectManager.ResetChunk(Id, MVGameControllerBase.WOCM);
	}

	private void UpdateText()
	{
		if (Data.ContainsKey("text"))
		{
			msgObject.TextMesh.text = (string)Data["text"];
		}
		if (Data.ContainsKey("textSize"))
		{
			float num = (float)Data["textSize"];
			msgObject.TextMesh.transform.localScale = new Vector3(num, num, num);
		}
		else
		{
			msgObject.TextMesh.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		}
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Radius = msgObject.TextMeshRenderer.bounds.extents.magnitude;
		}
	}
}
