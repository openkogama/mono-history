using System;
using System.Collections.Generic;
using UnityEngine;

internal class ESRotating : ESStateBase
{
	private const float _rotationSpeed = 1f;

	private const float _rotationSpeedSnapping = 15f;

	private float prevMouseX;

	private float rotateThreshold = 10f;

	private float xAcc;

	private float mouseSensitivity = 10f;

	private List<MVWorldObjectClient> targets = new List<MVWorldObjectClient>();

	private Vector3 pivot;

	private ILaserPointer laser;

	public override void Enter(EditorStateMachine e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)GetType().ToString());
		targets = new List<MVWorldObjectClient>();
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			e.PopState();
			return;
		}
		prevMouseX = Input.mousePosition.x;
		List<Transform> list = new List<Transform>();
		foreach (int selectedID in e.SelectedIDs)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(selectedID);
			targets.Add(worldObjectClient);
			list.Add(worldObjectClient.Transform);
		}
		if (targets.Count == 1)
		{
			pivot = targets[0].WorldPivot;
		}
		else
		{
			pivot = SharedCubeFunctions.GetWorldCenter(list);
		}
		laser = MVGameController.Instance.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Transforming);
		laser.LaserActive = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		float num = 1f;
		if (e.GridMode)
		{
			num = 15f;
		}
		if (MVInputWrapper.GetKey((KeyCode)323))
		{
			xAcc += (Input.mousePosition.x - prevMouseX) * (mouseSensitivity / num);
			while (Mathf.Abs(xAcc) > rotateThreshold)
			{
				float num2 = xAcc / Mathf.Abs(xAcc);
				float num3 = 0f;
				if (targets.Count == 1)
				{
					targets[0].WorldEulerAngles = MathFunctions.RoundVector(targets[0].WorldEulerAngles, 0);
					double value = Math.Round(targets[0].WorldEulerAngles.y) % (double)num;
					num3 = (float)Math.Round(value, 0);
				}
				if (targets.Count == 1)
				{
					Vector3 axis = Vector3.up;
					if (targets[0].HasInteractionFlag(InteractionFlags.CanRotateX))
					{
						axis = Vector3.right;
					}
					else if (targets[0].HasInteractionFlag(InteractionFlags.CanRotateY))
					{
						axis = Vector3.up;
					}
					else if (targets[0].HasInteractionFlag(InteractionFlags.CanRotateZ))
					{
						axis = Vector3.forward;
					}
					targets[0].RotateAround(pivot, axis, (0f - num2) * num - num3);
					targets[0].SyncRot = targets[0].WorldRotation;
				}
				else
				{
					foreach (MVWorldObjectClient target in targets)
					{
						target.RotateAround(pivot, Vector3.up, (0f - num2) * num - num3);
						target.SyncRot = target.WorldRotation;
					}
				}
				xAcc -= num2 * rotateThreshold;
			}
			prevMouseX = Input.mousePosition.x;
			laser.UpdatePosition(pivot);
		}
		else
		{
			e.PopState();
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		laser.ChangeState(LaserPointerState.Idle);
		laser.LaserActive = false;
		DoGridSnapping();
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
	}

	private void DoGridSnapping()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		num = ((!AEditController.IsGridSnap()) ? 0.0625f : 1f);
		foreach (MVWorldObjectClient target in targets)
		{
			target.SyncPos = target.GetClosestGridPoint(num, target.WorldPosition);
		}
	}
}
