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

	private List<WorldObjectClientRef> targets = new List<WorldObjectClientRef>();

	private Vector3 pivot;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log(GetType().ToString());
		targets = new List<WorldObjectClientRef>();
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
			WorldObjectClientRef worldObjectClientRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(selectedID);
			targets.Add(worldObjectClientRef);
			list.Add(worldObjectClientRef.WorldObjectClient.Transform);
		}
		if (targets.Count == 1)
		{
			pivot = targets[0].WorldObjectClient.WorldPivot;
		}
		else
		{
			pivot = SharedCubeFunctions.GetWorldCenter(list);
		}
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Transforming);
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.SetLaserActiveState(isActive: true);
	}

	private bool ValidateTargets()
	{
		foreach (WorldObjectClientRef target in targets)
		{
			if (target.WorldObjectClient == null)
			{
				return false;
			}
		}
		return true;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (!ValidateTargets())
		{
			e.PopState();
		}
		else if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect))
		{
			xAcc += (MVInputWrapper.GetPointerPosition().x - prevMouseX) * (mouseSensitivity / rotationSpeed);
			while (Mathf.Abs(xAcc) > rotateThreshold)
			{
				MVWorldObjectClient worldObjectClient = targets[0].WorldObjectClient;
				float num = xAcc / Mathf.Abs(xAcc);
				float num2 = 0f;
				if (targets.Count == 1)
				{
					worldObjectClient.WorldEulerAngles = MathFunctions.RoundVector(worldObjectClient.WorldEulerAngles, 0);
					double value = Math.Round(worldObjectClient.WorldEulerAngles.y) % (double)rotationSpeed;
					num2 = (float)Math.Round(value, 0);
				}
				if (targets.Count == 1)
				{
					Vector3 axis = Vector3.up;
					if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanRotateX))
					{
						axis = Vector3.right;
					}
					else if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanRotateY))
					{
						axis = Vector3.up;
					}
					else if (worldObjectClient.HasInteractionFlag(InteractionFlags.CanRotateZ))
					{
						axis = Vector3.forward;
					}
					worldObjectClient.RotateAround(pivot, axis, (0f - num) * rotationSpeed - num2);
					worldObjectClient.SyncRot = worldObjectClient.WorldRotation;
				}
				else
				{
					foreach (WorldObjectClientRef target in targets)
					{
						MVWorldObjectClient worldObjectClient2 = target.WorldObjectClient;
						worldObjectClient2.RotateAround(pivot, Vector3.up, (0f - num) * rotationSpeed - num2);
						worldObjectClient2.SyncRot = worldObjectClient2.WorldRotation;
					}
				}
				xAcc -= num * rotateThreshold;
			}
			prevMouseX = MVInputWrapper.GetPointerPosition().x;
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.UpdatePosition(pivot);
		}
		else
		{
			e.PopState();
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ChangeState(LaserPointerState.Idle);
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.SetLaserActiveState(isActive: false);
		DoGridSnapping();
		e.NetworkSelector.RequestReleaseOwnership(e.SelectedIDs);
	}

	private void DoGridSnapping()
	{
		float num = 0f;
		num = ((!MVGameControllerBase.EditModeUI.IsGridSnap()) ? 0.0625f : 1f);
		foreach (WorldObjectClientRef target in targets)
		{
			MVWorldObjectClient worldObjectClient = target.WorldObjectClient;
			if (worldObjectClient != null)
			{
				worldObjectClient.SyncPos = worldObjectClient.GetClosestGridPoint(num, worldObjectClient.WorldPosition);
			}
		}
	}
}
