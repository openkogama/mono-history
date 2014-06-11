using MV.Common;
using UnityEngine;

public class VehicleSeatBase : MonoBehaviour
{
	public SeatType SeatType;

	public bool UnequipVehicleUser;

	public MVCameraBase Camera;

	public Transform AvatarAttachPoint;

	private MVAvatar owner;

	private int seatID = -1;

	public bool IsOccupied { get; set; }

	public MVAvatar Owner => owner;

	public int SeatID
	{
		get
		{
			return seatID;
		}
		set
		{
			if (seatID == -1)
			{
				seatID = value;
			}
			else
			{
				Debug.LogError((object)"Trying to re-set seatID");
			}
		}
	}

	public void SetCamera()
	{
		if (!((Object)(object)Camera == (Object)null))
		{
			MVGameController.Instance.Game.CameraController.SetCamera(Camera);
		}
	}

	public virtual void Attach(MVAvatar avatar)
	{
		if (UnequipVehicleUser)
		{
			MVEquipable component = avatar.GameObject.GetComponent<MVEquipable>();
			if ((Object)(object)component != (Object)null)
			{
				component.Unequip();
			}
		}
		owner = avatar;
	}

	public virtual void Detach(MVAvatar avatar)
	{
	}
}
