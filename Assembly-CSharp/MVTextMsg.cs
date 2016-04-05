using System.Collections.Generic;
using UnityEngine;

public class MVTextMsg : MVLogicObject
{
	private bool textVisible = true;

	private Bounds localBounds;

	private MVTextMsgObject msgObject;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVTextMsg(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVTextMsgPrefab, worldObjects)
	{
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
		OnDataUpdate();
		if (InputLinkRefs.Count == 0)
		{
			msgObject.TextMeshRenderer.enabled = (textVisible = true);
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		Data["text"] = string.Empty;
		OnDataUpdate();
	}

	public override void OnInputLinkChanged()
	{
		if (InputLinkRefs.Count == 0)
		{
			msgObject.TextMeshRenderer.enabled = (textVisible = true);
		}
		else
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputStateChanged()
	{
		msgObject.TextMeshRenderer.enabled = (textVisible = InputState);
	}

	public override void OnDataUpdate()
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
	}

	public override void ChangeLOD(float distance)
	{
		bool flag = disabledByLod;
		base.ChangeLOD(distance);
		if (flag != disabledByLod)
		{
			msgObject.TextMeshRenderer.enabled = textVisible && !disabledByLod;
		}
	}
}
