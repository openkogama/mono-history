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

	public override void Enter(EditorStateMachine e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)GetType().ToString());
		targets = new List<MVWorldObjectClient>();
		if (!e.NetworkSelector.RequestOwnership(e.Selected))
		{
			e.PopState();
			return;
		}
		prevMouseX = Input.mousePosition.x;
		foreach (int item in e.Selected)
		{
			targets.Add(MVGameController.Instance.WOCM.GetWorldObjectClient(item));
		}
		pivot = SharedCubeFunctions.GetWorldCenter(targets);
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
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
					Vector3 vector = targets[0].GameObject.transform.eulerAngles;
					MathFunctions.RoundVector(ref vector, 0);
					targets[0].GameObject.transform.eulerAngles = vector;
					num3 = (float)Math.Round((float)Math.Round(targets[0].GameObject.transform.eulerAngles.y) % num, 0);
				}
				foreach (MVWorldObjectClient target in targets)
				{
					target.GameObject.transform.RotateAround(pivot, Vector3.up, (0f - num2) * num - num3);
					target.SyncRot = target.GameObject.transform.rotation;
				}
				xAcc -= num2 * rotateThreshold;
			}
			prevMouseX = Input.mousePosition.x;
		}
		else
		{
			e.PopState();
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		DoGridSnapping();
		e.NetworkSelector.RequestReleaseOwnership(e.Selected);
	}

	private void DoGridSnapping()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		num = ((!MVGameController.Instance.EditorController.GridSnap) ? 0.0625f : 1f);
		foreach (MVWorldObjectClient target in targets)
		{
			target.GameObject.transform.position = target.GetClosestGridPoint(num, target.GameObject.transform.position);
			target.SyncPos = target.GameObject.transform.position;
		}
	}
}
