using MV.Common;
using UnityEngine;

public class DummyLaserPointer : PickupItem, ILaserPointer
{
	public override bool ActivateGunModeOnEquip => false;

	public override AvatarItemType Type => AvatarItemType.LaserPointer;

	public byte CurrentCubeMaterial
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public bool LaserActive
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public override int MaxQuantity => 0;

	public void ActivateLaserForDuration(float duration)
	{
	}

	public void ChangeState(LaserPointerState newState)
	{
	}

	public void SetLaserCubeVisible(bool visible)
	{
	}

	public void UpdatePosition(Vector3 to)
	{
	}
}
