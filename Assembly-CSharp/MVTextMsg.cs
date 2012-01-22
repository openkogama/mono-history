using UnityEngine;

public class MVTextMsg : MVLogicObject, WorldObjectWithSettings
{
	private TextMesh textMesh;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => false;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/TextMsgObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		textMesh = gameObject.GetComponentInChildren<TextMesh>();
	}

	public override void Initialize()
	{
		OnDataUpdate();
		if (InputLinkRefs.Count == 0)
		{
			((Component)textMesh).renderer.enabled = true;
		}
		else
		{
			((Component)textMesh).renderer.enabled = false;
		}
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
		}
		else
		{
			((Component)textMesh).renderer.enabled = false;
		}
	}

	public override void OnDataUpdate()
	{
		if (Data.ContainsKey("text"))
		{
			textMesh.text = (string)Data["text"];
		}
	}
}
