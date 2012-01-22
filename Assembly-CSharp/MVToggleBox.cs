using MV.WorldObject;
using UnityEngine;

public class MVToggleBox : MVLogicObject, WorldObjectWithSettings, WorldObjectWithLogicReset
{
	private bool currentOutputState;

	public override bool HasInputConnector => true;

	public override bool HasOutputConnector => true;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ToggleBoxObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
	}

	public override void OnDataUpdate()
	{
		currentOutputState = (bool)Data["state"];
		foreach (Link outputLinkRef in OutputLinkRefs)
		{
			outputLinkRef.isSet = currentOutputState;
		}
	}
}
