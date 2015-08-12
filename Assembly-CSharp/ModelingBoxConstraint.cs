using System;
using MV.WorldObject;
using UnityEngine;

public class ModelingBoxConstraint : IModelingConstraint
{
	private ObscuredIntVector minCorner;

	private ObscuredIntVector maxCorner;

	private Vector3 center;

	public ObscuredIntVector MinCorner
	{
		get
		{
			return minCorner;
		}
		protected set
		{
			if (minCorner != value)
			{
				minCorner = value;
				FMinCorner = minCorner.ToVector3();
				ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, new IntVector(minCorner.x, minCorner.y, minCorner.z), new IntVector(maxCorner.x, maxCorner.y, maxCorner.z));
				OnBoxChanged(args);
			}
		}
	}

	public ObscuredIntVector MaxCorner
	{
		get
		{
			return maxCorner;
		}
		protected set
		{
			if (maxCorner != value)
			{
				maxCorner = value;
				FMinCorner = maxCorner.ToVector3();
				ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, new IntVector(minCorner.x, minCorner.y, minCorner.z), new IntVector(maxCorner.x, maxCorner.y, maxCorner.z));
				OnBoxChanged(args);
			}
		}
	}

	public Vector3 FMinCorner { get; private set; }

	public Vector3 FMaxCorner { get; private set; }

	public Vector3 Center
	{
		get
		{
			return center;
		}
		protected set
		{
			if (center != value)
			{
				center = value;
				ConstraintBoxChangedEventArgs args = new ConstraintBoxChangedEventArgs(center, new IntVector(minCorner.x, minCorner.y, minCorner.z), new IntVector(maxCorner.x, maxCorner.y, maxCorner.z));
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
			Debug.Log("Size parameter fields shouldn't be odd: " + size);
		}
	}

	public ModelingBoxConstraint(IntVector minCorner, IntVector maxCorner)
	{
		ChangeBox(minCorner, maxCorner);
	}

	protected void ChangeBox(IntVector minCorner, IntVector maxCorner)
	{
		this.minCorner = new ObscuredIntVector(minCorner);
		this.maxCorner = new ObscuredIntVector(maxCorner);
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
		if (pos.x < (short)MinCorner.x || pos.y < (short)MinCorner.y || pos.z < (short)MinCorner.z)
		{
			return false;
		}
		if ((short)MaxCorner.x < pos.x || (short)MaxCorner.y < pos.y || (short)MaxCorner.z < pos.z)
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
