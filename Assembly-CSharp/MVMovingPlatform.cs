using System.Collections;
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
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (end == null || start == null)
			{
				return Velocity;
			}
			Vector3 val = end.Position - start.Position;
			Vector3 normalized = val.normalized;
			Vector3 val2 = Velocity;
			val = normalized * val2.magnitude;
			val = Vector4.op_Implicit(start.Transform.localToWorldMatrix * Vector4.op_Implicit(val));
			return val;
		}
	}

	public MVMovingPlatform(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		if (CubeModel == null)
		{
			Debug.LogWarning((object)("Moving platform " + id + " init - movable's cube model is NULL! If this is a new platform group restart the session. Otherwise it is broken."));
			return;
		}
		IntVector min = new IntVector(-5, -2, -5);
		IntVector max = new IntVector(5, 2, 5);
		CubeModel.ModelingConstraintBuilder = () => new ModelingBoxCountConstraint(CubeModel, min, max, 1);
		CubeModel.InteractionFlags &= ~InteractionFlags.DirectlySelectable;
		CubeModel.BeingEditedChanged += MVCubeModelBase_BeingEditedChanged;
	}

	public void MoveBetweenNodes(MVMovingPlatformNode start, MVMovingPlatformNode end)
	{
		this.start = start;
		this.end = end;
		RecalculateMovement();
	}

	public void RecalculateMovement()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end.WorldPosition - start.WorldPosition;
		WorldPosition = start.WorldPosition;
		Vector3 val2 = val;
		val2.y = 0f;
		val2.Normalize();
		if (0.5f < val2.magnitude)
		{
			WorldRotation = Quaternion.LookRotation(val2, Vector3.up);
		}
		Vector3 val3 = Velocity;
		float magnitude = val3.magnitude;
		Vector3 val4 = ((!(0f < magnitude)) ? val.normalized : (val.normalized * magnitude));
		SetDistance(val.magnitude);
		SetOrgRotation(Quaternion.LookRotation(val.normalized));
		SetVelocity(val4);
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
