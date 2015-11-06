using System;
using System.Collections.Generic;
using UnityEngine;

internal class ESRotating : ESStateBase
{
	private float rotationSpeed = 15f;

	private float prevMouseX;

	private float rotateThreshold = 10f;

	private float xAcc;

	private float mouseSensitivity = 10f;

	private List<MVWorldObjectClient> targets = new List<MVWorldObjectClient>();

	private Vector3 pivot;

	private ILaserPointer laser;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log(GetType().ToString());
		targets = new List<MVWorldObjectClient>();
		if (!e.NetworkSelector.RequestOwnership(e.SelectedIDs))
		{
			e.PopState();
			return;
		}
		rotationSpeed = (float)e.Data["rotationDegreesStep"];
		prevMouseX = MVInputWrapper.GetPointerPosition().x;
		List<Transform> list = new List<Transform>();
		foreach (int selectedID in e.SelectedIDs)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(selectedID);
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
		laser = MVGameControllerBase.WOCM.AvatarLocal.LaserPointer;
		laser.ChangeState(LaserPointerState.Transforming);
		laser.LaserActive = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect))
		{
			xAcc += (MVInputWrapper.GetPointerPosition().x - prevMouseX) * (mouseSensitivity / rotationSpeed);
			while (Mathf.Abs(xAcc) > rotateThreshold)
			{
				float num = xAcc / Mathf.Abs(xAcc);
				float num2 = 0f;
				if (targets.Count == 1)
				{
					targets[0].WorldEulerAngles = MathFunctions.RoundVector(targets[0].WorldEulerAngles, 0);
					double value = Math.Round(targets[0].WorldEulerAngles.y) % (double)rotationSpeed;
					num2 = (float)Math.Round(value, 0);
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
					targets[0].RotateAround(pivot, axis, (0f - num) * rotationSpeed - num2);
					targets[0].SyncRot = targets[0].WorldRotation;
				}
				else
				{
					foreach (MVWorldObjectClient target in targets)
					{
						target.RotateAround(pivot, Vector3.up, (0f - num) * rotationSpeed - num2);
						target.SyncRot = target.WorldRotation;
					}
				}
				xAcc -= num * rotateThreshold;
			}
			prevMouseX = MVInputWrapper.GetPointerPosition().x;
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
		float num = 0f;
		num = ((!MVGameControllerLegacyUI.EditorController.IsGridSnap()) ? 0.0625f : 1f);
		foreach (MVWorldObjectClient target in targets)
		{
			target.SyncPos = target.GetClosestGridPoint(num, target.WorldPosition);
		}
	}
}
