using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVMovingPlatform : MVMovable
{
	private MVMovingPlatformNode start;

	private MVMovingPlatformNode end;

	public MVMovingPlatformNode Start => start;

	public MVMovingPlatformNode End => end;

	protected override Vector3 WorldVelocity
	{
		get
		{
			if (end == null || start == null)
			{
				return Velocity;
			}
			Vector3 vector = (end.Position - start.Position).normalized * Velocity.magnitude;
			return start.Transform.localToWorldMatrix * vector;
		}
	}

	public MVMovingPlatform(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		if (CubeModel == null)
		{
			Debug.LogWarning("Moving platform " + id + " init - movable's cube model is NULL! If this is a new platform group restart the session. Otherwise it is broken.");
			return;
		}
		IntVector min = new IntVector(-5, -2, -5);
		IntVector max = new IntVector(5, 2, 5);
		CubeModel.ModelingConstraintBuilder = () => new ModelingBoxCountConstraint(CubeModel, min, max, 1);
		CubeModel.InteractionFlags &= ~InteractionFlags.DirectlySelectable;
		CubeModel.BeingEditedChanged += MVCubeModelBase_BeingEditedChanged;
		CubeModel.SetupCulling(OnStateChanged);
		CubeModel.SetCullDistanceBand(2);
	}

	public void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool visible = CubeModel.IsLodVisible(cullingGroupEvent);
		SetVisible(visible);
	}

	public void MoveBetweenNodes(MVMovingPlatformNode start, MVMovingPlatformNode end)
	{
		this.start = start;
		this.end = end;
		RecalculateMovement();
	}

	public void RecalculateMovement()
	{
		Vector3 vector = end.WorldPosition - start.WorldPosition;
		WorldPosition = start.WorldPosition;
		Vector3 forward = vector;
		forward.y = 0f;
		forward.Normalize();
		if (0.5f < forward.magnitude)
		{
			WorldRotation = Quaternion.LookRotation(forward, Vector3.up);
		}
		float magnitude = Velocity.magnitude;
		Vector3 vector2 = ((!(0f < magnitude)) ? vector.normalized : (vector.normalized * magnitude));
		SetDistance(vector.magnitude);
		SetOrgRotation(Quaternion.LookRotation(vector.normalized));
		SetVelocity(vector2);
	}

	private void MVCubeModelBase_BeingEditedChanged(object sender, EditStateEventArgs e)
	{
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)sender;
		PausedMovement = mVCubeModelBase.BeingEdited;
	}

	protected override void OnSelectedChanged(bool selected)
	{
		PausedMovement = selected;
		base.OnSelectedChanged(selected);
	}
}
