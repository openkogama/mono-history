using UnityEngine;

internal class ESWaitForSelected : ESStateBase
{
	private float minDistance = 4f;

	private float maxDistance = 25f;

	private float maxDistanceBase = 3.5f;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log((object)"wait for selected");
		e.DeSelect();
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (e.SingleSelectedWO != null)
		{
			Vector3 val = ((Component)e.WeCamera).transform.rotation * Vector3.forward;
			val.y = 0f;
			val.Normalize();
			Ray val2 = new Ray(MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position, val);
			e.SingleSelectedWO.GetTranslateData(out var tod, maxDistanceBase, minDistance, maxDistance);
			Vector3 position = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position;
			Bounds value = tod.worldBounds.Value;
			float y = value.min.y;
			float y2 = e.SingleSelectedWO.GameObject.transform.position.y;
			float num = y2 - y;
			val2.origin += val2.direction * tod.maxDistance;
			val2.origin += Vector3.up * num;
			float num2 = 0f;
			num2 = ((!MVGameController.Instance.EditorController.GridSnap) ? 0.0625f : 1f);
			e.SingleSelectedWO.GameObject.transform.position = e.SingleSelectedWO.GetClosestGridPoint(num2, val2.origin);
			e.PushState(EditorEvent.ESTranslate, EditorEvent.ObjectSelected);
		}
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
