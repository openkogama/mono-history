using System;
using System.Runtime.CompilerServices;
using MV.WorldObject;
using UnityEngine;

public class ModelingBoxConstraint : IModelingConstraint
{
	private IntVector minCorner;

	private IntVector maxCorner;

	private Vector3 center;

	public IntVector MinCorner
	{
		get
		{
			return minCorner;
		}
		protected set
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (minCorner != value)
			{
				minCorner = value;
				FMinCorner = minCorner.ToVector3();
				ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, minCorner, maxCorner);
				OnBoxChanged(args);
			}
		}
	}

	public IntVector MaxCorner
	{
		get
		{
			return maxCorner;
		}
		protected set
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (maxCorner != value)
			{
				maxCorner = value;
				FMinCorner = maxCorner.ToVector3();
				ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, minCorner, maxCorner);
				OnBoxChanged(args);
			}
		}
	}

	public Vector3 FMinCorner
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public Vector3 FMaxCorner
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public Vector3 Center
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return center;
		}
		protected set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (center != value)
			{
				center = value;
				ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, minCorner, maxCorner);
				OnBoxChanged(args);
			}
		}
	}

	public event EventHandler<ConstraintBoxChangedEventArgs> BoxChanged = delegate
	{
	};

	public ModelingBoxConstraint(IntVector size)
		: this(-size / 2, size / 2)
	{
		if (size.x % 2 == 1 || size.y % 2 == 1 || size.z % 2 == 1)
		{
			Debug.Log((object)("Size parameter fields shouldn't be odd: " + size));
		}
	}

	public ModelingBoxConstraint(IntVector minCorner, IntVector maxCorner)
	{
		ChangeBox(minCorner, maxCorner);
	}

	protected void ChangeBox(IntVector minCorner, IntVector maxCorner)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		this.minCorner = minCorner;
		this.maxCorner = maxCorner;
		FMinCorner = minCorner.ToVector3();
		FMaxCorner = maxCorner.ToVector3();
		center = 0.5f * (FMinCorner + FMaxCorner);
		ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, minCorner, maxCorner);
		OnBoxChanged(args);
	}

	protected virtual void OnBoxChanged(ConstraintBoxChangedEventArgs args)
	{
		BoxChanged(this, args);
	}

	public virtual bool CanAddCubeAt(IntVector pos)
	{
		if (pos.x < MinCorner.x || pos.y < MinCorner.y || pos.z < MinCorner.z)
		{
			return false;
		}
		if (MaxCorner.x < pos.x || MaxCorner.y < pos.y || MaxCorner.z < pos.z)
		{
			return false;
		}
		return true;
	}

	public virtual bool CanRemoveCubeAt(IntVector pos)
	{
		return true;
	}

	public virtual bool CanEditCubeAt(IntVector pos)
	{
		return true;
	}
}
