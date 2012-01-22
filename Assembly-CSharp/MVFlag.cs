using System;
using UnityEngine;

public class MVFlag : MVLogicObject
{
	private FlagEvents flagEvents;

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
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/FlagObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		flagEvents = gameObject.GetComponentInChildren<FlagEvents>();
		flagEvents.FlagCaptured += flagEvents_FlagCaptured;
	}

	private void flagEvents_FlagCaptured(object sender, EventArgs e)
	{
		Debug.Log((object)"FLAG CAPTURED!");
		MVGameController.Instance.Game.ReportCaptureFlag();
	}

	public override void Destroy()
	{
		flagEvents.FlagCaptured -= flagEvents_FlagCaptured;
	}
}
