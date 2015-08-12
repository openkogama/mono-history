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

	public MVTextMsg(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TextMsgObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		localBounds = ComputeLocalBounds(gameObject.transform.position, new MeshRenderer[1] { gameObject.GetComponent<MeshRenderer>() });
		textMesh = gameObject.GetComponentInChildren<TextMesh>();
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
			textMesh.GetComponent<Renderer>().enabled = true;
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
			textMesh.GetComponent<Renderer>().enabled = true;
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
			textMesh.GetComponent<Renderer>().enabled = true;
			textVisible = true;
		}
		else
		{
			textMesh.GetComponent<Renderer>().enabled = false;
			textVisible = false;
		}
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("text"))
		{
			textMesh.text = (string)Data["text"];
		}
		if (Data.ContainsKey("textSize"))
		{
			float num = (float)Data["textSize"];
			textMesh.transform.localScale = new Vector3(num, num, num);
		}
		else
		{
			textMesh.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		}
	}

	public override void ChangeLOD(float distance)
	{
		bool flag = disabledByLod;
		base.ChangeLOD(distance);
		if (flag != disabledByLod)
		{
			textMesh.GetComponent<Renderer>().enabled = textVisible && !disabledByLod;
		}
	}
}
