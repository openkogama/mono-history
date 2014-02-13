using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVTextMsg : MVLogicObject
{
	private const string prefabPath = "Prefabs/TextMsgObject";

	private bool textVisible = true;

	private Bounds localBounds;

	private TextMesh textMesh;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	public MVTextMsg(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TextMsgObject", worldObjects)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags |= InteractionFlags.HasSettings;
		localBounds = ComputeLocalBounds(gameObject.transform.position, new MeshRenderer[1] { gameObject.GetComponent<MeshRenderer>() });
		textMesh = gameObject.GetComponentInChildren<TextMesh>();
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return localBounds;
	}

	public override void Initialize()
	{
		base.Initialize();
		OnDataUpdate();
		if (InputLinkRefs.Count == 0)
		{
			((Component)textMesh).renderer.enabled = true;
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
			((Component)textMesh).renderer.enabled = true;
		}
		else
		{
			OnInputStateChanged();
		}
	}

	public override void OnInputStateChanged()
	{
		if (InputState)
		{
			((Component)textMesh).renderer.enabled = true;
			textVisible = true;
		}
		else
		{
			((Component)textMesh).renderer.enabled = false;
			textVisible = false;
		}
	}

	public override void OnDataUpdate()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (Data.ContainsKey("text"))
		{
			textMesh.text = (string)Data["text"];
		}
		if (Data.ContainsKey("textSize"))
		{
			float num = (float)Data["textSize"];
			((Component)textMesh).transform.localScale = new Vector3(num, num, num);
		}
		else
		{
			((Component)textMesh).transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		}
	}

	public override void ChangeLOD(float distance)
	{
		bool flag = disabledByLod;
		base.ChangeLOD(distance);
		if (flag != disabledByLod)
		{
			((Component)textMesh).renderer.enabled = textVisible && !disabledByLod;
		}
	}
}
